using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#region partial
/*
▶ partial

- 하나의 클래스를 여러 파일로 나눠 작성할 수 있게 하는 키워드

- 파일은 여러개지만 논리적으로 하나의 클래스를 사용할 때 사용할 수 있음
 ㄴ 컴파일 될 때 하나의 클래스로 합쳐진다.

장점 

- 기능별 분리에 유리

- 관련 기능을 관리하기 좋다

- 변수 + 함수를 서로 공유 가능

단점

- 구조가 커지면 코드 리딩 부분에서 불리함

- 책임 자체를 나누는 것이 아니기 때문에
 ㄴ 클래스 분리는 적절할 때 활용을 해야 한다.

※ partial : 큰 클래스의 파일 정리

※ 별도 class 분리 : 책임 분리

ㆍ유니티 관점

- 유니티 입장에서는 하나의 MonoBehaviour 클래스

- 메세지 함수 주의
 ㄴ 분할해둔 파일에 메세지 함수를 당연히 여러 번 사용할 수 없다.


*/
#endregion


public partial class CCameraFollwBasic : MonoBehaviour
{
    // 공통 흐름 / 스무딩 처리
    // 빌드 : 어디로 갈지
    // 적용 : 어떻게 갈지
    // 틱 계산 : 언제 갱신이 될지?

    public enum ECameraMode
    {
        ThirdPerson,
        FirstPerson,
        QuaterView
    }
    #region 인스펙터
    [Header("필수 연결")]
    [SerializeField] private Transform _target;
    [SerializeField] private Camera _camera;

    [Header("모드 토글 키")]
    [SerializeField] private KeyCode _keyThird = KeyCode.Alpha1;
    [SerializeField] private KeyCode _keyFirst = KeyCode.Alpha2;
    [SerializeField] private KeyCode _keyQuarter = KeyCode.Alpha3;

    [Header("모드 토글 키")]
    [SerializeField] private ECameraMode _startMode = ECameraMode.ThirdPerson;
    [SerializeField] private bool _snapOnModeChange = true;

    [Header("디버그")]
    [SerializeField] private bool _drawDebug = true;


    #endregion

    #region 내부변수
    private Transform _camTr;
    private ECameraMode _mode;


    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if(_camera == null)
        {
            GameObject mainCamGO = GameObject.FindGameObjectWithTag("MainCamera");

            _camera = Camera.main;

            // _camera = Camera.main;
            // GetComponent

            if (mainCamGO != null)
            {
                _camera = mainCamGO.GetComponent<Camera>();
            }
        }

        if(_target  == null || _camera == null)
        {

            enabled = false;
            return;
        }

        // 캐싱 작업
        _camTr = _camera.transform;



