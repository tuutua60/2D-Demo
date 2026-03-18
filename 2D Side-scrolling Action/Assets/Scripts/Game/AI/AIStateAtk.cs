using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻击AI状态的处理类
/// </summary>
public class AIStateAtk : AIStateBase
{
    //第一次进行攻击前的等待时间
    private float firstAtkWaitTime;
    //每一次攻击后的间隔
    private float atkOffsetMin;
    private float atkOffsetMax;

    //攻击范围内的Y范围
    private float atkRangeY;
    //攻击范围内的X范围
    private float atkRangeX;

    private bool canAtk = false;
    public AIStateAtk(AILogic logic) : base(logic)
    {
        firstAtkWaitTime = aiLogic.aiInfo.f_atk_firstAtkWaitTime;
        string[] atkOffset = aiLogic.aiInfo.f_atk_atkOffset.Split(",");
        atkOffsetMin = float.Parse(atkOffset[0]);
        atkOffsetMax = float.Parse(atkOffset[1]);
        string[] atkRange = aiLogic.aiInfo.f_atk_atkRange.Split(",");
        atkRangeX = float.Parse(atkRange[0]);
        atkRangeY = float.Parse(atkRange[1]);
    }
    public override void EnterAIState()
    {
        Debug.Log("攻击状态");
        canAtk = false;
        MonoMgr.Instance.StartCoroutine(FirstAtkWaitTime());
    }

    public override void ExitAIState()
    {

    }

    public override void UpdateAIState()
    {
        if (canAtk)
        {
            aiLogic.enemy.Punch();
            canAtk = false;
            
            MonoMgr.Instance.StartCoroutine(AtkOffsetTime());
        }

        //不停检测玩家是否在可攻击范围内 如果不在 就切换到移动状态
        if (Mathf.Abs(aiLogic.enemy.transform.position.y - PlayerObject.player.transform.position.y) > atkRangeY ||
            (aiLogic.enemy.BodyisRight&&
            (PlayerObject.player.transform.position.x - aiLogic.enemy.transform.position.x > atkRangeX ||
             PlayerObject.player.transform.position.x < aiLogic.enemy.transform.position.x))||
             (!aiLogic.enemy.BodyisRight &&
             (aiLogic.enemy.transform.position.x - PlayerObject.player.transform.position.x > atkRangeX ||
             PlayerObject.player.transform.position.x > aiLogic.enemy.transform.position.x)))
        {
            aiLogic.ChangeAIState(E_AI_State.Move);
        }
    }

    /// <summary>
    /// 第一次攻击的计时
    /// </summary>
    /// <returns></returns>
    private IEnumerator FirstAtkWaitTime()
    {
        yield return new WaitForSeconds(firstAtkWaitTime);
        canAtk = true;
    }

    /// <summary>
    /// 每次攻击间隔计时
    /// </summary>
    /// <returns></returns>
    public IEnumerator AtkOffsetTime()
    {
        yield return new WaitForSeconds(Random.Range(atkOffsetMin,atkOffsetMax));
        canAtk=true;
    }
}
