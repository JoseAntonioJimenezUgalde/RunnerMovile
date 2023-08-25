using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Slider loadingSlider;  // Asigna tu Slider desde el inspector
    public float loadingTime = 3f; // Tiempo de espera antes de comenzar la carga

    private void Start()
    {
        Time.timeScale = 1;

        string levelToLoad = UIManager.nextLevel;
        StartCoroutine(MakeTheLoad(levelToLoad));
    }

    IEnumerator MakeTheLoad(string level)
    {
        // Esperar el tiempo de carga antes de empezar
        yield return new WaitForSeconds(loadingTime);

        // Comenzar la carga de la escena de manera asíncrona   
        AsyncOperation operation = SceneManager.LoadSceneAsync(level);

        // Actualizar el valor del slider mientras se carga la escena
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // El progreso va de 0 a 0.9
            loadingSlider.value = progress;
            yield return null;
        }
    }
}

