using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class PickUpObject : MonoBehaviour
{
    public float addAtk = 2;
    public float addDef = 1;
    public float addHp = 20;


    public void Init()
    {
        Vector3 pos = this.transform.position;
        pos.z = pos.y+0.8f;
        this.transform.position = pos;
    }
}
