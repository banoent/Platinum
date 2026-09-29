using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CInventory : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private int _potionCount;

    #endregion

    #region 내부변수
    public int PotionCount => _potionCount;

    #endregion

    public void AddPotionInInventory()
    {
        _potionCount++;
    }

    public bool UsePotion()
    {
        if (_potionCount <= 0)
        {
            return false;
        }           

        _potionCount--;

        return true;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
