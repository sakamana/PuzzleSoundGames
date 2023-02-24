using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void OnclicStartButton()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnclicEndButton()
    {
        Application.Quit();
    }

    public void OnclicBackButton()
    {
        SceneManager.LoadScene("StartScene");
    }
}
