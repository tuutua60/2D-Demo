using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 抽屉数据  池子中的一列容器
/// </summary>
public class PoolData
{
    //抽屉中 对象挂载的父节点
    public GameObject fatherObj;
    //对象的容器
    public List<GameObject> poolList;

    public PoolData(GameObject obj, GameObject poolObj)
    {
        //给我们的抽屉 创建一个父对象 并且把他作为我们pool(衣柜)对象的子物体
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.parent = poolObj.transform;
        poolList = new List<GameObject>() {};
        PushObj(obj);
    }

    /// <summary>
    /// 往抽屉里面 放东西
    /// </summary>
    /// <param name="obj"></param>
    public void PushObj(GameObject obj)
    {
        //失活 让其隐藏
        obj.SetActive(false);
        //存起来
        poolList.Add(obj);
        //设置父对象
        obj.transform.parent = fatherObj.transform;
    }

    /// <summary>
    /// 从抽屉里面 取东西
    /// </summary>
    /// <returns></returns>
    public GameObject GetObj()
    {
        GameObject obj = null;
        //取出第一个
        obj = poolList[0];
        poolList.RemoveAt(0);
        //激活 让其显示
        obj.SetActive(true);
        //断开了父子关系
        obj.transform.parent = null;

        return obj;
    }
}

/// <summary>
/// 缓存池模块
/// 1.Dictionary List
/// 2.GameObject 和 Resources 两个公共类中的 API 
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    //缓存池容器 （衣柜）
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    private GameObject poolObj;

    /// <summary>
    /// 从缓存池拿东西
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <returns></returns>
    public void GetObject(string path, UnityAction<GameObject> callBack)
    {
        //有抽屉 并且抽屉里有东西
        if (poolDic.ContainsKey(path) && poolDic[path].poolList.Count > 0)
        {
            callBack(poolDic[path].GetObj());
        }
        else
        {
            //通过异步加载资源 创建对象给外部用
            ResourcesMgr.Instance.LoadAsync<GameObject>(path, (o) =>
            {
                o.name = path;
                callBack(o);
            });
        }
    }


    /// <summary>
    /// 往缓存池放东西
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="obj">要放进缓存池的对象</param>
    public void PushObject(string path, GameObject obj)
    {
        if (poolObj == null)
            poolObj = new GameObject("Pool");

        //里面有抽屉
        if (poolDic.ContainsKey(path))
        {
            poolDic[path].PushObj(obj);
        }
        //里面没有抽屉
        else
        {
            poolDic.Add(path, new PoolData(obj, poolObj));
        }
    }


    /// <summary>
    /// 清空缓存池的方法 
    /// 主要用在 场景切换时
    /// </summary>
    public void Clear()
    {
        poolDic.Clear();
        poolObj = null;
    }
}