        SetMode(_startMode, true);
    }

    // Update is called once per frame
    void Update()
    {
        // 키 입력만 처리하면 된다.

        if (Input.GetKeyDown(_keyThird))
        {
            SetMode(ECameraMode.ThirdPerson, _snapOnModeChange);

        }

        if (Input.GetKeyDown(_keyFirst))
        {
            SetMode(ECameraMode.FirstPerson, _snapOnModeChange);
        }

        if (Input.GetKeyDown(_keyQuarter))
        {
            SetMode(ECameraMode.QuaterView, _snapOnModeChange);
            CPrint.Warn("쿼터뷰 변경");
        }
    }


    private void LateUpdate()
    {
        
        if(_target == null || _camTr == null)
        {
            return;
        }

        switch (_mode)
        {
            // 여기는 사실상 다른 파일에서 작성이 되고 넘어와야 하는 함수
            case ECameraMode.ThirdPerson:
                TickThird();
                break;
            case ECameraMode.FirstPerson:
                TickFirst();
                break;
            case ECameraMode.QuaterView:
                TickQuarter();
                break;
           
        }

        if (_drawDebug)
        {
            // 카메라 - 타겟 관계 확인 용도
            CPrint.Line3D(_camTr.position, _target.position, Color.yellow);
            
        }

    }

    private void SetMode(ECameraMode mode, bool snap)
    {
        // snap : t → 즉시 카메라가 있어야 할 포즈로 이동 (즉각)
        // f → 현재 카메라에서 새로운 포즈로 부드럽게 이동 (자연스럽게)

        _mode = mode;

        CPrint.Section($"모드 → {_mode}");

        switch (_mode)
        {
            case ECameraMode.ThirdPerson:
                InitThird(snap);
                
                break;
            case ECameraMode.FirstPerson:
                InitFirst(snap);
                
                break;
            case ECameraMode.QuaterView:
                initQuarter(snap);
                break;
            
        }
    }


    private float GetSmoothT(float sharpness)
    {
        /*
        sharpness = 2
         ㄴ 느리게 끌려오듯 따라온다

        sharpness = 15
         ㄴ 빠르게 따라 붙는다.

        sharpness = 30
         ㄴ 거의 바로 붙는 느낌 → 부드러움 → 반응성을 보기 붙이는 케이스
        */
        return 1f - Mathf.Exp(-sharpness * Time.deltaTime);
    }
    /*
    - 보간 → 지수 보간용 t 만들고 싶다.

    Ex :

    - 카메라 / 플레이어 사이에 거리가 10M 남았다.
     ㄴ 남은 거리의 절반을 이동하면 → 5M
     ㄴ 다시 이동하면 2.5 M / 다시 절반이면 1.25M

    ※ 계속 가까워지면 가까워질 수록 움직이는 거리도 작아진다.

    - 이번 프레임에서 → 현재위치 →  목표 위치까지 몇 %정도로 따라갈 것인가?
     ㄴ 이걸 나타내는 t를 만드는게 목적

    Mathf.Exp

    - e^x
     ㄴ x가 커질수록 값이 빠르게 증가

    - e^-x 
     ㄴ x가 커질수록 값이 빠르게 감소

    ※ x가 음수 방향으로 커질수록 Exp의 결과가 1 → 0에 가까워진다


    ㆍ 1f - Mathf.Exp(-sharpness * Time.deltaTime);
    - 해석을 2개로 하면 된다.


    1. Mathf.Exp(-sharpness * Time.deltaTime);
     ㄴ 이번 프레임이 끝난 뒤 → 기존 차이가 얼마나 남아있을 것인가?

    2. 1f - Mathf.Exp(-sharpness * Time.deltaTime);
     ㄴ 그렇다면 이번 프레임에 목표쪽으로 얼마나 이동할 것인가?

    Exp : 남겨둘 비율

    1 - Exp 결과 : 따라갈 비율

    - Exp 결과가 0.9가 나왔다.
     ㄴ 기존 차이를 90% 남겼다.
     ㄴ 이 말은 이번 프레임에 10%만 이동한다.

    - Sharpness 자체가 목표에 얼마나 빠르게 붙을 것인지를 결정한다.
     ㄴ 이 값이 증가 → 더 작은 음수가 되고
     ㄴ 커지면 -1 / -2 / -3

    ㆍ지수식 
    - Exp(-k * deltatime) → e^(-k * deltat)


    */


    private void ApplyPose(Vector3 desiredPos, Quaternion desiredRot, float sharpness, bool snap)
    {
        // desiredPos   : 카메라가 가야하는 목표 위치
        // desiredRot   : 카메라가 가야하는 목표 회전
        // sharpness    : 목표로 얼마나 빠르게 따라갈지 (스무딩 강도)
        // snap         : 즉시 / 블렌딩 시킬건지

        // 즉시 → 씬 시작 / 리스폰 / 텔레포트 같은거
        if (snap)
        {
            _camTr.position = desiredPos;
            _camTr.rotation = desiredRot;

            return;
        }

        // bool snap false시 → 부드럽게 따라간다.
        float t = GetSmoothT(sharpness);

        // 위치 스무딩
        // 현재 위치에서 → 목표 위치로 → t비율만큼 이동
        _camTr.position = Vector3.Lerp(_camTr.position, desiredPos, t);

        // 회전 스무딩
        //  ㄴ 효율보다는 안정성
        _camTr.rotation = Quaternion.Slerp(_camTr.rotation, desiredRot, t);

        /*

        ㆍQuaternion.Slerp(시작점, 끝점, )
        - 구면 (선형) 보간
        - 두 쿼터니언 회전 사이를 회전 공간의 곡면을 따라 보간한다.

        t = 0
         ㄴ 시작 회전

        t = 0.5
         ㄴ 시작과 목표 사이의 중간회전

        t = 1
         ㄴ 목표 회전

        - 회전은 단순한 선형 운동이 아니기에 → 각도를 고려해야 하는 구형 운동이 유리한 경우가 많다.

        Ex : 지구본..?

        - 서울 → 미국
         ㄴ Lerp → 뚫고 간다
            ㄴ 계산이 빠르고 단순 / 작은 각도의 회전에서는 Slerp와 큰 차이를 인지하기 힘들다

         ㄴ Slerp → 지구 표면에 따라 이동
            ㄴ 큰 각도 차이에서는 자연스러운 회전 결과를 얻기가 좋다.
            ㄴ 같은 시각 회전과 목표회전을 고정한 상태에서 t를 0 → 1로 일정하게 증가 시키면 회전 각도를 기준으로 일정한 비율로 진행되는 보간을 만들 수가 있다.



        Slerp : 현재 회전과 목표 회전 사이에서 어느 회전을 선택할 것인가?

        GetSmoothT : 이번 프레임에 목표쪽으로 얼마나 따라갈 것인가?

        - 2개를 합치면 → FPS에 영향을 받지 않으면서 폭표 회전을 부드럽게 따라가는 카메라
        */
    }
}
