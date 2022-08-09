using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float angularPower = 2; //회전력
    float scaleValue = 0.1f; //크기
    bool isShoot; 
    
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());

    }

    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f); //2.2초후 
        isShoot = true; //isShoot을 true로 돌림
        yield return new WaitForSeconds(0.5f); //0.2초후 
        isShoot = false; //isShoot을 true로 돌림

    }

    IEnumerator GainPower() //게이지를 모으는 함수
    {
        while (!isShoot) //isShot이 false면 
        {
            angularPower += 0.02f; //회전력을 0.02씩 더함
            scaleValue += 0.005f; //크기를 0.005씩 키움
            transform.localScale = Vector3.one * scaleValue; //점점 커지는 크기 적용
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration); //회전력에 맞게 회전
            yield return null;
        }
    }
}
