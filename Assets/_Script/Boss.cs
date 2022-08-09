using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public GameObject missile; //미사일
    public Transform missilePortA; //미사일 포트 A 
    public Transform missilePortB; //미사일 포트 B

    Vector3 lookVec; //플레이어 움직임 예측 벡터 변수
    Vector3 tauntVec; //어디로 뛸지 지정하는 벡터 변수
    [SerializeField] bool isLook = true; //점프시 방향을 유지하는 변수

    private void Start()
    {
        nav.isStopped = true;
        StartCoroutine(Think());
    }
    private void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }
        if (!isDead)
        {
            if (isLook) //플레이어를 바라보고 있으면
            {
                //플레이어 좌표에서 다음으로 이동할 곳을 예측해 점프하며 이동
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                lookVec = new Vector3(h, 0, v) * 3f;
                transform.LookAt(target.position + lookVec);
            }
            else
                nav.SetDestination(tauntVec);
        }
        
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int ranAction = Random.Range(0, 5);
        switch (ranAction)
        {
            case 0:
            case 1:
                StartCoroutine(MissileShot());
                break; //미사일 발사 패턴
            case 2:
            case 3:
                StartCoroutine(RockeShot());
                break; //돌 발사 패턴
            case 4:
                StartCoroutine(Taunt());
                break; //점프 공격 패턴
        }
    }


    IEnumerator MissileShot()
    {
        //미사일 발사 애니메이션과 알맞게 유도 미사일을 2개 발사
        anim.SetTrigger("doShot"); 
        yield return new WaitForSeconds(0.2f);
        //미사일을 미사일포트 A에서 생성 후 플레이어를 추적하게 함
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);
        //미사일을 미사일포트 B에서 생성 후 플레이어를 추적하게 함
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2f);
        StartCoroutine(Think());
    }

    IEnumerator RockeShot()
    
    {   
        //캐릭터를 바라보는 것을 멈추고
        isLook = false;
        anim.SetTrigger("doBigShot");
        //바위를 생성
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);
        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator Taunt()
    {
        //캐릭터를 바라보는 것을 멈추고 캐릭터를 향해 점프 공격
        tauntVec = target.position + lookVec;
        isLook = false;
        nav.isStopped = false; //몬스터 이동 활성화
        boxcol.enabled = false; 
        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true; //근접 공격 활성화 

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false; //근접 공격 비활성화 

        yield return new WaitForSeconds(1.5f);
        isLook = true; 
        nav.isStopped = true;  //몬스터 이동 비활성화
        boxcol.enabled = true; 
        StartCoroutine(Think());
    }
}
