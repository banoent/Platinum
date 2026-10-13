using UnityEngine;

public class CPickUpCoin : MonoBehaviour
{
    #region ¿ŒΩ∫∆Â≈Õ
    [SerializeField] private int _amount = 10;
    #endregion
    private void OnTriggerEnter(Collider other)
    {
        CPlayerWallet wallet = other.GetComponent<CPlayerWallet>();

        if (wallet == null)
        {
            return;
        }

        wallet.AddCoin(_amount);
        Destroy(gameObject);
    }
}
