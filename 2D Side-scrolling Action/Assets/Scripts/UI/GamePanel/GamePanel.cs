using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    private GameObject enemyHp;
    private GameObject goObj;
    private float playerHpWidth;
    private float enemyHpWidth;
    private Image imgPlayerHp;
    private Image imgEnemyHp;
    private Text txtKillCount;
    private Text txtCoinCount;
    protected override void Awake()
    {
        base.Awake();
        enemyHp = this.transform.Find("EnemyHp").gameObject;
        goObj = this.transform.Find("Go").gameObject;
        imgPlayerHp = this.GetControl<Image>("imgPlayerHp");
        imgEnemyHp = this.GetControl<Image>("imgEnemyHp");
        txtKillCount = this.GetControl<Text>("txtKillCount");
        txtCoinCount = this.GetControl<Text>("txtCoin");
        enemyHp.SetActive(false);
        goObj.SetActive(false);
        playerHpWidth = imgPlayerHp.rectTransform.sizeDelta.x;
        enemyHpWidth = imgEnemyHp.rectTransform.sizeDelta.x;
    }

    /// <summary>
    /// 显示Go图标
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowOrHideGo(bool isShow)
    {
        goObj.SetActive(isShow);
    }

    /// <summary>
    /// 更新玩家血条
    /// </summary>
    /// <param name="nowHp"></param>
    /// <param name="maxHp"></param>
    public void UpdatePlayerHp(float nowHp,float maxHp)
    {
        imgPlayerHp.rectTransform.sizeDelta = new Vector2(nowHp / maxHp * playerHpWidth, imgPlayerHp.rectTransform.sizeDelta.y);
    }

    /// <summary>
    /// 更新敌人血条
    /// </summary>
    /// <param name="nowHp"></param>
    /// <param name="maxHp"></param>
    public void UpdateEnemyHp(float nowHp, float maxHp)
    {
        imgEnemyHp.rectTransform.sizeDelta = new Vector2(nowHp / maxHp * enemyHpWidth, imgEnemyHp.rectTransform.sizeDelta.y);
        enemyHp.SetActive(true);
        CancelInvoke("DelayHideEnemyHp");
        Invoke("DelayHideEnemyHp",1f);
    }

    /// <summary>
    /// 更新击杀数
    /// </summary>
    public void UpdateKillCount(int count)
    {
        txtKillCount.text = count.ToString();
    }

    /// <summary>
    /// 更新金币数
    /// </summary>
    /// <param name="count"></param>
    public void UpdateCoinCount(int count)
    {
        txtCoinCount.text = "$: "+ count.ToString();
    }

    private void DelayHideEnemyHp()
    {
        enemyHp.SetActive(false);
    }
}
