using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SceneMan : MonoBehaviour
{
    public void ChangeScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }
}
