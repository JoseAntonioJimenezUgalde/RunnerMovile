using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Puntos : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private int puntos;

    public static Puntos instance;


    private void Awake()
    {
        if (instance == null) { instance = this; } else { Destroy(instance); }
    }
    public void PuntosCurrent()
    {
        puntos += 10;

        text.text = "Puntos = " + puntos.ToString();
    }
}
