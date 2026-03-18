using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperty : BaseProperty
{
    //升级所需经验
    public int levelUpExp;
    //当前经验
    public int nowExp;
    //当前等级
    public int nowLevel;
    public PlayerProperty(int infoId) : base(infoId)
    {
        if (!BinaryDataMgr.Instance.GetTable<T_PlayerLevelInfoContainer>().dataDic.ContainsKey(infoId))
            return;
        T_PlayerLevelInfo info = BinaryDataMgr.Instance.GetTable<T_PlayerLevelInfoContainer>().dataDic[infoId];
        if (info == null)
            return;

        this.atk = info.f_atk;
        this.def = info.f_def;
        this.maxHp = info.f_hp;
        this.nowHp = maxHp;
        this.levelUpExp = info.f_levelUp_exp;
        this.nowExp = 0;
        this.nowLevel = infoId;
    }
}
