using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MySocket
{
    public class BufferBase
    {
        public byte[] Buffer { get; set; } //存放内存的数组
        public int DataCount //写入数据大小
        {
            get { return dataCount; }
            set { dataCount = value; }
        } 
        private int dataCount;
        public int GetReserveCount() //获得剩余的字节数
        {
            return Buffer.Length - DataCount;
        }

        public BufferBase(int bufferSize)
        {
            dataCount = 0;
            Buffer = new byte[bufferSize];
        }

        public void SetBufferSize(int size) //设置缓存大小
        {
            if (Buffer.Length < size)
            {
                byte[] tmpBuffer = new byte[size];
                Array.Copy(Buffer, 0, tmpBuffer, 0, dataCount); //复制以前的数据
                Buffer = tmpBuffer; //替换
            }
        }

        public void Clear()
        {
            dataCount = 0;
        }
        public void Clear(int count) //清理指定大小的数据
        {
            if (count >= dataCount) //如果需要清理的数据大于现有数据大小，则全部清理
            {
                dataCount = 0;
            }
            else
            {
                for (int i = 0; i < dataCount - count; i++) //否则后面的数据往前移
                {
                    Buffer[i] = Buffer[count + i];
                }
                dataCount = dataCount - count;
            }
        }

        public void WriteBuffer(byte[] buffer, int offset, int count)
        {
            if (GetReserveCount() >= count) //缓冲区空间够，不需要申请
            {
                Array.Copy(buffer, offset, Buffer, dataCount, count);
                dataCount = dataCount + count;
            }
            else //缓冲区空间不够，需要申请更大的内存，并进行移位
            {
                int totalSize = Buffer.Length + count - GetReserveCount(); //总大小-空余大小
                byte[] tmpBuffer = new byte[totalSize];
                Array.Copy(Buffer, 0, tmpBuffer, 0, dataCount); //复制以前的数据
                Array.Copy(buffer, offset, tmpBuffer, dataCount, count); //复制新写入的数据
                dataCount = dataCount + count;
                Buffer = tmpBuffer; //替换
            }
        }
        private void WriteBuffer(byte[] buffer)
        {
            WriteBuffer(buffer, 0, buffer.Length);
        }

        public void Write(string msg)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            WriteBuffer(BitConverter.GetBytes(buffer.Length));
            WriteBuffer(buffer);
        }
    }
}
