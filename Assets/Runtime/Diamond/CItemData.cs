using UnityEngine;

[CreateAssetMenu
    (
    fileName = "ItemData",
    menuName = "Item/ItemData"
    )
]
public class CItemData : ScriptableObject
{
    [Header("식별ID")]
    [SerializeField] private string _itemID;

    [Header("아이템 정보")]
    [SerializeField] private string _itemName;
    [SerializeField] private Sprite _itemIcon;

    // 아이템 설명서
    [TextArea]
    [SerializeField] private string _description;

    [Header("아이템 효과")]
    [SerializeField] private float _healAmount = 30f;

    public string ItemID => _itemID;
    public string ItemName => _itemName;
    public Sprite ItemIcon => _itemIcon;
    public string Description => _description;
    public float HealAmount => _healAmount;
}