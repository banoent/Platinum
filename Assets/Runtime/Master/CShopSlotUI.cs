using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CShopSlotUI : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private Button _button;

    #endregion

    #region  내부변수
    private CShopProductData _productData;
    private CShopUI _shopUI;

    #endregion

    public void Setup(CShopProductData productData, CShopUI shopUI)
    {
        _productData = productData;
        _shopUI = shopUI;

        _icon.sprite = productData.ItemData.ItemIcon;
        _nameText.text = productData.ItemData.ItemName;
        _priceText.text = $"{productData.Price} Coin";

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClickSlot);
    }

    private void OnClickSlot()
    {
        _shopUI.SelectProduct(_productData);
    }
}