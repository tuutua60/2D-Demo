using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProperty : BaseProperty
{
    public int deadExp;
    public EnemyProperty(int infoId): base(infoId)
    {
        if (!BinaryDataMgr.Instance.GetTable<T_EnemyInfoContainer>().dataDic.ContainsKey(infoId))
            return;
        T_EnemyInfo info = BinaryDataMgr.Instance.GetTable<T_EnemyInfoContainer>().dataDic[infoId];
        if (info == null)
            return;

        this.atk = info.f_atk;
        this.def = info.f_def;
        this.maxHp = info.f_hp;
        this.nowHp = maxHp;
        this.deadExp = info.f_exp;
    }
}
