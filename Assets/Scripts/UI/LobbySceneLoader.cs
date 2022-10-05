using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbySceneLoader : MonoBehaviour
{
   public void LoadLobbyScene()
    {
	    SceneManager.LoadScene("Lobby");
    }
}
