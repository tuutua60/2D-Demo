using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private static bool hasInit = false;

    private static GameData gameData;

    public static int killCount
    {
        get { return gameData.killCount; }
        set { gameData.killCount = value; }
    }
    public static int coinCount
    {
        get { return gameData.coinCount; }
        set { gameData.coinCount = value; }
    }

    void Awake()
    {
        if (!hasInit)
        {
            hasInit = true;
            //如果数据不多或者经常用 可以一进游戏就加载
            BinaryDataMgr.Instance.InitData();
            try
            {
                gameData = BinaryDataMgr.Instance.LoadData<GameData>("GameData");
            }
            catch
            {
                gameData = new GameData();
            }
            //显示开始面板
            UIManager.Instance.ShowPanel<BeginPanel>();
            //播放开始场景的背景音乐

            MusicMgr.Instance.ChangeBKValue(0.3f);
            MusicMgr.Instance.ChangeSoundValue(0.3f);
        }
        MusicMgr.Instance.PlayBkMusic("Begin");

        PoolMgr.Instance.Clear();
        EventCenter.Instance.Clear();

    }

    void Update()
    {
        
    }
}
