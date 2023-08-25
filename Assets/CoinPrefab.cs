using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPrefab : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Coin.instance.CoinText();
            Puntos.instance.PuntosCurrent();
            Destroy(gameObject);
        }
    }
}
