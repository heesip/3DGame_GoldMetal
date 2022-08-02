using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inven : MonoBehaviour
{
    public GameObject[] weapons; //장착 가능한 무기
    public bool[] hasWeapons; //가지고 있는 무기
    public GameObject[] grenades; //

    public int ammo;    //총알
    public int coin;    //동전 
    public int grenade; //수류탄
    public int heart;   //회복약

    public int maxAmmo = 999;   //총알 최대치
    public int maxCoin = 99999; //동전 최대치
    public int maxGrenade = 9;  //수류탄 최대치  
    public int maxHeart = 9;    //회복약 최대치  
}
