using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyObject : RoleObject
{
    //专门用于处理AI逻辑的对象
    private AILogic aiLogic;
    //敌人出生点
    public Vector3 bornPos;
    protected override void Awake()
    {
        base.Awake();

        roleType = E_Role_Type.Enemy;
        eventCheck.checkDead += CheckDead;
    }

    protected override void Update()
    {
        base.Update();
        aiLogic.UpdateAI();
    }
    /// <summary>
    /// 初始化敌人属性
    /// </summary>
    /// <param name="id"></param>
    public override void InitProperty(int id)
    {
        property = new EnemyProperty(id);
        isDead = false;
        aiLogic = new AILogic(this, 1);
        //记录敌人刚出现的位置
        bornPos = this.transform.position;
    }
    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="dir">移动方向</param>
    public void Move(Vector2 dir)
    {
        //改变移动方向 就会在基类中的 Update逻辑中 去处理移动相关逻辑
        moveDir = dir;
    }
    /// <summary>
    /// 攻击
    /// </summary>
    public override void Punch()
    {
        //切换攻击动画即可 不需要考虑其他互斥
        ChangeAction(E_Action_Type.Punch);
    }
    /// <summary>
    /// 死亡
    /// </summary>
    public override void Dead()
    {
        isDead = true;
        MusicMgr.Instance.PlaySound("dead");
        ChangeAction(E_Action_Type.Dead);
    }
    private void CheckDead()
    {
        PoolMgr.Instance.PushObject(this.gameObject.name, this.gameObject);
    }

    protected override void CheckBodyDir()
    {
        if(isDead)
            return;
        if(aiLogic.nowAIState == E_AI_State.Patrol||
           aiLogic.nowAIState == E_AI_State.Back)
        {
            base.CheckBodyDir();

        }
        else
        {
            if (PlayerObject.player != null)
            {
                if (this.transform.position.x - PlayerObject.player.transform.position.x > 0)
                {
                    roleSprite.flipX = true;
                }
                if (this.transform.position.x - PlayerObject.player.transform.position.x < 0)
                {
                    roleSprite.flipX = false;
                }
            }
        }
    }

    public override void Hit(float hitTime)
    {
        //如果受伤时 是击飞状态 那就没必要执行受伤逻辑了
        if (roleAnimator.GetBool("isKnockDown"))
        {
            return;
        }
        //如果处于受伤状态 又受伤 那么需要把上一次的延时函数取消掉
        CancelInvoke("DelayClearHit");
        //切换受伤动作
        ChangeAction(E_Action_Type.Hit);

        //处理对应的随机受伤状态
        roleAnimator.SetInteger("hitState",Random.Range(1,4));

        //延时函数来处理过一段事件结束受伤状态
        Invoke("DelayClearHit", hitTime);
    }
    private void DelayClearHit()
    {
        roleAnimator.SetBool("isHit", false);
        roleAnimator.SetInteger("hitState", 0);
    }
}
