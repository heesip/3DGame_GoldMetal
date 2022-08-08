using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C };
    public Type enemyType;
    public int maxHealth; //최대 체력
    public int curHealth; //현재 체력
    public Transform target; //타겟
    public BoxCollider meleeArea;//공격 범위
    public bool isChase; //추적하다
    public bool isAttack; //공격
    public GameObject bullet;
    Rigidbody enemyrigid;
    BoxCollider boxcol;
    Material mat;
    NavMeshAgent nav;
    Animator anim;



    private void Awake()
    {
        enemyrigid = GetComponent<Rigidbody>();
        boxcol = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2); //게임이 시작하고 2초가 지나면 추적
    }

    private void Update()
    {
        if (nav.enabled)
        {
            nav.SetDestination(target.position); // 타겟을 따라 이동
            nav.isStopped = !isChase;
        }
    }

    private void FixedUpdate()
    {
        FreezeVelocity(); //충돌시 강제 회전 방지
        Targeting();
    }

    void ChaseStart() //추적 시작 함수
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            enemyrigid.velocity = Vector3.zero; //플레이어 가속도 0으로 잡아줌
            enemyrigid.angularVelocity = Vector3.zero; //플레이어 회전 가속도 0으로 잡아줌
        }
    }
    void Targeting()
    {
        float targetRadius = 0f; //타겟 감지
        float targetRange = 0f; //공격 범위

        switch (enemyType) //몬스터 타입
        {
            case Type.A: //A타입 몬스터
                targetRadius = 1.5f;
                targetRange = 3f;
                break;
            case Type.B: //B타입 몬스터
                targetRadius = 1f;
                targetRange = 12f;
                break;
            case Type.C:
                targetRadius = 0.5f;
                targetRange = 25f;
                break;
            default:
                break;
        }

        //레이캐스트 안에 플레이어가 들어있고 몬스터 앞쪽으로 공격 범위 안으로 들어와 있으면
        RaycastHit[] rayhit =
            Physics.SphereCastAll(transform.position, targetRadius,
            transform.forward, targetRange, LayerMask.GetMask("Player"));

        //위에서 만든 레이케스트 크기가 0보다 크고 공격이 가능한 상황이면 
        if (rayhit.Length > 0 && !isAttack) 
        {
            StartCoroutine(Attack()); //공격 코루틴 함수 실행
        }
    }

    IEnumerator Attack()
    {
        isChase = false; //플레이어 추적 정지
        isAttack = true; //공격 불가능한 상태 (공격이 진행중에 중첩되서 실행되는 것을 방지)
        anim.SetBool("isAttack", true); //공격 애니메이션 실행
        switch (enemyType)
        {
            case Type.A:
                //0.6초 후 애니메이션에 알맞게 콜라이더 활성화(플레이어 피격)
                yield return new WaitForSeconds(0.6f);
                meleeArea.enabled = true;
                //0.2초 후 공격하는 콜라이더 비활성화(한번에 공격으로 피격은 한번만 일어나게함)
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = false;
                // 0.8초후 
                yield return new WaitForSeconds(0.8f);
                break;

            case Type.B:
                // 0.2초 후 앞쪽으로 30만큼 힘을가하며 공격 콜라이더 활성화
                yield return new WaitForSeconds(0.2f); 
                enemyrigid.AddForce(transform.forward * 30, ForceMode.Impulse);
                meleeArea.enabled = true; 
                // 0.5초 후 몬스터에게 붙은 가속도 0으로 변환
                yield return new WaitForSeconds(0.5f);
                enemyrigid.velocity = Vector3.zero; 
                meleeArea.enabled = false; 
                //2초간 멈춤
                yield return new WaitForSeconds(2f);
                break;

            case Type.C:
                //0.5초 후
                yield return new WaitForSeconds(0.5f);
                //미사일 생성 및 발사 
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;
                //2초간 멈춤
                yield return new WaitForSeconds(2f);
                break;

            default:
                break;
        }
        
        anim.SetBool("isAttack", false); //공격 애니메이션 비활성화
        isChase = true; //플레이어 추적 시작
        isAttack = false; //공격 가능한 상태
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee") //근접무기가 닿으면
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage; //현재 체력을 무기 데미지 만큼 감소 
            //Enemy 위치 값에서 enemy를 때린 무기 위치값을 빼서 값을 reactVec에 저장
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));
        }
        else if (other.tag == "Bullet") //총알에 닿으면
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage; //현재 체력을 총알 데미지 만큼 감소
            //Enemy 위치 값에서 enemy를 때린 무기 위치값을 빼서 값을 reactVec에 저장
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVec, false));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade) //피격 함수 (피격시 받은 reactVec값 받아옴), 피격 준 물체가 수류탄인지 확인
    {
        mat.color = Color.red; //빨간색으로 변함
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0) //현재 체력이 0보다 크면
        {
            mat.color = Color.white; //원래 흰색으로 변경
        }

        else //0보다 작거나 같으면
        {
            mat.color = Color.gray;  //회색으로 변경
            gameObject.layer = 11; //레이어를 11번(EnemyDead)로 변경
            anim.SetBool("isWalk", false);
            anim.SetTrigger("doDie");
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");
            if (isGrenade) //데미지를 준게 수류탄이면
            {
                reactVec = reactVec.normalized; //react값이 어느방향이든 같은 값을 가져옴
                reactVec += Vector3.up * 3.5f; //react값을 위쪽 방향으로 더해줌
                enemyrigid.freezeRotation = false; //회전이 가능하게 변경
                enemyrigid.AddForce(reactVec * 5, ForceMode.Impulse); //그 값을 5만큼 곱해서 힘을 가함
                enemyrigid.AddTorque(reactVec * 15, ForceMode.Impulse); //그 값을 15만큼 곱해서 회전을 가함

            }
            else //그게아니면
            {
                reactVec = reactVec.normalized; //react값이 어느방향이든 같은 값을 가져옴
                reactVec += Vector3.up; //react값을 위쪽 방향으로 더해줌
                enemyrigid.AddForce(reactVec * 5, ForceMode.Impulse); //그 값을 5만큼 곱해서 힘을 가함
            }



            Destroy(gameObject, 1.2f); //1.2초 후 파괴
        }
    }
    public void HitByGrenade(Vector3 explosionPos) //수류탄 피격함수
    {
        curHealth -= 100; //체력 100감소
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));

    }
}

