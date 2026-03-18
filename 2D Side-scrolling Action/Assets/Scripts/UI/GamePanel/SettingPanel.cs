using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SettingPanel : BasePanel
{
    protected override void OnClick(string btnName)
    {
        base.OnClick(btnName);

        switch (btnName)
        {
            case "btnQuit":
                Application.Quit();
                break;
            case "btnReturn":
                UIManager.Instance.HidePanel<SettingPanel>();
                break;
        }
    }

    public override void ShowMe()
    {
        base.ShowMe();
        PlayerObject.player.RemoveController();
    }

    public override void HideMe(UnityAction callback = null)
    {
        base.HideMe(callback);
        PlayerObject.player.AddController();
    }
}
