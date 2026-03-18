using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : RoleObject
{
    //静态玩家对象
    public static PlayerObject player;

    //玩家跳跃初速度
    public float jumpSpeed = 9;
    protected override void Awake()
    {
        base.Awake();
        player = this;
        //开启输入控制
        InputMgr.Instance.StartOrEndCheck(true);
        //获取输入权限
        AddController();
        roleType = E_Role_Type.Player;

        //测试
        InitProperty(1);
    }
    /// <summary>
    /// 初始化玩家属性
    /// </summary>
    /// <param name="id"></param>
    public override void InitProperty(int id)
    {
        property = new PlayerProperty(id);
    }
    /// <summary>
    /// 移动方法重写
    /// </summary>
    protected override void CheckMove()
    {
        base.CheckMove();
        //移动的边界判断
        //记录当前的位置
        Vector3 nowPos = this.transform.position;
        //用当前的位置 来和边界进行判断
        if (LevelMgr.Instance.CheckOutSectionRectX(this.transform.position.x))
        {
            nowPos.x = frontPos.x;
        }
        if (LevelMgr.Instance.CheckOutSectionRectY(this.transform.position.y))
        {
            nowPos.y = frontPos.y;
        }
        this.transform.position = nowPos;
    }
    /// <summary>
    /// 手部攻击
    /// </summary>
    public override void Punch()
    {
        //当前能不能判断执行攻击行为
        if (!CanAtk)
        {
            return;
        }

        //方式1 触发条件去处理
        ChangeAction(E_Action_Type.Punch);
        //方式2 通过int累加去处理
        //CancelInvoke("DelayClearAtkCount");
        //++atkCount;
        //roleAnimator.SetInteger("atkCount",atkCount);
        //Invoke("DelayClearAtkCount",0.3f);
    }
    /// <summary>
    /// 死亡
    /// </summary>
    public override void Dead()
    {
        isDead = true;
        GameData data = new GameData() { killCount = Main.killCount, coinCount = Main.coinCount };
        BinaryDataMgr.Instance.SaveData(data, "GameData");
        UIManager.Instance.ShowPanel<TipPanel>(E_UI_Layer.Middle, (panel) =>
        {
            panel.ChangeTip("游戏结束");
        });
    }
    /// <summary>
    /// 拾取
    /// </summary>
    private void PickUp()
    {
        //范围检测 检测和我们相交的对象
        //1.先判断 是否和地面上的物品重合
        Collider2D collider = Physics2D.OverlapBox(this.transform.position + Vector3.up*-1.181f,new Vector2(1,0.6f),0,1<<LayerMask.NameToLayer("PickUpItem"));
        if (collider == null)
            return;
        PickUpObject obj = collider.GetComponent<PickUpObject>();
        if (obj == null)
            return;
        ChangeAction(E_Action_Type.PickUp);
        print("拾取成功");
        property.atk += obj.addAtk;
        property.def += obj.addDef;
        property.nowHp += obj.addHp;
        if (property.nowHp > property.maxHp)
            property.nowHp = property.maxHp;
        UIManager.Instance.GetPanel<GamePanel>().UpdatePlayerHp(property.nowHp,property.maxHp);
        PoolMgr.Instance.PushObject("Prefabs/Objects/MeatPickup", obj.gameObject);

    }
    /// <summary>
    /// 投掷
    /// </summary>
    private void Throw()
    {
        ChangeAction(E_Action_Type.Throw);
    }
    /// <summary>
    /// 跳跃
    /// </summary>
    private void Jump()
    {
        //在地面是true 才能进行跳跃
        //之后如果还有不能跳的条件 之后再加
        if (CanJump)
        {
            //初始化当前跳跃速度
            nowYSpeed = jumpSpeed;
            //切换动作
            ChangeAction(E_Action_Type.Jump);
            //切换在地面的状态
            ChangeRoleIsGround(false);
        }
    }
    /// <summary>
    /// 跳跃攻击
    /// </summary>
    private void JumpAtk()
    {
        //如果当前处于跳跃攻击状态 就不要再触发跳跃攻击
        //不是跳跃攻击状态 才能进行跳跃攻击
        if (!roleAnimator.GetCurrentAnimatorStateInfo(1).IsName("JumpKick"))
        {
            ChangeAction(E_Action_Type.JumpAtk);
        }
    }
    /// <summary>
    /// 腿部攻击
    /// </summary>
    private void Kick()
    {
        if (!CanAtk)
        {
            return;
        }
        ChangeAction(E_Action_Type.Kick);
    }
    /// <summary>
    /// 是否格挡
    /// </summary>
    private void Defend(bool isDefend)
    {
        roleAnimator.SetBool("isDefend",isDefend);
    }


    private void CheckX(float x)
    {
        //x 就会是 -1 0 1 三个值的数
        //按A 为-1 不按为0 按D为1
        moveDir.x = x;
    }
    private void CheckY(float y)
    {
        //y 就会是 -1 0 1 三个值的数
        //按S 为-1 不按为0 按W为1
        moveDir.y = y;
    }
    private void CheckKeyDown(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.J:
                //手部攻击
                //如果不在地面 处理 跳跃攻击逻辑
                if (!GetIsGround)
                    JumpAtk();
                //在地面上 就能够手部攻击
                else
                    Punch();
                break;
            case KeyCode.K:
                //腿部攻击
                //如果不在地面 处理 跳跃攻击逻辑
                if (!GetIsGround)
                    JumpAtk();
                else
                    Kick();
                break;
            case KeyCode.LeftShift:
                //进行格挡
                Defend(true);
                break;
            case KeyCode.L:
                //投掷
                Throw();
                break;
            case KeyCode.F:
                //拾取
                PickUp();
                break;
            case KeyCode.Space:
                Jump();
                break;
            case KeyCode.Escape:
                UIManager.Instance.ShowPanel<SettingPanel>(E_UI_Layer.Top);
                break;
            //测试按键
            case KeyCode.Alpha1:
                Hit(0.2f);
                break;
            case KeyCode.Alpha2:
                KnockDown(3, 5);
                break;
        }
    }
    private void CheckKeyUp(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.LeftShift:
                Defend(false);
                break;
        }
    }
    /// <summary>
    /// 给予控制权
    /// </summary>
    public void AddController()
    {
        //事件中心有加就有减 一定不要传lambda表达式 一定要在下方去声明函数
        EventCenter.Instance.AddEventListener<float>("Horizontal", CheckX);
        EventCenter.Instance.AddEventListener<float>("Vertical", CheckY);
        //监听按键按下内容
        EventCenter.Instance.AddEventListener<KeyCode>("SomeKeyDown", CheckKeyDown);
        //监听按键抬起内容
        EventCenter.Instance.AddEventListener<KeyCode>("SomeKeyUp", CheckKeyUp);
    }

    /// <summary>
    /// 移除控制权
    /// </summary>
    public void RemoveController()
    {
        EventCenter.Instance.RemoveEventListener<float>("Horizontal", CheckX);
        EventCenter.Instance.RemoveEventListener<float>("Vertical", CheckY);
        EventCenter.Instance.RemoveEventListener<KeyCode>("SomeKeyDown", CheckKeyDown);
        EventCenter.Instance.RemoveEventListener<KeyCode>("SomeKeyUp", CheckKeyUp);
    }
    private void OnDestroy()
    {
        //注销事件
        RemoveController();
    }

    private bool CanJump
    {
        get
        {
            AnimatorStateInfo layerInfo2 = roleAnimator.GetCurrentAnimatorStateInfo(1);
            if (GetIsGround  &&
               !roleAnimator.GetBool("isDefend") &&
               !roleAnimator.GetBool("isHit")    &&
               !roleAnimator.GetBool("isKnockDown")&&
               !layerInfo2.IsName("PickUp")         &&
               !layerInfo2.IsName("Throw")          &&
               !IsAtkState)
            {
                return true;
            }
            return false;
        }
    }
    protected bool CanAtk
    {
        get
        {
            AnimatorStateInfo layerInfo2 = roleAnimator.GetCurrentAnimatorStateInfo(1);
            if (roleAnimator.GetBool("isDefend") ||
                roleAnimator.GetBool("isHit") ||
                roleAnimator.GetBool("isKnockDown") ||
                layerInfo2.IsName("PickUp") ||
                layerInfo2.IsName("Throw"))
            {
                return false;
            }
            return true;
        }
    }
}
