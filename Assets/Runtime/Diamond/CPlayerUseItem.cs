using UnityEngine;

public class CPlayerUseItem : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private CPlayer _player;
    [SerializeField] private CInventory _inventory;

    #endregion


    public bool UseItem(CItemData itemData)
    {
        if (itemData == null)
        {
            return false;
        }


        // 체력이 최대라면 사용하지 않음
        if (!_player.CanHeal())
        {
            CPrint.Log("체력이 최대이므로 포션을 사용할 수 없습니다.");

            return false;
        }

        // 실제 아이템이 있는지 검사
        CInventory.CInventoryItem item = _inventory.FindItem(itemData.ItemID);

        if (item == null)
        {
            return false;
        }

        // 플레이어 회복
        _player.Heal(itemData.HealAmount);

        // 아이템 수량 감소
        _inventory.RemoveItem(itemData);

        CPrint.Log($"{itemData.ItemName} 사용");
        return true;
    }
}