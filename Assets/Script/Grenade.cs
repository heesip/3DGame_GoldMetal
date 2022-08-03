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

        //레이캐스트 배열을 만들어 보이지는 않지만 수류탄 주변으로 반지름 15크기 안에 들어오는 적들의 정보를 가져옴
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15,
                                                     Vector3.up, 0f,
                                                     LayerMask.GetMask("Enemy"));

        foreach(RaycastHit hitObj in rayHits) //가져온 적들에게 전부 수류탄 데미지 + 위치 정보를 적용
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 1.5f); //1.5초뒤 자신을 파괴
    }
}
