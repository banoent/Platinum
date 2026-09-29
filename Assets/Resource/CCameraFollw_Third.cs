using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 오빗
/*
▶ 오빗

- 궤도 → 게임 / 그래픽스 → 대상을 중심으로 빙글빙글 도는 카메라를 의미한다.

- 핵심 개념은 2가지
 ㄴ 1. 중심 (Target) : 카메라가 따라다니는 기준 대상 → 보통 플레이어
 ㄴ 2. 반지름 (Distance) : 중심과 카메라 사이의 거리 (카메라가 도는 원의 크기)
 
- 오빗 카메라는 카메라가 움직인다기 보다 → 중심점을 기준으로 카메라의 각도 (Yaw / Pitch)가 변한다고 이해하면 좋다.
 ㄴ Yaw		: 좌 / 우 (마우스 X) → 대상 주위를 수평으로 회전	
 ㄴ Pitch	: 상 / 하 (마우스 Y) → 대상 주위를 수직으로 회전 (+ 위 / 아래 올려보기 등등)

- 오빗은 카메라가 목표를 향해 단순히 LookAt만 하는 것이 아닌
 ㄴ 바라보는 방향만 맞춰주고
 ㄴ 오빗은 위치가 실제로 원형으로 이동하는 카메라 동작처럼 구현하는 개념



※일반적인 사용 공식

1. Yaw / Pitch 각도 누적
 ㄴ 입력

2. 거리 유지
 ㄴ + 줌 인 / 줌 아웃

3. 최종 위치 계산
 ㄴ 각도 + 거리로 카메라 위치 산출 → LookAt



*/
#endregion

public partial class CCameraFollwBasic : MonoBehaviour
{
	#region 인스펙터
	[Header("3인칭 오빗")]
	[SerializeField] private Vector3 _thirdOffset = new Vector3(0f, 3f, -3f);
	[SerializeField] private float _thirdLookAtHeight = 1.5f;
	[Min(0f)]
	[SerializeField] private float _thirdSharpness = 18f;

	[Header("3인칭 옵션")]
	[SerializeField] private bool _thirdUseOrbit = true;
	// 마우스로 카메라 회전 기능 On / Off
	// True → 마우스로 카메라가 타겟 주의를 돈다.
	// False → 고정된 3인칭

	[SerializeField] private float _orbitSensitivity = 3.0f;
    // 얼마나 각도에 반영할 지


    // 제한
    [SerializeField] private float _orbitPitchMin = -10.0f;
    [SerializeField] private float _orbitPitchMax = 25.0f;

	#endregion

	#region 내부 변수
	private float _orbitYaw;	// 좌우 회전 (방향 전환)
	private float _orbitPitch;	// 상하 회전 (올려보기 / 내려보기)


	#endregion


	// 3인칭 모드 진입 초기화
	private void InitThird(bool snap)
	{
		// 오빗 → Yaw / Pitch라는 누적 값을 기준으로 카메라 위치가 결정이 된다.

		// 타겟의 Y회전을 기준으로 시작한다.
		//  ㄴ 타겟이 이미 바라보는 방향이 있응면 → 그 방향을 Yaw 시작값으로 쓰면 → 플레이어 뒤쪽에서 자연스럽게 시작이 된다.

		_orbitYaw = _target.eulerAngles.y;
		_orbitPitch = 12.0f;

		Vector3 desiredPos;
		Quaternion desiredRot;

		BuildThirdPose(out desiredPos, out desiredRot);
		ApplyPose(desiredPos, desiredRot, _thirdSharpness, snap);
		// 차후 → ISO → 웬만하면 유니티 딸깍으로 사용하자;;
		//	 ㄴ 생각보다 유니티에서 지원하는 기능이 미비..

		// ㅁㅡㅁ 
		// ㅣ  ㅣ
		// ㅁㅡㅁ
		//
		

    }

	private void TickThird()
	{
		// 매 프레임 업데이트
		//  ㄴ 목표 포즈 계산
		//  ㄴ 따라 붙는다.

		Vector3 desiredPos;
        Quaternion desiredRot;

		// 실제 카메라가 흔들리지 않고 부드럽게 따라오는 느낌은 ApplayPose에서 만들어지고
		// 즉, 어디로 갈 것인가를 계산
		// 어디로 가야하는지는 BuildThirdPose()에서 결정된다.

        BuildThirdPose(out desiredPos, out desiredRot);

		// 어떻게 갈 것인가 결정 (snap , Smooth)
		ApplyPose(desiredPos, desiredRot, _thirdSharpness, false);

		if (_drawDebug)
		{
			// 카메라 정면 확인
			CPrint.Ray(_camTr.position, _camTr.forward * 2.0f, Color.red);
		}
    }

	// 오빗의 핵심 : 목표 위치 / 회전 만든다.
	private void BuildThirdPose(out Vector3 desiredPos, out Quaternion desiredRot)
	{
		// 크게 3파트
		// ㄴ 오빗 입력 / 위치 계산 (오빗 ON) / 기본 3인칭 팔로우

		// 입력 처리
		if (_mode == ECameraMode.ThirdPerson && _thirdUseOrbit && Input.GetMouseButton(1))
		{
			float mx = Input.GetAxis("Mouse X"); // Yaw 누적
			float my = Input.GetAxis("Mouse Y"); // Pitch 누적

			_orbitYaw += mx * _orbitSensitivity;
			_orbitPitch -= my * _orbitSensitivity;

			_orbitPitch = Mathf.Clamp(_orbitPitch, _orbitPitchMin, _orbitPitchMax);
			
		}

		// 위치 계산 오빗 On인 경우
		if (_thirdUseOrbit)
		{
			// 오빗의 핵심 → 카메라를 돌리기 보다 → offset 벡터를 회전시켜 타겟 주의를 공전 시킨다.
			// ㄴ 카메라가 타겟을 중심으로 공전해야함 → 카메라를 돌리는 것이 아닌 카메라가 놓인 테이블을 돌린다.

			// Yaw / Pitch를 쿼터니언 회전을 만든다.
			Quaternion orbitRot = Quaternion.Euler(_orbitPitch, _orbitYaw, 0f);


			desiredPos = _target.position + (orbitRot * _thirdOffset);

			// 카메라가 바라볼 목표 위치
			// 회전은 LookAt으로 고정 → 항상 타겟을 바라보게
			//  ㄴ _thirdLookAtHeight 없는 경우에는 시선이 너무 아래로 향할 수도 있기 때문
			Vector3 LookPos = _target.position + Vector3.up * _thirdLookAtHeight;

            // 해당 방향 바라보는 쿼터니언 생성
            desiredRot = Quaternion.LookRotation(LookPos - desiredPos, Vector3.up);
		}
		else
		{
			// 오빗을 끄면 : 타겟 회전 기준으로 뒤에서 따라오는 기본 3인칭
			//  ㄴ 타겟 회전에 붙어서 항상 뒤에서 따라오는 카메라가 된다.

			desiredPos = _target.position + (_target.rotation * _thirdOffset);

			Vector3 lookPos = _target.position + Vector3.up * _thirdLookAtHeight;
			desiredRot = Quaternion.LookRotation(lookPos - desiredPos, Vector3.up);
		}


	}
}
