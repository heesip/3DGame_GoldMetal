using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target; //따라다닐 타겟(플레이어)
    public Vector3 offset; //고정값    

    void Update()
    {
        //카메라 위치는 타겟 위치 + 고정값을 따라간다. 
        transform.position = target.position + offset;
    }
}
