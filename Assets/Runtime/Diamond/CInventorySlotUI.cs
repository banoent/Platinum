using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CInventorySlotUI : MonoBehaviour
{
    #region 인스펙터

    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private Button _button;

    #endregion


    #region 내부변수

    private CItemData _itemData;
    private CInventoryUI _inventoryUI;

    #endregion


    public void Setup(CInventory.CInventoryItem item, CInventoryUI inventoryUI)
    {
        _itemData = item.ItemData;
        _inventoryUI = inventoryUI;
        _icon.sprite = item.ItemData.ItemIcon;
        _nameText.text = item.ItemData.ItemName;
        _countText.text = $"x {item.Count}";

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClickSlot);
    }


    private void OnClickSlot()
    {
        if (_itemData == null)
        {
            return;
        }

        _inventoryUI.SelectItem(_itemData);
    }
}