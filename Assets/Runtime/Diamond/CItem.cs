using UnityEngine;

public class CItem : MonoBehaviour
{
    #region ¿ŒΩ∫∆Â≈Õ
    [SerializeField] private CItemData _itemData;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        CInventory inventory = other.GetComponent<CInventory>();

        if (inventory == null)
        {
            return;
        }

        inventory.AddItem(_itemData);

        Destroy(gameObject);
    }
}