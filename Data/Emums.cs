using System;
public class Emums
{
    
}

public enum CMD
{
    None = 0,
    Heartbeat = 1,
    Login = 1000,                   // 登陆
    Register = 1001,               // 注册
    EnterGame = 1002,           // 进入游戏
    GetRole = 1003,                // 获取角色信息
    CreateRole = 1004,            // 创建角色
    SelectRole = 1005,             // 选择角色
    EnterBattleScene = 1006,   // 开始战斗
    EnterNewRole = 1007,             // 加入战斗
    Attack = 1008,                    // 攻击
}
