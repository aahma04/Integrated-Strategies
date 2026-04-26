using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject[] tutorialButtons;

    public GameObject ResetDataPanel;

    private int currentTutorialIndex = -1;

    public void Start()
    {
        foreach(GameObject tutorialButton in tutorialButtons)
        {
            tutorialButton.SetActive(false);
        }
        ResetDataPanel.SetActive(false);

        Debug.Log($"Player Level: {PlayerProgress.playerLevel}, XP: {PlayerProgress.currentXP}");
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


    public void ResetDataPrompt()
    {
        ResetDataPanel.SetActive(true);
    }


    public void ConfirmResetData()
    {
        PlayerPrefs.DeleteAll();
        ResetDataPanel.SetActive(false);
    }


    public void DenyResetData()
    {
        ResetDataPanel.SetActive(false);
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
