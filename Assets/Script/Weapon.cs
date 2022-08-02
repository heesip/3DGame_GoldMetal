using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range } //무기공격 타입 { 근접, 원거리 }
    public Type type;
    public int damage; //데미지
    public float rate; //공격속도
    public BoxCollider meleeArea; //근접공격 범위
    public TrailRenderer trailRenderer; //공격 효과
    public int maxAmmo; //전체 탄약 
    public int curAmmo; //현재 탄약

    public Transform bulletPos; //총알이 생성되는 위치
    public GameObject bullet; //총알 프리팹
    public Transform bulletCasePos; //탄피 생성되는 위치
    public GameObject bulletCase; //탄피 프리팹

    public void Use()
    {
        if (type == Type.Melee) //무기 타입이 근접이면
        {
            //스윙 코루틴 함수 실행
            StopCoroutine(Swing()); 
            StartCoroutine(Swing());
        }
        else if (type == Type.Range && curAmmo > 0) //원거리 타입에 현재 탄약이 1개라도 있으면
        {
            curAmmo--;
            //샷 코루틴 함수 실행
            StopCoroutine(Shot());
            StartCoroutine(Shot());
        }

    }

    IEnumerator Swing() //스윙 코루틴 함수
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true; //공격범위 활성화
        trailRenderer.enabled = true; //이팩트 활성화

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false; //공격범위 비활성화

        yield return new WaitForSeconds(0.3f);
        trailRenderer.enabled = false; //이팩트 비활성화
    }

    IEnumerator Shot() //샷 코루틴 함수
    {
        //발사 시킬 총알 오브젝트를 잡아줌
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        //그 오브젝트에서 리지드바디를 할당함
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        //할당한 리지드바디를 활용해 총알을 앞쪽으로 50만큼 가속도를 붙여줌
        bulletRigid.velocity = bulletPos.forward * 50;

        yield return new WaitForSeconds(0.1f); //0.1초 후
        
        //탄피 오브젝트를 잡아줌
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        //해당 오브젝트에서 리지드바디를 할당함
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        //탄피가 날아갈 방향과 힘을 랜덤으로 잡음
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-4, -2) + Vector3.up * Random.Range(2, 4);
        //위에서 할당한 리지드바디를 활용해 탄피를 날림
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        //위와 비슷한 방식으로 탄피를 10만큼의 힘으로 회전 시킴
        caseRigid.AddTorque(Vector3.up * 10,ForceMode.Impulse);

    }

}
