using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioHandler : MonoBehaviour
{

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        if (scene.name != "Level") {
            Destroy(gameObject);
        }
    }
    
}
