using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject tutorialButton;

    public void Start()
    {
        tutorialButton.SetActive(false);
    }


    public void LoadTutorial()
    {
        tutorialButton.SetActive(true);
    }

    public void Play()
    {
        SceneManager.LoadScene("Level");
        Time.timeScale = 1f;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
