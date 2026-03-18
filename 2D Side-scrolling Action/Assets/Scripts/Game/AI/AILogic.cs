using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI 状态
/// </summary>
public enum E_AI_State
{
    Null,
    /// <summary>
    /// 巡逻状态
    /// </summary>
    Patrol,
    /// <summary>
    /// 移动状态
    /// </summary>
    Move,
    /// <summary>
    /// 攻击状态
    /// </summary>
    Atk,
    /// <summary>
    /// 脱战状态
    /// </summary>
    Back,
}

/// <summary>
/// 专门用于处理AI逻辑的类
/// </summary>
public class AILogic
{
    //AI逻辑控制的怪物对象
    public EnemyObject enemy;
    //用一个字典记录AI逻辑中的所有AI状态
    private Dictionary<E_AI_State, AIStateBase> stateDic = new Dictionary<E_AI_State, AIStateBase>();
    //当前的AI状态
    public E_AI_State nowAIState = E_AI_State.Null;
    private AIStateBase nowState;

    //AI数据对象
    public T_AI_Info aiInfo;
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="enemy">控制的怪物对象是谁</param>
    public AILogic(EnemyObject enemy,int id)
    {
        this.enemy = enemy;

        //读取AI表中的 对应ID的数据
        aiInfo = BinaryDataMgr.Instance.GetTable<T_AI_InfoContainer>().dataDic[id];

        stateDic.Add(E_AI_State.Patrol,new AIStatePatrol(this));
        stateDic.Add(E_AI_State.Move,new AIStateMove(this));
        stateDic.Add(E_AI_State.Atk,new AIStateAtk(this));
        stateDic.Add(E_AI_State.Back,new AIStateBack(this));

        ChangeAIState(E_AI_State.Patrol);
    }

    /// <summary>
    /// 更新AI
    /// </summary>
    public void UpdateAI()
    {
        nowState.UpdateAIState();
    }
    /// <summary>
    /// 切换AI状态
    /// </summary>
    /// <param name="state"></param>
    public void ChangeAIState(E_AI_State state)
    {
        //1.在记录当前状态之前 应该执行上一个状态的退出方法
        if(nowAIState != E_AI_State.Null)
            nowState.ExitAIState();
        //记录要切换到的状态
        this.nowAIState = state;
        nowState = stateDic[nowAIState];
        //2.进入新状态 应该执行新状态的进入方法
        nowState.EnterAIState();
    }
}
