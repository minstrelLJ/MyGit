using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Tools;

namespace MySocket
{
    public delegate void SocketServerEvent(AsyncSocketUserToken userSocket, string msg);

    public class Server
    {
        public SocketServerEvent Receive;
        private Socket socketServer;
        private int port;

        private Semaphore maxNumberAcceptedClients;                             // 限制访问接收连接的线程数，用来控制最大并发数
        private AsyncSocketUserTokenList asyncSocketUserTokenList;
        private AsyncSocketUserTokenPool asyncSocketUserTokenPool;

        public Server() { }
        public Server(int _port)
        {
            port = _port;

            maxNumberAcceptedClients = new Semaphore(Config.PARALLE_NUM, Config.PARALLE_NUM);
            asyncSocketUserTokenList = new AsyncSocketUserTokenList();
            asyncSocketUserTokenPool = new AsyncSocketUserTokenPool(Config.PARALLE_NUM);

            // 按照连接数建立读写对象
            AsyncSocketUserToken userToken;
            for (int i = 0; i < Config.PARALLE_NUM; i++)
            {
                userToken = new AsyncSocketUserToken();
                userToken.ReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                userToken.SendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                asyncSocketUserTokenPool.Push(userToken);
            }
        }

        public void Start()
        {
            IPEndPoint listenPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port);
            socketServer = new Socket(listenPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socketServer.Bind(listenPoint);
            socketServer.Listen(Config.PARALLE_NUM);

            MyLog.Log("Start listener socket {0} success", listenPoint.ToString());
            StartAccept(null);
        }
        private void StartAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            if (acceptEventArgs == null)
            {
                acceptEventArgs = new SocketAsyncEventArgs();
                acceptEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                acceptEventArgs.AcceptSocket = null; //释放上次绑定的Socket，等待下一个Socket连接
            }

            maxNumberAcceptedClients.WaitOne(); //获取信号量
            bool willRaiseEvent = socketServer.AcceptAsync(acceptEventArgs);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArgs);
            }
        }
        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs acceptEventArgs)
        {
            try
            {
                ProcessAccept(acceptEventArgs);
            }
            catch (Exception e)
            {
                Tools.MyLog.Error("Accept client {0} error, \nmessage: {1} \nstackTrace: {2}", acceptEventArgs.AcceptSocket, e.Message, e.StackTrace);
            }
        }
        private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            AsyncSocketUserToken userToken = asyncSocketUserTokenPool.Pop();
            asyncSocketUserTokenList.Add(userToken); //添加到正在连接列表
            userToken.ConnectSocket = acceptEventArgs.AcceptSocket;
            userToken.ConnectDateTime = DateTime.Now;
            userToken.serverSocket = this;
            Tools.MyLog.Log("New client {0} accept, Client count {1}", acceptEventArgs.AcceptSocket.RemoteEndPoint, asyncSocketUserTokenList.Count);

            try
            {
                bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs); //投递接收请求
                if (!willRaiseEvent)
                {
                    lock (userToken)
                    {
                        ProcessReceive(userToken.ReceiveEventArgs);
                    }
                }
            }
            catch (Exception e)
            {
                Tools.MyLog.Error("Accept client {0} error, \nmessage: {1} \nstackTrace: {2}", userToken.ConnectSocket, e.Message, e.StackTrace);
            }

            StartAccept(acceptEventArgs); //把当前异步事件释放，等待下次连接
        }
        private void IO_Completed(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            AsyncSocketUserToken userToken = asyncEventArgs.UserToken as AsyncSocketUserToken;
            userToken.ActiveTime = DateTime.Now;
            try
            {
                lock (userToken)
                {
                    if (asyncEventArgs.LastOperation == SocketAsyncOperation.Receive)
                        ProcessReceive(asyncEventArgs);
                    else if (asyncEventArgs.LastOperation == SocketAsyncOperation.Send)
                        ProcessSend(asyncEventArgs);
                    else
                        throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                }
            }
            catch (Exception e)
            {
                Tools.MyLog.Error("IO_Completed {0} error, \nmessage: {1} \nstackTrace: {2}", userToken.ConnectSocket, e.Message, e.StackTrace);
            }
        }
        private void ProcessReceive(SocketAsyncEventArgs receiveEventArgs)
        {
            AsyncSocketUserToken userToken = receiveEventArgs.UserToken as AsyncSocketUserToken;
            if (userToken.ConnectSocket == null)
                return;
            userToken.ActiveTime = DateTime.Now;
            if (userToken.ReceiveEventArgs.BytesTransferred > 0 && userToken.ReceiveEventArgs.SocketError == SocketError.Success)
            {
                int offset = userToken.ReceiveEventArgs.Offset;
                int count = userToken.ReceiveEventArgs.BytesTransferred;

                if (count > 0) //处理接收数据
                {
                    //如果处理数据返回失败，则断开连接
                    if (!userToken.ProcessReceive(userToken.ReceiveEventArgs.Buffer, offset, count))
                    {
                        CloseClientSocket(userToken);
                    }
                    else //否则投递下次介绍数据请求
                    {
                        bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs); //投递接收请求
                        if (!willRaiseEvent)
                            ProcessReceive(userToken.ReceiveEventArgs);
                    }
                }
                else
                {
                    bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs); //投递接收请求
                    if (!willRaiseEvent)
                        ProcessReceive(userToken.ReceiveEventArgs);
                }
            }
            else
            {
                CloseClientSocket(userToken);
            }
        }
        private bool ProcessSend(SocketAsyncEventArgs sendEventArgs)
        {
            AsyncSocketUserToken userToken = sendEventArgs.UserToken as AsyncSocketUserToken;
            userToken.ActiveTime = DateTime.Now;
            if (sendEventArgs.SocketError == SocketError.Success)
                return userToken.SendCompleted(); //调用子类回调函数
            else
            {
                CloseClientSocket(userToken);
                return false;
            }
        }
        private void CloseClientSocket(AsyncSocketUserToken userToken)
        {
            if (userToken.ConnectSocket == null)
                return;

            string socketInfo = string.Format("Remote Address: {0}", userToken.ConnectSocket.RemoteEndPoint);
            socketInfo = string.Format("Client disconnected {0}.", socketInfo);

            try
            {
                userToken.ConnectSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception E)
            {
                Tools.MyLog.Error("CloseClientSocket Disconnect client {0} error, message: {1}", socketInfo, E.Message);
            }

            userToken.ConnectSocket.Close();
            userToken.ConnectSocket = null; //释放引用，并清理缓存，包括释放协议对象等资源

            maxNumberAcceptedClients.Release();
            asyncSocketUserTokenPool.Push(userToken);
            asyncSocketUserTokenList.Remove(userToken);

            MyLog.Log("{0} Client count {1}", socketInfo, asyncSocketUserTokenList.Count);
        }


        public int ClientCount
        {
            get { return asyncSocketUserTokenList.Count; }
        }
        public AsyncSocketUserTokenList ClientList
        {
            get { return asyncSocketUserTokenList; }
        }
    }
}
