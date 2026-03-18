using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingObject : MonoBehaviour
{
    private int nowHp = 2;
    //掉落物品预设体路径
    private string downThingPath;
    //爆率
    private float factor;
    private Animator animator;
    private bool isDead;
    public bool IsDead => isDead;
    private void Awake()
    {
        animator = this.GetComponentInChildren<Animator>();
        animator.GetComponent<EventCheck>().checkItemDead += () =>
        {
            PoolMgr.Instance.PushObject(this.gameObject.name, this.gameObject);
        };
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(T_Item_Info info)
    {
        Vector3 pos = this.transform.position;
        pos.z = pos.y+0.2f;
        this.transform.position = pos;
        this.transform.rotation = Quaternion.identity;
        nowHp = info.f_Hit_Count;
        downThingPath = info.f_Down_Thing;
        factor = info.f_Down_Factor*1000;
        isDead = false;
        animator.SetBool("isDead",isDead);
    }

    public void Wound()
    {
        //减血
        nowHp--;
        if(nowHp <= 0)
        {
            Dead();
        }
    }

    public void Dead()
    {
        print("物品破坏");
        isDead = true;
        animator.SetBool("isDead",isDead);
        //随机掉落物品
        if(Random.Range(1, 1001) <= (int)factor ? true : false)
        {
            PoolMgr.Instance.GetObject(downThingPath, (obj) =>
            {
                obj.transform.position = this.transform.position;
                obj.GetComponent<PickUpObject>().Init();
            });
        }
    }
}
