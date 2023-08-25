using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Coin : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private int cointInt;
    public static Coin instance;

    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void CoinText()
    {
        cointInt++;
        coinText.text = "Monedas " + cointInt.ToString();
    }
}
