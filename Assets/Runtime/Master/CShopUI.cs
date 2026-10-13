using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CShopUI : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private CInventory _inventory;
    [SerializeField] private CPlayerWallet _wallet;

    [Header("패널")]
    [SerializeField] private GameObject _shopPanel;

    [Header("목록")]
    [SerializeField] private Transform _content;
    [SerializeField] private CShopSlotUI _slotPrefab;

    [Header("정보")]
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TMP_Text _itemNameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private TMP_Text _feedbackText;
    [SerializeField] private Button _buyButton;

    #endregion

    #region 내부변수
    private CShop _currentShop;
    private CShopProductData _selectedProduct;
    private bool _isBuying;

    #endregion

    #region 프로퍼티
    public bool IsOpen => _shopPanel.activeSelf;

    #endregion

    private void Start()
    {
        _shopPanel.SetActive(false);
        ClearInfo();
    }

    public void Open(CShop shop)
    {
        if (shop == null)
        {
            return;
        }

        _currentShop = shop;
        _shopPanel.SetActive(true);
        RefreshProducts();
    }

    public void Close()
    {
        _shopPanel.SetActive(false);
        _selectedProduct = null;
        ClearInfo();
    }

    public void RefreshProducts()
    {
        for (int i = _content.childCount - 1; i >= 0; --i)
        {
            Destroy(_content.GetChild(i).gameObject);
        }

        for (int i = 0; i < _currentShop.Products.Count; ++i)
        {
            CShopSlotUI slot = Instantiate(_slotPrefab, _content);
            slot.Setup(_currentShop.Products[i], this);
        }
    }

    public void SelectProduct(CShopProductData productData)
    {
        _selectedProduct = productData;
        ShowInfo(productData);
    }

    private void ShowInfo(CShopProductData productData)
    {
        _itemIcon.enabled = true;
        _itemIcon.sprite = productData.ItemData.ItemIcon;
        _itemNameText.text = productData.ItemData.ItemName;
        _descriptionText.text = productData.ItemData.Description;
        _priceText.text = $"Price : {productData.Price} Coin";
        _feedbackText.text = "";
        _buyButton.interactable = true;
    }

    public void OnClickBuyButton()
    {
        if (_isBuying)
        {
            return;
        }

        if (_selectedProduct == null)
        {
            return;
        }

        _isBuying = true;

        TryBuy(_selectedProduct);

        _isBuying = false;
    }

    private void TryBuy(CShopProductData productData)
    {
        if (!_wallet.CanSpend(productData.Price))
        {
            _feedbackText.text = "Not Enough Coin.";
            CPrint.Log("Buy Fail : Not Enough Coin.");
            return;
        }

        if (!_wallet.TrySpend(productData.Price))
        {
            _feedbackText.text = "Buy Fail.";
            return;
        }

        _inventory.AddItem(productData.ItemData);

        _feedbackText.text = $"{productData.ItemData.ItemName} Buy Complete!";
        CPrint.Log($"{productData.ItemData.ItemName} Buy Complete");
    }

    private void ClearInfo()
    {
        _itemIcon.enabled = false;
        _itemNameText.text = "";
        _descriptionText.text = "";
        _priceText.text = "";
        _feedbackText.text = "";
        _buyButton.interactable = false;
    }
}

