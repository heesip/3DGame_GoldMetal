﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Vector3 moveVec; //이동 값을 받는 변수
    Vector3 dodgeVec; //이동 값을 받는 변수
    public float speed; //이동속도 
    public float jumpPower; //점프력
    Animator anim; //플레이어 애니메이션 
    PlayerInput playerInput; //플레이어가 받는 키보드 입력
    Rigidbody playerRigid; //물리작용
    public bool isJump; //점프가능, 점프중 확인
    public bool isDodge; //회피가능, 회피중 확인

    public GameObject nearObject; //플레이어 근처에 있는 아이템
    Inven inven; //인벤토리
    public Weapon equipWeapon; //장작중인 아이템
    public bool isSwap;//무기변경 가능여부
    int equipWeaponIndex = -1; //장착중인 무기 인덱스 기본값 -1 (무기를 장착중인 상태가 아님)

    [SerializeField] bool isFireReady = true;
    float fireDelay;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerRigid = GetComponent<Rigidbody>();
        inven = GetComponent<Inven>();
    }

    void Update()
    {
        Move();
        Jump();
        Attack();
        StartCoroutine("Dodge");
        GetItem();
        Swap();
    }

    void Move() //움직임 함수
    {
        //x축, z축, 대각선으로 이동할때 같은 속도를 유지하게함
        moveVec = new Vector3(playerInput.xAxis, 0, playerInput.zAxis).normalized;

        if (isDodge)//회피중인 상황이면
            moveVec = dodgeVec; //이동방향을 회피방향으로 고정
        else if (isSwap || !isFireReady)//무기 변경중인 상황이면
            moveVec = Vector3.zero; //움직임 멈춤

        //플레이어 위치는 이동하는 값과 스피드를 받아서 반형한다. 걷기 버튼 누르면 0.35배 속도 아니라면 원래 속도로
        transform.position += moveVec * speed * (playerInput.wDown ? 0.35f : 1f) * Time.deltaTime;

        //나아가는 방향을 바라보게 함
        transform.LookAt(transform.position + moveVec);

        //이동하는값이 0이 아니면 달리는 애니메이션 작동
        anim.SetBool("isRun", moveVec != Vector3.zero);
        //걷기 버튼 활성화시 걷는 애니메이션 작동
        anim.SetBool("isWalk", playerInput.wDown);

    }

    void Jump() //점프 함수
    {
        //점프키를 누르고 isJump와 isDodge 그리고 isSwap이 false면 실행
        if (playerInput.jDown && !isJump && !isDodge && !isSwap)
        {
            playerRigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse); //플레이어를 위쪽 방향으로 힘을 가해 점프시킴
            anim.SetTrigger("doJump"); //점프애니메이션 작동
            anim.SetBool("isJump", true); //애니메이션에 isJump를 활성화 시켜서 착지 애니메이션이 이어지게 만들어줌 
            isJump = true; //isJump를 활성화시켜 점프와 회피가 불가능하게 만듬

        }
    }

    void Attack()
    {
        if (equipWeapon == null) return; //장착된 무기가 없다면 실행 X

        fireDelay += Time.deltaTime; //공격 딜레이는 실제시간을 받아온다
        //공격 준비는 무기에 할당된 공격속도를 공격딜레이가 넘기면 활성화 
        isFireReady = equipWeapon.rate < fireDelay; 
        //공격버튼을 누를때 공격 준비가 되어있는 상황 + 회피와 무기 교체를 하고있지 않으면
        if (playerInput.fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use(); //무기 사용
            //무기 타입이 근접 무기라면 스윙 애니메이션 원거리 무기라면 샷 애니메이션 
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ?"doSwing" : "doShot"); 
            fireDelay = 0; //딜레이 수치를 0으로 만들어 공격 준비 상태를 비활성화 
        }
    }

    void Dodge() //회피 함수
    {
        // 움직이는 상황에서 회피키를 누르고 isDodge와 isSwap이 false면 실행
        if (playerInput.dDown && moveVec != Vector3.zero && !isDodge && !isSwap)
        {
            isDodge = true; //isDodge 활성화시켜 점프와 회피 연속사용을 막고 회피방향 고정시킴
            dodgeVec = moveVec; //회피 방향은 이동중인 방향 
            speed *= 2; //스피드 2배
            anim.SetTrigger("doDodge"); //회피 애니메이션 작동
            Invoke("Delay", 0.5f);//
            speed *= 0.5f; //회피시 2배로 만든 속도를 0.5를 곱해 원래 이동속도로 복구

        }
    }

    void GetItem() //아이템 획득 함수
    {
        //근처에 오브젝트가 있고 점프, 회피하는 상황이 아닐때 획득 키를 누르면  
        if (playerInput.gDown && nearObject != null && !isJump && !isDodge && !isSwap)
        {
            Item item = nearObject.GetComponent<Item>();



            if (nearObject.tag == "Weapon") //가까운 오브젝트 태그가 Weapon일때
            {
                int weaponIndex = item.value; //아이템이 가지고 있는 값을  무기 인덱스에 넣음
                inven.hasWeapons[weaponIndex] = true; //해당 값과 동일한 hasWeapons를 활성화
            }
            else if (nearObject.tag == "Item") //가까운 오브젝트 태그가 Item일때
            {
                switch (item.type)
                {
                    case Item.Type.Ammo:
                        inven.ammo += item.value; //인벤 ammo에 아이템 값만큼 수량을 추가함
                        if (inven.ammo > inven.maxAmmo) inven.ammo = inven.maxAmmo;
                        break;
                    case Item.Type.Coin:
                        inven.coin += item.value; //인벤 Coin에 아이템 값만큼 수량을 추가함
                        if (inven.coin > inven.maxCoin) inven.coin = inven.maxCoin;
                        break;
                    case Item.Type.Grenade:
                        inven.grenade += item.value; //인벤 Grenade에 아이템 값만큼 수량을 추가함
                        if (inven.grenade > inven.maxGrenade) inven.grenade = inven.maxGrenade;
                        break;
                    case Item.Type.Heart:
                        inven.heart += item.value; //인벤 Grenade에 아이템 값만큼 수량을 추가함
                        if (inven.heart > inven.maxHeart) inven.heart = inven.maxHeart;
                        break;
                    default:
                        break;
                }
            }
            Destroy(nearObject); //먹은 오브젝트 필드에서 파괴 

        }
    }

    void Swap() //무기 변경 함수
    {
        //특정 번호을 누를때 해당 무기가 없거나 이미 장착중이라면 아무것도 불러오지 않음  
        if (playerInput.sDown1 && (!inven.hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (playerInput.sDown2 && (!inven.hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (playerInput.sDown3 && (!inven.hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1; //무기인덱스 -1번
        if (playerInput.sDown1) weaponIndex = 0; //무기변경 버튼 1번을 누르면 무기인덱스를 0번으로 변경
        if (playerInput.sDown2) weaponIndex = 1; //무기변경 버튼 2번을 누르면 무기인덱스를 1번으로 변경
        if (playerInput.sDown3) weaponIndex = 2; //무기변경 버튼 3번을 누르면 무기인덱스를 2번으로 변경

        //특정 무기가 있어 특정 번호를 누를때 점프, 회피, 무기 변경중이 아니면
        if ((playerInput.sDown1 || playerInput.sDown2 || playerInput.sDown3) && !isJump && !isDodge && !isSwap)
        {
            if (equipWeapon != null) //장착중인 무기가 있다면
                equipWeapon.gameObject.SetActive(false); //현재 들고 있는 무기 비활성화

            equipWeaponIndex = weaponIndex; //장착할 무기인덱스는 번호를 눌렀을때 변경되는 무기인덱스를 넣어줌
            equipWeapon = inven.weapons[weaponIndex].GetComponent<Weapon>(); //장착할 무기는 인벤토리에서 무기인덱스 번호와 같은 것을 가져옴
            anim.SetTrigger("doSwap"); //무기 변경 애니메이션 실행
            equipWeapon.gameObject.SetActive(true); //장착한 무기 활성화
            isSwap = true; //무기 변경 불가능한 상태로 변경
            Invoke("Delay", 0.5f); //0.5초뒤 딜레이 함수에 있는 것을 불러옴

        }

    }

    void Delay() //딜레이 함수
    {
        isDodge = false; //회피가 가능한 상태로 변경
        isSwap = false; //무기 변경이 가능한 상태로 변경
    }

    private void OnTriggerStay(Collider other)
    {
        //플레이어가 무기 혹은 아이템 태그를 가진 오브젝트 범위안에 있다면
        if (other.tag == "Weapon" || other.tag == "Item")
        {
            nearObject = other.gameObject; //근처에 있는 아이템을 감지
            Debug.Log(nearObject); //어떤 아이템인지 기록을 엔진에 남겨줌
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //플레이어가 무기 혹은 아이템 태그를 가진 오브젝트 범위 안에서 벗어났다면
        if (other.tag == "Weapon" || other.tag == "Item")
        {
            nearObject = null; //플레이어 근처에 있는 아이템감지를 멈춤
        }
    }

    private void OnCollisionEnter(Collision collision) //콜라이더와 충돌하면
    {

        anim.SetBool("isJump", false); //애니메이션에 isJump를 활성화 시켜서 착지 애니메이션이 이어지게 만들어줌 
        if (collision.gameObject.tag == "Floor") //충돌한 오브젝트 테그가 Floor면  
        {
            isJump = false; //isJump를 false로 돌려 점프가 가능한 상태로 만듬
        }
    }


}
