using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
 
 [ExecuteInEditMode]
public class TextArroundPlayer : MonoBehaviour
{
    
    public float acceleration = 1, speed = 10;
    public RectTransform[] transforms;

    private void FixedUpdate()
    {
        foreach(RectTransform r in transforms)
        {
            r.rotation = Quaternion.Euler(r.rotation.x, r.rotation.y + speed, r.rotation.z);
            
        }
        speed += acceleration;

    }

}