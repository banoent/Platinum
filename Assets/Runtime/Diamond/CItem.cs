using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CInventory inventory = other.GetComponent<CInventory>();

        if (inventory == null)
        {
            return;
        }

        inventory.AddPotionInInventory();

        Destroy(gameObject);
    }
}
