using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public enum E_Role_Type
{
    Enemy,
    Player,
}
public enum E_Action_Type
{
    /// <summary>
    /// 待机
    /// </summary>
    Idle,
    /// <summary>
    /// 走路
    /// </summary>
    Walk,
    /// <summary>
    /// 跳跃
    /// </summary>
    Jump,
    /// <summary>
    /// 跳跃攻击
    /// </summary>
    JumpAtk,
    /// <summary>
    /// 攻击
    /// </summary>
    Punch,
    /// <summary>
    /// 踢
    /// </summary>
    Kick,
    /// <summary>
    /// 受伤
    /// </summary>
    Hit,
    /// <summary>
    /// 击倒
    /// </summary>
    KnockDown,
    /// <summary>
    /// 投掷
    /// </summary>
    Throw,
    /// <summary>
    /// 拾取
    /// </summary>
    PickUp,
    /// <summary>
    /// 格挡
    /// </summary>
    Defend,
    /// <summary>
    /// 死亡
    /// </summary>
    Dead,
}

/// <summary>
/// 角色对象基类 之后的怪物 玩家 都继承
/// 因为他们都有共同点
/// </summary>
public abstract class RoleObject : MonoBehaviour
{
    public E_Role_Type roleType;
    //角色的移动方向
    protected Vector2 moveDir = Vector2.zero;
    //角色的移动速度
    public float moveSpeed = 3;
    //角色的Sprite
    protected SpriteRenderer roleSprite;
    //角色的Animator
    protected Animator roleAnimator;
    //身体的对象
    protected Transform bodyTransform;
    //身体是否在地面
    private bool isGround = true;
    private Transform shadowTransform;
    protected bool GetIsGround => isGround;
    //角色是否死亡
    public bool isDead = false;
    //当前Y速度
    protected float nowYSpeed;
    protected float nowXSpeed;
    //当前X速度
    //重力加速度
    public float gSpeed = 30;

    protected EventCheck eventCheck;

    //对象的属性类 里面有所有属性相关的内容
    public BaseProperty property;

    //移动之前上一次所在的位置 用来处理边界判断
    protected Vector3 frontPos;
    protected virtual void Awake()
    {
        bodyTransform = this.transform.Find("Role");
        shadowTransform = this.transform.Find("Shadow");
        roleSprite = bodyTransform.GetComponent<SpriteRenderer>();
        roleAnimator = this.GetComponentInChildren<Animator>();

        //可以得到子对象伤害检测事件监听的脚本 然后来进行处理
        eventCheck = this.GetComponentInChildren<EventCheck>();
        eventCheck.checkDamage += CheckDamage;
        
    }
    protected virtual void Update()
    {
        //检测移动 相关位移
        CheckMove();
        CheckBodyDir();
        //检测跳跃或者击飞 相关位移
        CheckJumpOrKnockDown();
    }

    /// <summary>
    /// 初始化属性信息
    /// </summary>
    /// <param name="id"></param>
    public abstract void InitProperty(int id);


    /// <summary>
    /// 获取对象朝向
    /// </summary>
    public bool BodyisRight
    {
        get
        {
            return !roleSprite.flipX;
        }
    }

    /// <summary>
    /// 切换角色动作
    /// </summary>
    /// <param name="type">要切换的动作枚举</param>
    protected void ChangeAction(E_Action_Type type)
    {
        switch (type)
        {
            case E_Action_Type.Idle:
                roleAnimator.SetBool("isMoving", false);
                break;
            case E_Action_Type.Walk:
                roleAnimator.SetBool("isMoving",true);
                break;
            case E_Action_Type.Jump:
                roleAnimator.SetTrigger("jumpTrigger");
                break;
            case E_Action_Type.JumpAtk:
                roleAnimator.SetTrigger("jumpAtkTrigger");
                break;
            case E_Action_Type.Punch:
                roleAnimator.SetTrigger("punchTrigger");
                break;
            case E_Action_Type.Kick:
                roleAnimator.SetTrigger("kickTrigger");
                break;
            case E_Action_Type.Hit:
                roleAnimator.SetBool("isHit",true);
                break;
            case E_Action_Type.KnockDown:
                roleAnimator.SetBool("isKnockDown", true);
                break;
            case E_Action_Type.Throw:
                roleAnimator.SetTrigger("throwTrigger");
                break;
            case E_Action_Type.PickUp:
                roleAnimator.SetTrigger("pickUpTrigger");
                break;
            case E_Action_Type.Defend:
                //略
                break;
            case E_Action_Type.Dead:
                roleAnimator.SetBool("isDead",true);
                break;
        }
    }
    /// <summary>
    /// 切换角色是否在地面
    /// </summary>
    /// <param name="isGround"></param>
    protected void ChangeRoleIsGround(bool isGround)
    {
        roleAnimator.SetBool("isGround",isGround);
        this.isGround = isGround;
    }
    protected bool CanMoving
    {
        get
        {
            //去得到状态机中两层的状态
            //AnimatorStateInfo layerInfo1 = roleAnimator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo layerInfo2 = roleAnimator.GetCurrentAnimatorStateInfo(1);
            if (IsAtkState                  ||
                layerInfo2.IsName("Defend") ||
                roleAnimator.GetBool("isHit")    ||
                layerInfo2.IsName("KnockDown")||
                layerInfo2.IsName("PickUp")||
                layerInfo2.IsName("Throw")||
                roleAnimator.GetBool("isDead")||
                layerInfo2.IsName("StandUp"))
            {
                return false;
            }
            //默认能移动
            return true;
        }
    }

