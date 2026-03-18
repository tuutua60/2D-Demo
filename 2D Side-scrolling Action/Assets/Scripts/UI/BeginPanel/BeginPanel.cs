using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeginPanel : BasePanel
{
    private bool isLoadGame = false;

    protected override void Awake()
    {
        base.Awake();
        ChangeFadeSpeed(2f);
    }

    protected override void Update()
    {
        base.Update();
        if (Input.anyKeyDown&&!isLoadGame)
        {
            isLoadGame = true;
            UIManager.Instance.HidePanel<BeginPanel>();
            ScenesMgr.Instance.LoadSceneAsyn("GameScene", () =>
            {
                print("场景切换");
                //关卡初始化
                LevelMgr.Instance.InitLevel(1001);
            });
        }
    }
    public override void ShowMe()
    {
        base.ShowMe();
        ChangeFadeSpeed(10f);
    }
    public override void HideMe(UnityAction callback = null)
    {
        ChangeFadeSpeed(0.5f);
        base.HideMe(callback);
    }
}
