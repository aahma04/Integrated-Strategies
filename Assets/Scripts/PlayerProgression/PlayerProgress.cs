using UnityEngine;

public static class PlayerProgress
{
    private const string XP_KEY = "playerXP";

    public static int currentXP
    {
        get => PlayerPrefs.GetInt(XP_KEY, 0);
        private set
        {
            PlayerPrefs.SetInt(XP_KEY, value);
            PlayerPrefs.Save();
        }
    }

    public static int playerLevel => currentXP / 100;

    public static int XPIntoCurrentLevel => currentXP % 100;

    public static void AddXP(int amount)
    {
        currentXP += amount;
    }

    // For testing — call this to wipe progress
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(XP_KEY);
        PlayerPrefs.Save();
    }
}