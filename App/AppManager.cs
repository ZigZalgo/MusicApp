using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour {

	public void OnHost()
    {
        SceneManager.LoadScene("Host");
    }

    public void OnJoin()
    {
        SceneManager.LoadScene("Client");
    }
}
