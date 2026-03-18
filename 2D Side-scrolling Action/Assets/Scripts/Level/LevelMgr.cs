using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMgr : BaseManager<LevelMgr>
{
    //关卡分段数据
    private string[] sections;
    //当前分段索引
    private int nowSectionIndex = 0;
    //分段边界范围
    public SectionRect sectionRect;
    //当前分段有多少只敌人
    public int enemyNum;

    /// <summary>
    /// 根据关卡ID 初始化关卡信息
    /// </summary>
    /// <param name="id"></param>
    public void InitLevel(int id)
    {
        //获取关卡数据
        T_Level_Total_Info levelInfo = BinaryDataMgr.Instance.GetTable<T_Level_Total_InfoContainer>().dataDic[id];
        //创建地图
        ResourcesMgr.Instance.LoadAsync<GameObject>(levelInfo.f_map_path, (map) =>
        {
            map.transform.position = Vector3.zero;
            //地图加载结束 再去创建玩家
            ResourcesMgr.Instance.LoadAsync<GameObject>("Prefabs/Role/Player1", (player) =>
            {
                string[] pos = levelInfo.f_Player_BornPos.Split(",");
                player.transform.position = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]),0);
            });
            //处理分段数据
            sections = levelInfo.f_sections.Split(",");
            //从第一个分段开始
            nowSectionIndex = 0;
            //初始化当前第一个分段
            InitSectionInfo(int.Parse(sections[nowSectionIndex]));
        });

        //显示GamePanel
        UIManager.Instance.ShowPanel<GamePanel>(E_UI_Layer.Middle,(panel) =>
        {
            panel.UpdateKillCount(Main.killCount);
            panel.UpdateCoinCount(Main.coinCount);
        });

        //改变关卡背景音乐
        MusicMgr.Instance.PlayBkMusic(levelInfo.f_BKMusic_Path);
        //添加敌人死亡的事件监听
        EventCenter.Instance.AddEventListener("EnemyDead", CheckEnemyDead);
    }

    /// <summary>
    /// 用来初始化分段信息
    /// 创建怪物 创建物品 记录左右边界坐标
    /// </summary>
    /// <param name="id"></param>
    private void InitSectionInfo(int id)
    {
        //获取分段信息
        T_Level_Section_Info sectionInfo = BinaryDataMgr.Instance.GetTable<T_Level_Section_InfoContainer>().dataDic[id];
        //创建怪物
        string[] info1 = sectionInfo.f_enemy_Info.Split(";");
        //记录怪物数量
        enemyNum = info1.Length;
        for (int i = 0; i < info1.Length; i++)
        {
            string[] position = info1[i].Split(",");
            //创建怪物
            T_EnemyInfo enemyInfo = BinaryDataMgr.Instance.GetTable<T_EnemyInfoContainer>().dataDic[int.Parse(position[0])];
            PoolMgr.Instance.GetObject(enemyInfo.f_modelPath, (obj) =>
            {
                string xPos = position[1];
                string yPos = position[2];
                obj.transform.position = new Vector3(float.Parse(xPos),float.Parse(yPos),0);
                obj.GetComponent<RoleObject>().InitProperty(int.Parse(position[0]));
            });
        }
        //创建道具
        string[] info2 = sectionInfo.f_enemy_Info.Split(";");
        for (int i = 0; i < info2.Length; i++)
        {
            string[] position = info1[i].Split(",");
            //创建道具
            T_Item_Info itemInfo = BinaryDataMgr.Instance.GetTable<T_Item_InfoContainer>().dataDic[int.Parse(position[0])];
            PoolMgr.Instance.GetObject(itemInfo.f_Model_Path, (obj) =>
            {
                string xPos = position[1];
                string yPos = position[2];
                obj.transform.position = new Vector3(float.Parse(xPos), float.Parse(yPos), 0);
                obj.GetComponent<ThingObject>().Init(itemInfo);
            });
        }

        //获取边界
        string[] rects = sectionInfo.f_range.Split(",");
        
        sectionRect = new SectionRect(float.Parse(rects[0]), float.Parse(rects[1]), float.Parse(rects[2]), float.Parse(rects[3]));
        
        
        ++nowSectionIndex;
    }

    /// <summary>
    /// 判断x坐标是否超出了左右边界
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public bool CheckOutSectionRectX(float x)
    {
        if(x < sectionRect.left || x > sectionRect.right)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 判断y坐标是否超出了左右边界
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool CheckOutSectionRectY(float y)
    {
        if(y>sectionRect.top || y < sectionRect.bottom)
        {
            return true;
        }
        return false;
    }

    private void CheckEnemyDead()
    {
        --enemyNum;
        Main.killCount++;
        Main.coinCount += Random.Range(5, 15);
        UIManager.Instance.GetPanel<GamePanel>()?.UpdateKillCount(Main.killCount);
        UIManager.Instance.GetPanel<GamePanel>()?.UpdateCoinCount(Main.coinCount);
        if (enemyNum == 0)
        {
            //切换分段 或者其他相关逻辑
            if(nowSectionIndex == sections.Length)
            {
                //关卡结束逻辑
                Debug.Log("通关");
                UIManager.Instance.ShowPanel<TipPanel>(E_UI_Layer.Middle, (panel) =>
                {
                    panel.ChangeTip("通关");
                });
                GameData data = new GameData() { killCount = Main.killCount, coinCount = Main.coinCount };
                BinaryDataMgr.Instance.SaveData(data, "GameData");
            }
            else
            {
                InitSectionInfo(int.Parse(sections[nowSectionIndex]));
            }
        }
    }
}


/// <summary>
/// 分段边界范围
/// </summary>
public struct SectionRect
{
    public float right;
    public float left;
    public float top;
    public float bottom;
    public SectionRect(float l, float r, float t, float b)
    {
        this.right = r;
        this.left = l;
        this.top = t;
        this.bottom = b;
    }
}