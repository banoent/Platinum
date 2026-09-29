using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 쿼터뷰
/*
▶ 쿼터뷰

- 플레이어 게임 공간을 위쪽 / 대각선 방향에서 내려다 보는 시점

ㆍ특징

카메라가 타겟보다 높은 위치에 있다.
 ㄴ Y offset 사용
 ㄴ

- 카메라 방향을 고정해서 사용하는 경우가 많다.
 ㄴ 플레이어가 회전을 해도

※ 주변 공간 파악이 용이하다.

ㆍ3인칭과 비교

3인칭
 desiredPos = _player.position + (orbitRot * _thirdOffset);

쿼터뷰
desiredPos = _target.position + _quarterOffset;
 ㄴ 플레이어가 회전해도 카메라는 월드 기준 같은 방향을 유지한다.

※ 프로젝션을 수행할 때 원근 / 직교 모두 사용할 수 있다.

*/
#endregion

public partial class CCameraFollwBasic : MonoBehaviour
{
    #region 인스펙터
    [Header("쿼터뷰")]
    [SerializeField] private Vector3 _quaterOffset = new Vector3(8f, 10f, -8f);
    [SerializeField] private float _quarterLookAtHeight = 1.0f;
    [Min(0f)]
    [SerializeField] private float _quarterSharpness = 10.0f;

    #endregion

    private void initQuarter(bool snap)
    {
        Vector3 desiredPos;
        Quaternion desiredRot;

        BuildQuarterPose(out desiredPos, out desiredRot);
        ApplyPose(desiredPos, desiredRot, _quarterSharpness, true);
    }

    private void TickQuarter()
    {
        Vector3 desiredPos;
        Quaternion desiredRot;

        BuildQuarterPose(out desiredPos, out desiredRot);
        ApplyPose(desiredPos, desiredRot, _quarterSharpness, true);
    }

    private void BuildQuarterPose(out Vector3 desiredPos, out Quaternion desiredRot)
    {

        // 타겟 기준 월드 고정 오프셋으로 카메라 위치 계산
        desiredPos = _target.position + _quaterOffset;

        // 카메라가 바라볼 목표 위치
        Vector3 lookPos = _target.position + Vector3.up * _quarterLookAtHeight;

        desiredRot = Quaternion.LookRotation(lookPos - desiredPos, Vector3.up);
    }

    // Start is called before the first frame update

}
