using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    private Text txtTip;

    protected override void Awake()
    {
        base.Awake();
        txtTip = GetControl<Text>("txtTip");
    }

    protected override void OnClick(string btnName)
    {
        base.OnClick(btnName);

        switch (btnName)
        {
            case "btnQuit":
                Application.Quit();
                break;
            case "btnReturn":
                UIManager.Instance.HidePanel<TipPanel>();
                UIManager.Instance.HidePanel<GamePanel>();
                ScenesMgr.Instance.LoadScene("BeginScene",()=>{
                    //显示开始面板
                    UIManager.Instance.ShowPanel<BeginPanel>();
                    //播放开始场景的背景音乐
                    MusicMgr.Instance.PlayBkMusic("Begin");
                });
                break;
        }
    }

    public void ChangeTip(string tip)
    {
        txtTip.text = tip;
    }

    public override void ShowMe()
    {
        base.ShowMe();
        PlayerObject.player.RemoveController();
    }
}
