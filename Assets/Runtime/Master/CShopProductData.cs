using UnityEngine;

[CreateAssetMenu(fileName = "ShopProductData", menuName = "Shop/ShopProductData")]
public class CShopProductData : ScriptableObject
{
    [SerializeField] private string _productID;
    [SerializeField] private CItemData _itemData;
    [SerializeField] private int _price = 10;

    public string ProductID => _productID;
    public CItemData ItemData => _itemData;
    public int Price => _price;
}
