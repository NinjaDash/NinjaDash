using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCoins : MonoBehaviour
{
    private void OnEnable()
    {
        if (UIManager.Instance)
        {
            UIManager.Instance.SetCoinText();
        }
    }
}
