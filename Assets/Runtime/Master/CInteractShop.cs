using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CInteractShop : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private CShop _shop;
    [SerializeField] private CShopUI _shopUI;

    #endregion

    #region 내부변수
    private bool _canInteract;

    #endregion
    

    void Update()
    {
        if (!_canInteract)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleShop();
        }
    }

    private void ToggleShop()
    {
        if (_shopUI.IsOpen)
        {
            _shopUI.Close();
        }
        else
        {
            _shopUI.Open(_shop);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CPlayer>() == null)
        {
            return;
        }

        _canInteract = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CPlayer>() == null)
        {
            return;
        }

        _canInteract = false;

        if (_shopUI.IsOpen)
        {
            _shopUI.Close();
        }
    }
}
