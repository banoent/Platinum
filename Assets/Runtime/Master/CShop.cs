using System.Collections.Generic;
using UnityEngine;

public class CShop : MonoBehaviour
{
    [SerializeField] private List<CShopProductData> _products = new List<CShopProductData>();

    public List<CShopProductData> Products => _products;
}
