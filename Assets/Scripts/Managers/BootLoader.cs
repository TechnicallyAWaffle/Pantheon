using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    void Awake()
    {
        if (!SceneManager.GetSceneByName("Managers").isLoaded)
            SceneManager.LoadScene("Managers", LoadSceneMode.Additive);
        GameObject.Destroy(gameObject);
    }
}
