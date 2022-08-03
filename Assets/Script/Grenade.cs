using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        meshObj.SetActive(true);
        effectObj.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(Boom());
    }

    IEnumerator Boom()
    {
        yield return new WaitForSeconds(3f); //3초 후
        rigid.velocity = Vector3.zero; //가속도 0
        rigid.angularVelocity = Vector3.zero; //회전 가속도 0
        meshObj.SetActive(false); //수류탄 이미지 비활성화
        effectObj.SetActive(true); //폭발 이미지 활성화

        Destroy(gameObject, 1.5f); //1.5초뒤 자신을 파괴
    }
}
