using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Main Scene");
    }

    public void Quit()
    { 
        Application.Quit();
    }

}
