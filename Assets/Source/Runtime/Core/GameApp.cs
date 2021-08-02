using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameApp : MonoBehaviour
{
    private static GameApp _currentApp;

    [RuntimeInitializeOnLoadMethod]
    private static void OnGameAppStart()
    {
        GameObject appHndl = new GameObject("_GAME_APP_HANDLE");
        _currentApp = appHndl.AddComponent<GameApp>();
        GameObject.DontDestroyOnLoad(appHndl);
    }

    private void Start()
    {
    }
}
