using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BgScroller : MonoBehaviour
{
    private RawImage img;
    public float moveSpeed = 0.05f;
    private void Awake()
    {
        img = GetComponent<RawImage>();
    }

    private void Update()
    {
        Rect uvRect = img.uvRect;
        uvRect.x += moveSpeed * Time.deltaTime;
        img.uvRect = uvRect;
    }
}
