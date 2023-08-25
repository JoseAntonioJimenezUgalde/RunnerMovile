using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool pause;
    [SerializeField] private GameObject panelPause;

    public void Pause()
    {
        pause = !pause;

        if (pause)
        {
            AudioListener.volume = 0f;
            panelPause.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            AudioListener.volume = 1f;
            panelPause.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
