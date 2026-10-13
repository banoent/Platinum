using System;
using UnityEngine;

public class CPlayerWallet : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private int _coin;

    #endregion

    #region 프로퍼티
    public int Coin => _coin;

    #endregion

    #region 내부변수
    public event Action OnCoinChanged;

    #endregion


    public void AddCoin(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        _coin += amount;
        OnCoinChanged?.Invoke();

        CPrint.Log($"코인 획득 : +{amount} / 현재 코인 : {_coin}");
    }

    // 사용 가능 유무 확인
    public bool CanSpend(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        return _coin >= amount;
    }

    // 상점에서 코인 사용 시도
    public bool TrySpend(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (_coin < amount)
        {
            return false;
        }

        _coin -= amount;
        OnCoinChanged?.Invoke();

        CPrint.Log($"코인 사용 : -{amount} / 현재 코인 : {_coin}");

        return true;
    }





    
}
