using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static string nextLevel;


    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void StringScene(string scene)
    {
        nextLevel = scene;
    }
}
