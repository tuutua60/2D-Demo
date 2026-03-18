using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeObject : MonoBehaviour
{
    public float knifeAtk = 0;
    public float moveSpeed = 10f;
    public Transform shadowTrans;
    private void OnEnable()
    {
        Invoke("PushPool",1.4f);
    }

    private void Update()
    {
        this.transform.Translate(Vector3.right*moveSpeed*Time.deltaTime);
    }

    private void PushPool()
    {
        PoolMgr.Instance.PushObject(this.gameObject.name, this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent == null)
            return;
        RoleObject roleObj = collision.transform.parent.GetComponent<RoleObject>();
        if(roleObj == null)
            return;
        if (roleObj.roleType == E_Role_Type.Enemy&&!roleObj.isDead)
        {
            //判断伪z轴
            if (Mathf.Abs(this.transform.position.y - roleObj.transform.position.y-0.36f) > 0.6f)
                return;


            print("飞刀打到敌人");
            //飞刀伤害无倍率 无视防御
            roleObj.property.nowHp -= knifeAtk*0.6f;
            if(roleObj.property.nowHp <=0)
            {
                roleObj.property.nowHp = 0;
                roleObj.Dead();
            }
            else
            {
                roleObj.Hit(0.18f);
            }
            //更新怪物血条
            UIManager.Instance.GetPanel<GamePanel>().UpdateEnemyHp(roleObj.property.nowHp,roleObj.property.maxHp);
            //播放音效
            MusicMgr.Instance.PlaySound("dead");
            //将自己放入缓存池
            CancelInvoke("PushPool");
            PushPool();
        }
    }
}
