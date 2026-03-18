using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 拓展AI状态的处理类
/// </summary>
public class AIStateBack : AIStateBase
{
    private float speed;
    public AIStateBack(AILogic logic) : base(logic)
    {
        speed = aiLogic.aiInfo.f_back_speed;
    }
    public override void EnterAIState()
    {
        Debug.Log("脱战状态");
        aiLogic.enemy.moveSpeed = speed;
    }

    public override void ExitAIState()
    {
        
    }

    public override void UpdateAIState()
    {
        //不停回到出生点
        //回到出生点后 应该切换到巡逻状态
        aiLogic.enemy.Move(aiLogic.enemy.bornPos-aiLogic.enemy.transform.position);

        if (Vector2.Distance(aiLogic.enemy.transform.position, aiLogic.enemy.bornPos) <= 0.1f)
        {
            aiLogic.enemy.Move(Vector2.zero);
            aiLogic.ChangeAIState(E_AI_State.Patrol);
        }
    }
}
