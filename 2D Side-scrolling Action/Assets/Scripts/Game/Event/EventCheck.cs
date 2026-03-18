using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventCheck : MonoBehaviour
{
    //用于存储再伤害的检测时真正调用的函数的委托
    public event UnityAction<int> checkDamage;
    //存储对象死亡
    public event UnityAction checkDead;
    //存储物品死亡
    public event UnityAction checkItemDead;
    /// <summary>
    /// 用于给动画事件关联的函数
    /// </summary>
    public void CheckDamage(int id)
    {
        //调用委托
        checkDamage?.Invoke(id);
    }

    public void CheckDead()
    {
        checkDead?.Invoke();

        EventCenter.Instance.EventTrigger("EnemyDead");
    }

    public void CreateKnife()
    {
        PoolMgr.Instance.GetObject("Prefabs/Objects/Knife", (obj) => {
            KnifeObject knifeObj = obj.GetComponent<KnifeObject>();
            knifeObj.knifeAtk = PlayerObject.player.property.atk;
            if (PlayerObject.player.BodyisRight)
            {
                obj.transform.position = PlayerObject.player.transform.position + new Vector3(1.63f, 0.4f, 0);
                knifeObj.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                obj.transform.position = PlayerObject.player.transform.position + new Vector3(-1.63f, 0.4f, 0);
                knifeObj.transform.rotation = Quaternion.Euler(0,180,0);
            }
        });
    }

    public void CheckItemDead()
    {
        checkItemDead?.Invoke();
    }
}
