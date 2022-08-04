﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth; //최대 체력
    public int curHealth; //현재 체력
    public Transform target; //타겟
    public BoxCollider meleeArea;//공격 범위
    public bool isChase; //추적하다
    public bool isAttack; //공격
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
        meleeArea.enabled = false;

        Invoke("ChaseStart", 2); //게임이 시작하고 2초가 지나면 추적
    }

    private void Update()
    {
        if(nav.enabled)
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
        float targetRadius = 1.5f;
        float targetRange = 2f;

        RaycastHit[] rayhit =
            Physics.SphereCastAll(transform.position, targetRadius,
            transform.forward, targetRange, LayerMask.GetMask("Player"));

        if(rayhit.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        yield return new WaitForSeconds(0.6f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.8f);

        anim.SetBool("isAttack", false);
        isChase = true;
        isAttack = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee") //근접무기가 닿으면
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage; //현재 체력을 무기 데미지 만큼 감소 
            //Enemy 위치 값에서 enemy를 때린 무기 위치값을 빼서 값을 reactVec에 저장
            Vector3 reactVec = transform.position - other.transform.position; 
            StartCoroutine(OnDamage(reactVec,false));
        }
        else if(other.tag == "Bullet") //총알에 닿으면
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage; //현재 체력을 총알 데미지 만큼 감소
            //Enemy 위치 값에서 enemy를 때린 무기 위치값을 빼서 값을 reactVec에 저장
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVec,false));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade) //피격 함수 (피격시 받은 reactVec값 받아옴), 피격 준 물체가 수류탄인지 확인
    {
        mat.color = Color.red; //빨간색으로 변함
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0) //현재 체력이 0보다 크면
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
            nav.enabled =false;
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
            
            
           
            Destroy(gameObject,1.2f); //1.2초 후 파괴
        }
    }
    public void HitByGrenade(Vector3 explosionPos) //수류탄 피격함수
    {
        curHealth -= 100; //체력 100감소
        Vector3 reactVec = transform.position - explosionPos; 
        StartCoroutine(OnDamage(reactVec,true));

    }
}
