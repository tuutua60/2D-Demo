using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移动AI状态的处理类
/// </summary>
public class AIStateMove : AIStateBase
{
    private Vector3 targetPos;
    //第一次移动前摇
    private float firstMoveWaitTime;
    //是否在等待移动
    private bool isWait = true;
    //允许的最大活动距离 超过这个距离就会脱战
    private float maxDis;
    //状态下的移动速度
    private float speed;
    public AIStateMove(AILogic logic) : base(logic)
    {
        firstMoveWaitTime = aiLogic.aiInfo.f_move_firstMoveWaitTime;
        maxDis = aiLogic.aiInfo.f_move_maxDis;
        speed = aiLogic.aiInfo.f_move_speed;
    }
    public override void EnterAIState()
    {
        Debug.Log("朝向移动状态");
        aiLogic.enemy.moveSpeed = speed;
        isWait = true;
        MonoMgr.Instance.StartCoroutine(FirstMoveWaitTime());
    }

    public override void ExitAIState()
    {

    }

    public override void UpdateAIState()
    {
        //判断敌人和出生点的距离 超过了那就需要进入脱战状态
        if (Vector2.Distance(aiLogic.enemy.transform.position, aiLogic.enemy.bornPos) > maxDis)
        {
            aiLogic.enemy.Move(Vector2.zero);
            aiLogic.ChangeAIState(E_AI_State.Back);
            return;
        }

            if (isWait)
            return;
        //敌人要移动到玩家面前
        //计算目标点
        if(aiLogic.enemy.transform.position.x >= PlayerObject.player.transform.position.x)
        {
            targetPos = PlayerObject.player.transform.position + Vector3.right * 1.5f;
        }
        else
        {
            targetPos = PlayerObject.player.transform.position - Vector3.right * 1.5f;
        }
        //移动
        aiLogic.enemy.Move(targetPos-aiLogic.enemy.transform.position);
    
        //检测是否进入脱战

        //检测是否进入攻击
        if(Vector2.Distance(aiLogic.enemy.transform.position,targetPos) <= 0.05f)
        {
            aiLogic.enemy.Move(Vector2.zero);
            aiLogic.ChangeAIState(E_AI_State.Atk);
        }
    }

    private IEnumerator FirstMoveWaitTime()
    {
        yield return new WaitForSeconds(firstMoveWaitTime);
        isWait = false;
    }
}
