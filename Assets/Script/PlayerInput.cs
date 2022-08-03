using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float xAxis; //방향키 좌우 입력시 값을 받는 변수
    public float zAxis; //방향키 상하 입력시 값을 받는 변수
    public bool wDown; //걷기 버튼 입력 값 
    public bool jDown; //점프 버튼 입력 값
    public bool dDown; //회피 버튼 입력 값
    public bool gDown; //획득 버튼 입력 값
    public bool sDown1; //스왑1 버튼 입력 값
    public bool sDown2; //스왑2 버튼 입력 값
    public bool sDown3; //스왑3 버튼 입력 값
    public bool fDown; //공격 버튼 입력 값
    public bool rDown; //장전 버튼 입력 값
    public bool f2Down; //공격2 버튼 입력 값



    // Update is called once per frame
    void Update()
    {
        xAxis = Input.GetAxisRaw("Horizontal"); //방향키 좌 -1, 우 1 
        zAxis = Input.GetAxisRaw("Vertical"); //방향키 상 1, 하 -1
        wDown = Input.GetButton("Walk"); //걷기 버튼 누르면 활성화
        jDown = Input.GetButtonDown("Jump"); //점프 버튼 누르면 활성화
        dDown = Input.GetButtonDown("Dodge"); //회피 버튼 누르면 활성화
        gDown = Input.GetButtonDown("Get"); //획득 버튼 누르면 활성화
        sDown1 = Input.GetButtonDown("Swap1"); //스왑1 버튼 누르면 활성화
        sDown2 = Input.GetButtonDown("Swap2"); //스왑2 버튼 누르면 활성화
        sDown3 = Input.GetButtonDown("Swap3"); //스왑3 버튼 누르면 활성화
        fDown = Input.GetButton("Fire1"); //공격 버튼 누르면 활성화
        rDown = Input.GetButton("Reload"); //장전 버튼 누르면 활성화
        f2Down = Input.GetButtonDown("Fire2"); //공격2 버튼 누르면 활성화

    }
}
