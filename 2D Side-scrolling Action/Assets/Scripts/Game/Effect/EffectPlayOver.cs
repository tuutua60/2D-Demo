using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlayOver : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        Debug.Log("粒子特效播放完毕");
        PoolMgr.Instance.PushObject(this.transform.parent.gameObject.name,this.transform.parent.gameObject);
    }
}
