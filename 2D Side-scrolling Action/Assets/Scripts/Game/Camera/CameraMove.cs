using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    //摄像机移动的目标点
    private Vector3 targetPos;
    //摄像机移动速度
    public float moveSpeed = 10;

    //上一次摄像机所在的坐标
    private Vector3 frontPos;
    private void Start()
    {
        targetPos = this.transform.position;

    }

    private void LateUpdate()
    {
        //移动之前记录移动前的位置 
        frontPos = this.transform.position;
        if (PlayerObject.player == null)
            return;
        targetPos.x = PlayerObject.player.transform.position.x;
        
        
        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, moveSpeed * Time.deltaTime);

        //判断可视范围 左右 是否超出了 分段左右
        if( LevelMgr.Instance.CheckOutSectionRectX(Camera.main.ScreenToWorldPoint(Vector3.zero).x)||
            LevelMgr.Instance.CheckOutSectionRectX(Camera.main.ScreenToWorldPoint(Vector3.right * Screen.width).x))
        {
            this.transform.position = frontPos;
        }

    }
}
