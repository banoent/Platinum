using TMPro;
using UnityEngine;

public class CWalletUI : MonoBehaviour
{
    #region ¿ŒΩ∫∆Â≈Õ
    [SerializeField] private CPlayerWallet _wallet;
    [SerializeField] private TMP_Text _coinText;
    #endregion

    private void OnEnable()
    {
        if (_wallet != null)
        {
            _wallet.OnCoinChanged += Refresh;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (_wallet != null)
        {
            _wallet.OnCoinChanged -= Refresh;
        }
    }

    public void Refresh()
    {
        if (_wallet == null || _coinText == null)
        {
            return;
        }

        _coinText.text = $"Coin : {_wallet.Coin}";
    }
}
