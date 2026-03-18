using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 巡逻AI状态的处理类
/// </summary>
public class AIStatePatrol : AIStateBase
{
    //巡逻范围
    private float rangeWidth;
    private float rangeHeight;
    //目标点
    private Vector3 targetPos;
    //是否到达目标点
    private bool isArrive = true;
    //是否在目标点等待
    private bool isWait = false;
    //每次在目标点等待的时间
    private float waitTime;

    //攻击范围
    private float atkRange;
    //状态下的移动速度
    private float speed;
    public AIStatePatrol(AILogic logic) : base(logic)
    {
        string[] range = aiLogic.aiInfo.f_patrol_range.Split(",");
        rangeWidth = float.Parse(range[0]);
        rangeHeight = float.Parse(range[1]);

        waitTime = aiLogic.aiInfo.f_patrol_waitTime;
        atkRange = aiLogic.aiInfo.f_patrol_atkRange;
        speed = aiLogic.aiInfo.f_patrol_speed;
    }
    public override void EnterAIState()
    {
        Debug.Log("巡逻状态");
        //每次进入状态时 会执行的函数
        aiLogic.enemy.moveSpeed = speed;
    }

    public override void ExitAIState()
    {
        //每次退出状态时 会执行的函数
    }

    public override void UpdateAIState()
    {
        //如果不在等待
        if (!isWait)
        {
            //处理巡逻逻辑
            //已经到达目标点的话 就重新获取目标点
            if (isArrive)
            {
                GetTargetPos();
            }
            //如果没有到达 就一直朝目标点移动
            else
            {
                //不停告诉怪物应该朝哪个方向移动即可
                aiLogic.enemy.Move(targetPos - aiLogic.enemy.transform.position);
                if (Vector2.Distance(aiLogic.enemy.transform.position, targetPos) < 0.1f)
                {
                    //让玩家停下来
                    aiLogic.enemy.Move(Vector2.zero);
                    isArrive = true;
                    isWait = true;
                    //利用小框架中的Mono管理器 处理没有继承Mono的协程
                    MonoMgr.Instance.StartCoroutine(WaitMove());
                }
            }
        }

        //在巡逻状态时 应该不停判断和玩家之间的距离 如果玩家进入了攻击范围
        if (Vector3.Distance(aiLogic.enemy.transform.position, PlayerObject.player.transform.position) <= atkRange)
        {
            aiLogic.enemy.Move(Vector2.zero);
            //切换状态
            aiLogic.ChangeAIState(E_AI_State.Move);
        }
    }

    private IEnumerator WaitMove()
    {
        yield return new WaitForSeconds(waitTime);
        isWait = false;
    }

    /// <summary>
    /// 获取随机目标点
    /// </summary>
    private void GetTargetPos()
    {
        isArrive = false;
        Vector2 oldTargetPos = targetPos;
        targetPos.x = Random.Range(aiLogic.enemy.bornPos.x - rangeWidth, aiLogic.enemy.bornPos.x + rangeWidth);
        targetPos.y = Random.Range(aiLogic.enemy.bornPos.y - rangeHeight, aiLogic.enemy.bornPos.y + rangeHeight);
        if (Vector2.Distance(oldTargetPos, targetPos) < 3f)
        {
            GetTargetPos();
        }
    }
}