    protected bool IsAtkState
    {
        get
        {
            AnimatorStateInfo layerInfo2 = roleAnimator.GetCurrentAnimatorStateInfo(1);
            if (layerInfo2.IsName("Punch")  ||
                layerInfo2.IsName("Punch1") ||
                layerInfo2.IsName("Punch2") ||
                layerInfo2.IsName("Punch3") ||
                layerInfo2.IsName("Kick1")  ||
                layerInfo2.IsName("Kick2"))
            {
                return true;
            }
            return false;
        }
    }


    /// <summary>
    /// 受伤方法
    /// </summary>
    /// <param name="hitTime">僵直时间</param>
    public virtual void Hit(float hitTime)
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
        //延时函数来处理过一段事件结束受伤状态
        Invoke("DelayClearHit", hitTime);
    }
    private void DelayClearHit()
    {
        roleAnimator.SetBool("isHit", false);
    }

    /// <summary>
    /// 击飞方法
    /// </summary>
    /// <param name="xSpeed">x速度(正数代表向右 负数代表向左)</param>
    /// <param name="ySpeed">y速度</param>
    public virtual void KnockDown(float xSpeed, float ySpeed)
    {
        //如果当前是受伤状态 击飞的优先级更高 我们需要改变它的状态
        if (roleAnimator.GetBool("isHit"))
        {
            roleAnimator.SetBool("isHit", false);
            //取消延迟函数
            CancelInvoke("DelayClearHit");
        }
        //如果已经是击飞状态 那就不用再击飞
        if (roleAnimator.GetBool("isKnockDown"))
        {
            return;
        }
        //击飞速度赋值
        nowYSpeed = ySpeed;
        nowXSpeed = xSpeed;
        //切换击飞动作
        ChangeAction(E_Action_Type.KnockDown);
        //改变玩家不在地面
        ChangeRoleIsGround(false);
    }
    private void DelayClearKnockDown()
    {
        roleAnimator.SetBool("isKnockDown", false);
    }
    /// <summary>
    /// 手部攻击
    /// </summary>
    public abstract void Punch();
    /// <summary>
    /// 死亡
    /// </summary>
    public abstract void Dead();


    private Vector3 tmpPos;
    /// <summary>
    /// 检测移动 位移相关逻辑
    /// </summary>
    protected virtual void CheckMove()
    {
        if (CanMoving)
        {
            //移动之前记录当前位置
            frontPos = this.transform.position;
            //在移动前加判断 满足移动条件 才去移动
            //角色移动逻辑
            this.transform.Translate(Vector3.Normalize(moveDir) * moveSpeed * Time.deltaTime);
            tmpPos = this.transform.position;
            tmpPos.z = tmpPos.y;
            this.transform.position = tmpPos;
            if (moveDir.x < 0)
            {
                shadowTransform.transform.localPosition = Vector3.right*0.04f + Vector3.up*-1.2f;
            }
            else if (moveDir.x > 0)
            {
                shadowTransform.transform.localPosition = Vector3.right * -0.04f + Vector3.up * -1.2f;
            }
            //是否移动
            ChangeAction(moveDir == Vector2.zero ? E_Action_Type.Idle : E_Action_Type.Walk);
        }
    }

    protected virtual void CheckBodyDir()
    {
        //控制角色转向
        if (moveDir.x < 0)
        {
            roleSprite.flipX = true;
        }
        else if (moveDir.x > 0)
        {
            roleSprite.flipX = false;
        }
    }

    /// <summary>
    /// 检测处理跳跃和击飞 的位移相关的逻辑
    /// </summary>
    protected void CheckJumpOrKnockDown()
    {
        //处理 跳跃逻辑
        //不是在地面上 那就得跳跃
        if (!GetIsGround)
        {
            //跳跃身体对象
            bodyTransform.Translate(Vector2.up * nowYSpeed * Time.deltaTime);
            //模拟重力 速度变化
            nowYSpeed -= gSpeed * Time.deltaTime;

            //判断身体的y是否<=0 即可判断是否落地
            if (bodyTransform.localPosition.y <= 0)
            {
                //放置到地面
                bodyTransform.localPosition = Vector2.zero;
                //改变地面标识
                ChangeRoleIsGround(true);

                //在落地时动态创建灰尘特效
                PoolMgr.Instance.GetObject("Prefabs/Effect/DownEff", (eff) =>
                {
                    //异步创建特效成功后 做的事情
                    //根据面朝向 决定灰尘是左飘还是右飘
                    if (!BodyisRight)
                        eff.transform.rotation = Quaternion.Euler(0, 180, 0);
                    else
                        eff.transform.rotation = Quaternion.Euler(0, 0, 0);
                    eff.transform.position = this.transform.position - Vector3.up * 1.25f - Vector3.forward;
                    //让粒子重新播放一次
                    ParticleSystem effSystem = eff.transform.Find("Eff").GetComponent<ParticleSystem>();
                    effSystem.Play(); 
                });
            }
        }
        if (nowXSpeed != 0)
        {
            //移动之前记录当前位置
            frontPos = this.transform.position;
            this.transform.Translate(nowXSpeed * Vector2.right * Time.deltaTime);
            if (GetIsGround)
            {
                nowXSpeed = 0;
                Invoke("DelayClearKnockDown", 0.45f);
            }
        }
    }

    /// <summary>
    /// 当动作播放 触发伤害检测时响应的函数
    /// </summary>
    /// <param name="id"></param>
    protected void CheckDamage(int id)
    {
        //不同的攻击动作
        //1.伤害判断范围不同
        //2.可带给目标带来的伤害表现不同
        if (!BinaryDataMgr.Instance.GetTable<T_DamageInfoContainer>().dataDic.ContainsKey(id))
        {
            print("对应ID伤害表信息没有找到" + id);
            return;
        }
        //获取伤害表中对应ID的数据
        T_DamageInfo dmgInfo = BinaryDataMgr.Instance.GetTable<T_DamageInfoContainer>().dataDic[id];
        //1.伤害范围检测
        //获取矩形范围判断的 偏移和宽高
        string[] strs = dmgInfo.f_check_range.Split(",");
        float offsetX = float.Parse(strs[0]);
        float offsetY = float.Parse(strs[1]);
        float rectWidth = float.Parse(strs[2]);
        float rectHeight = float.Parse(strs[3]);
        Vector2 center = new Vector2(roleSprite.transform.position.x + offsetX, roleSprite.transform.position.y + offsetY);
        if (!BodyisRight)
        {
            center = new Vector2(roleSprite.transform.position.x - offsetX, roleSprite.transform.position.y + offsetY);
        }
        Vector2 size = new Vector2(rectWidth, rectHeight);


        Collider2D[] colliders = Physics2D.OverlapBoxAll(center,size,0);

        DrawOverlapBox(center, size);
        DrawCenter(center);

        for (int i = 0; i < colliders.Length; i++)
        {

            //如果该碰撞体没有父对象 直接返回
            if (colliders[i].transform.parent == null)
                continue;
            //如果打到物品
            if (colliders[i].CompareTag("ThingObject"))
            {
                ThingObject thingObject = colliders[i].transform.parent.GetComponent<ThingObject>();
                if (thingObject.IsDead)
                    continue;
                if (BodyisRight)
                    thingObject.transform.rotation = Quaternion.Euler(thingObject.transform.rotation.x,0, thingObject.transform.rotation.z);
                else
                    thingObject.transform.rotation = Quaternion.Euler(thingObject.transform.rotation.x, 180, thingObject.transform.rotation.z);
                thingObject.Wound();
                PoolMgr.Instance.GetObject("Prefabs/Effect/HitEff", (eff) => {
                    //异步创建特效成功后 做的事情
                    eff.transform.position = thingObject.transform.position + Vector3.up*0.5f;

                    //让粒子重新播放一次
                    ParticleSystem effSystem = eff.transform.Find("Eff").GetComponent<ParticleSystem>();
                    effSystem.Play();
                    MusicMgr.Instance.PlaySound("hit1");
                });
                print("物品受伤");
                continue;
            }
            //被攻击者
            RoleObject roleObj;
            roleObj = colliders[i].transform.parent.GetComponent<RoleObject>();
            //如果父对象没有角色脚本 直接返回
            if(roleObj==null)
                continue;
            //如果打到的对象已经死亡 直接返回
            if (roleObj.isDead)
                continue;
            //如果打到的对象是同一阵营 直接返回
            if (roleObj.roleType == this.roleType)
                continue;
            //找到受伤对象后 还应该判断伪z轴误差
            if (Mathf.Abs(this.transform.position.y - roleObj.transform.position.y) > dmgInfo.f_check_zRange)
                continue;

            //判断玩家是否格挡
            if(roleObj.roleType == E_Role_Type.Player && roleObj.roleAnimator.GetBool("isDefend") &&
                roleObj.BodyisRight != this.BodyisRight)
            {
                //格挡
                //生成格挡特效
                PoolMgr.Instance.GetObject("Prefabs/Effect/DefendEff", (eff) => {
                    //异步创建特效成功后 做的事情
                    //根据面朝向 决定特效是左飘还是右飘
                    if (!roleObj.BodyisRight)
                    {
                        eff.transform.rotation = Quaternion.Euler(0, 180, 0);
                        eff.transform.position = roleObj.transform.position + new Vector3(-0.3f, 0.8f, 0) - Vector3.forward;
                    }

                    else
                    {
                        eff.transform.rotation = Quaternion.Euler(0, 0, 0);
                        eff.transform.position = roleObj.transform.position + new Vector3( 0.3f, 0.8f, 0) - Vector3.forward;
                    }

                    //让粒子重新播放一次
                    ParticleSystem effSystem = eff.transform.Find("Eff").GetComponent<ParticleSystem>();
                    effSystem.Play();
                    MusicMgr.Instance.PlaySound("defend");
                });
                continue;
            }

            //受伤和伤害相关的处理
            if(dmgInfo.f_hitTime != 0)
            {//受伤处理
                roleObj.Hit(dmgInfo.f_hitTime);
            }
            else
            {//击飞处理
                strs = dmgInfo.f_knockDownSpeed.Split(",");
                if (BodyisRight)
                {
                    roleObj.KnockDown(float.Parse(strs[0]), float.Parse(strs[1]));
                }
                else
                {
                    roleObj.KnockDown(-float.Parse(strs[0]), float.Parse(strs[1]));
                }
            }
            //播放受伤的打击特效
            PoolMgr.Instance.GetObject("Prefabs/Effect/HitEff",(eff)=>{
                //异步创建特效成功后 做的事情
                //根据面朝向 决定特效是左飘还是右飘
                if (!roleObj.BodyisRight)
                {
                    eff.transform.rotation = Quaternion.Euler(0, 180, 0);
                    eff.transform.position = roleObj.transform.position + new Vector3(0.1f, 1f, 0) - Vector3.forward;
                }

                else
                {
                    eff.transform.rotation = Quaternion.Euler(0, 0, 0);
                    eff.transform.position = roleObj.transform.position + new Vector3(-0.1f, 1f, 0) - Vector3.forward;
                }

                //让粒子重新播放一次
                ParticleSystem effSystem = eff.transform.Find("Eff").GetComponent<ParticleSystem>();
                effSystem.Play();
                MusicMgr.Instance.PlaySound("hit1");
            });
            //受伤数值处理
            //基础伤害
            float dmg = this.property.atk * dmgInfo.f_factor - roleObj.property.def;
            if(dmg <= 1)
            {
                //当基础伤害小于1时 最少伤害1
                dmg = 1;
            }

            //减血
            roleObj.property.nowHp -= dmg;
            //更新血条
            if (roleObj.roleType == E_Role_Type.Player)
                UIManager.Instance.GetPanel<GamePanel>().UpdatePlayerHp(roleObj.property.nowHp,roleObj.property.maxHp);
            else
                UIManager.Instance.GetPanel<GamePanel>().UpdateEnemyHp(roleObj.property.nowHp, roleObj.property.maxHp);
            //判断死亡
            if(roleObj.property.nowHp <= 0)
            {
                roleObj.property.nowHp = 0;
                roleObj.Dead();
            }
        }
    }

    //测试范围检测画线
    void DrawOverlapBox(Vector2 center, Vector2 size)
    {
        Vector2 halfSize = size * 0.5f;

        Vector2 topLeft = center + new Vector2(-halfSize.x, halfSize.y);
        Vector2 topRight = center + new Vector2(halfSize.x, halfSize.y);
        Vector2 bottomLeft = center + new Vector2(-halfSize.x, -halfSize.y);
        Vector2 bottomRight = center + new Vector2(halfSize.x, -halfSize.y);

        Debug.DrawLine(topLeft, topRight, UnityEngine.Color.green,1f);
        Debug.DrawLine(topRight, bottomRight, UnityEngine.Color.green,1f);
        Debug.DrawLine(bottomRight, bottomLeft, UnityEngine.Color.green,1f);
        Debug.DrawLine(bottomLeft, topLeft, UnityEngine.Color.green,1f);
    }
    void DrawCenter(Vector2 center, float size = 0.1f)
    {
        // 横线
        Debug.DrawLine(center + Vector2.left * size, center + Vector2.right * size, UnityEngine.Color.red,1f);
        // 竖线
        Debug.DrawLine(center + Vector2.up * size, center + Vector2.down * size, UnityEngine.Color.red, 1f);
    }
}
