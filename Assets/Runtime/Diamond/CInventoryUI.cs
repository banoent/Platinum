using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CInventoryUI : MonoBehaviour
{
    #region 인스펙터

    [Header("인벤토리")]
    [SerializeField] private CInventory _inventory;
    [SerializeField] private CPlayerUseItem _playerUseItem;

    [Header("패널")]
    [SerializeField] private GameObject _inventoryPanel;

    [Header("아이템 목록")]
    [SerializeField] private Transform _itemContent;
    [SerializeField] private CInventorySlotUI _slotPrefab;

    [Header("선택 아이템 정보")]
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TMP_Text _itemNameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private Button _useButton;

    #endregion


    #region 내부변수
    private CItemData _selectedItem;

    #endregion


    private void Start()
    {
        _inventoryPanel.SetActive(false);

        ClearItemInfo();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }
    public void ToggleInventory()
    {
        bool isOpen = !_inventoryPanel.activeSelf;
        _inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            RefreshInventory();
        }
    }
    public void RefreshInventory()
    {
        CPrint.Log($"[InventoryUI] Refresh / 아이템 종류 : {_inventory.Items.Count}");
        // 기존 슬롯 삭제
        for (int i = _itemContent.childCount - 1; i >= 0; --i)
        {
            Destroy(_itemContent.GetChild(i).gameObject);
        }


        // 현재 Inventory 내용을 가지고 슬롯 생성
        for (int i = 0; i < _inventory.Items.Count; ++i)
        {
            CPrint.Log
        (
            $"슬롯 생성 : {_inventory.Items[i].ItemData.ItemName}" +
            $" X {_inventory.Items[i].Count}"
        );

            CInventorySlotUI slot = Instantiate
                (
                    _slotPrefab,
                    _itemContent
                );

            slot.Setup(_inventory.Items[i], this);
        }

        // 선택 아이템이 있다면 정보 갱신
        if (_selectedItem != null)
        {
            ShowItemInfo(_selectedItem);
        }
    }


    public void SelectItem(CItemData itemData)
    {
        _selectedItem = itemData;
        ShowItemInfo(itemData);
    }


    private void ShowItemInfo(CItemData itemData)
    {
        if (itemData == null)
        {
            ClearItemInfo();

            return;
        }

        CInventory.CInventoryItem inventoryItem = _inventory.FindItem(itemData.ItemID);

        // 수량이 0이 되어 사라진 경우
        if (inventoryItem == null)
        {
            _selectedItem = null;

            ClearItemInfo();

            return;
        }

        _itemIcon.enabled = true;
        _itemIcon.sprite = itemData.ItemIcon;
        _itemNameText.text = itemData.ItemName;
        _descriptionText.text = itemData.Description;
        _countText.text = $"Item Count : {inventoryItem.Count}";

        _useButton.interactable = true;
    }

    // 클릭으로 아이템 사용
    public void OnClickUseButton()
    {
        if (_selectedItem == null)
        {
            return;
        }

        bool result = _playerUseItem.UseItem(_selectedItem);

        if (!result)
        {
            return;
        }

        RefreshInventory();
    }


    // 아이템 정보 리셋
    private void ClearItemInfo()
    {
        _itemIcon.enabled = false;
        _itemIcon.sprite = null;
        _itemNameText.text = "";
        _descriptionText.text = "";
        _countText.text = "";
        _useButton.interactable = false;
    }
}