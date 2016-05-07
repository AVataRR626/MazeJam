using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    public void QuitGame () {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void StartLevel(string name)
    {
        SceneManager.LoadScene(name);
        /*
        ïf(name == "Convergence") {
            
        }
        "Divergence"
        "Destiny"
        "PointToPoint"
            "Conquest"
            */

    }




}
