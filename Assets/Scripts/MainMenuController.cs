using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject[] tutorialButtons;

    private int currentTutorialIndex = -1;

    public void Start()
    {
        foreach(GameObject tutorialButton in tutorialButtons)
        {
            tutorialButton.SetActive(false);
        }
    }


    public void LoadNextTutorial()
    {
        currentTutorialIndex++;

        if (currentTutorialIndex < tutorialButtons.Length)
        {
            tutorialButtons[currentTutorialIndex].SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("Level");
            Time.timeScale = 1f;
        }
    }


    public void OpenLorePage()
    {
        SceneManager.LoadScene("LorePage");
    }

    public void OpenMainMenuPage()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
