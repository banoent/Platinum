using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerUseItem : MonoBehaviour
{
    #region ¿ŒΩ∫∆Â≈Õ

    [SerializeField] private CInventory _inventory;
    [SerializeField] private CPlayer _player;

    [SerializeField] private float _healAmount = 30f;

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            UsePotion();
        }
    }

    private void UsePotion()
    {
        if (!_inventory.UsePotion())
        {
            return;
        }
            
    }
}
