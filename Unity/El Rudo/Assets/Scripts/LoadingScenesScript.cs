using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScenesScript : MonoBehaviour
{
    [SerializeField]
    Slider slider;
    public void LoadFightingScene()
    {
        StartCoroutine(LoadYourAsyncScene());
    }

    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("FightScene");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            slider.value = asyncLoad.progress / .9f;
            yield return null;
        }
    }
}
