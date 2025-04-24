using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagSetter : MonoBehaviour
{
    void Start()
    {
        // このゲームオブジェクトと全ての子オブジェクトにタグを設定
        SetTagRecursively(transform, "TetriminoBlock");
    }
    
    void SetTagRecursively(Transform parent, string tag)
    {
        parent.gameObject.tag = tag;
        
        foreach (Transform child in parent)
        {
            SetTagRecursively(child, tag);
        }
    }
}
