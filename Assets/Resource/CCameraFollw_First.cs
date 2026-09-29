using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CCameraFollwBasic : MonoBehaviour
{
	#region 인스펙터
	[Header("1인칭")]
	[SerializeField] private Vector3 _firstOffset = new Vector3(0f, 1.6f, 0.1f);
	[Min(0f)]
	[SerializeField] private float _firstSharpness = 20f;
	[Header("1인칭 옵션")]
	[SerializeField] private bool _firstUseTargetRotation = true;
	// t : tartget의 회전을 카메라 회전으로 사용
	// f : 현재 카메라 회전을 그대로 유지

	#endregion

	// Init / Tick / BuildFirstPose

	// 시작시 1번 정렬
	// 1인칭 모드 → 처음 진입 시 → 카메라가 팅기지 않게 목표 위치 / 회전에 즉시 스냅시키는 용도
	private void InitFirst(bool snap)
    {
		Vector3 desirePos;
		Quaternion desireRot;

		BuildFirstPose(out desirePos, out desireRot);

		ApplyPose(desirePos, desireRot, _firstSharpness, snap);

		
    }

	private void TickFirst()
	{
		// 매 프레임 따라가기
		// 게임이 돌아가면 → 목표 포즈 계산 → 따라 붙는다.
        Vector3 desirePos;
        Quaternion desireRot;

		BuildFirstPose(out desirePos, out desireRot);

		// 1인칭 → 스무딩을 넣어주는게 좋긴하다 → 모델 / 애니메이션에 따라 카메라가 미세하게 튀는 걸 완화할 수 있다.
		ApplyPose(desirePos, desireRot, _firstSharpness, false);
    }
	
	private void BuildFirstPose(out Vector3 desirePos, out Quaternion desireRot)
	{
		// 목표 카메라 포즈 계산
		// 함수에서 값을 2개 만들어서 돌려주고 사용

		// 1인칭 → 보통 플레이어 기준으로 카메라가 몸에 붙는다.
		// _firstOffset → 타겟 기준의 로컬 오프셋

		desirePos = _target.position + (_target.rotation * _firstOffset);
		// _target.rotation * _firstOffset → 타겟이 바라보는 방향에 맞춰 Offset Vector도 함께 회전

		// True : 플레이어 회전 →  카메라 회전
		//  ㄴ 몸이 도는 방향이 곧 시점 방향 (가장 일반적인 방식)

		if (_firstUseTargetRotation)
		{
			desireRot = _target.rotation;
		}
		else // False : 카메라 회전을 고전하거나 / LookAt 같은 다른 방식으로 변형할 여지를 위해 남겨둔다.
		{
			desireRot = _target.rotation;
		}





        }


}
