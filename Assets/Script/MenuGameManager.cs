using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGameManager : MonoBehaviour
{
    public void StartGame()
    {
       
        SceneManager.LoadScene("P1PicView");
    }
}
