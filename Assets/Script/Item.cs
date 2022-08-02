using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //열거형(아이템 타입지정) 
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };  //타입(총알, 동전, 수류탄, 하트, 무기)
    public Type type; //타입을 엔진에서 지정

    public int value; //아이템 번호 혹은 아이템 수량

    private void Update()
    {
        transform.Rotate(Vector3.up * 25 * Time.deltaTime); //회전
    }
}
