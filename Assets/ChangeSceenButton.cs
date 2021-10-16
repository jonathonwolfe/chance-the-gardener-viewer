using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceenButton : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        LevelManager.Instanace.LoadScene(sceneName);
    }
}
