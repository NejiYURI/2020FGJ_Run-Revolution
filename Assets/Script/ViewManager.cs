using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewManager : MonoBehaviour
{

    public AudioSource SEPlayer;
    public void StartBtn(string SceneName)
    {
        SEPlayer.Play();
        SceneManager.LoadScene(SceneName);
    }
}
