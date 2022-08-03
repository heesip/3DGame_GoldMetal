using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage; //데미지 엔진에서 적용


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") //바닥에 닿으면 (탄피에 사용)
        {
            Destroy(gameObject, 2.5f); //자신을 2.5초 후 파괴

        }
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Wall") //벽에 닿으면 (총알에 사용)
            Destroy(gameObject); //자신을 즉시 파괴

    }
}
