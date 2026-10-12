using System;
using System.Collections.Generic;
using UnityEngine;

public class CInventory : MonoBehaviour
{
    [Serializable]
    public class CInventoryItem
    {
        public CItemData ItemData;
        public int Count;

        public CInventoryItem(CItemData itemData)
        {
            ItemData = itemData;
            Count = 1;
        }
    }


    #region 내부변수
    private List<CInventoryItem> _items = new List<CInventoryItem>();

    #endregion


    #region 프로퍼티
    public List<CInventoryItem> Items => _items;

    #endregion


    public void AddItem(CItemData itemData)
    {
        if (itemData == null)
        {
            return;
        }

        CInventoryItem item = FindItem(itemData.ItemID);

        // 이미 가지고 있는 아이템
        if (item != null)
        {
            item.Count++;
        }

        // 처음 획득한 아이템
        else
        {
            _items.Add(new CInventoryItem(itemData));
        }

        CPrint.Log($"아이템 획득 : {itemData.ItemName}");
    }

    public bool RemoveItem(CItemData itemData)
    {
        if (itemData == null)
        {
            return false;
        }

        CInventoryItem item = FindItem(itemData.ItemID);

        if (item == null)
        {
            return false;
        }

        if (item.Count <= 0)
        {
            return false;
        }

        item.Count--;

        // 전부 사용했다면 목록에서 제거
        if (item.Count <= 0)
        {
            _items.Remove(item);
        }

        return true;
    }


    public CInventoryItem FindItem(string itemID)
    {
        for (int i = 0; i < _items.Count; ++i)
        {
            if (_items[i].ItemData.ItemID == itemID)
            {
                return _items[i];
            }
        }

        return null;
    }
}