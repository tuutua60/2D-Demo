using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI状态处理基类
/// </summary>
public abstract class AIStateBase
{
    //记录AI逻辑对象
    protected AILogic aiLogic;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logic"></param>
    public AIStateBase(AILogic logic) 
    {
        this.aiLogic = logic;
    }
    /// <summary>
    /// 进入AI状态时 处理什么逻辑
    /// </summary>
    public abstract void EnterAIState();

    /// <summary>
    /// 离开AI状态时 处理什么逻辑
    /// </summary>
    public abstract void ExitAIState();

    /// <summary>
    /// 更新AI状态
    /// </summary>
    public abstract void UpdateAIState();
}
