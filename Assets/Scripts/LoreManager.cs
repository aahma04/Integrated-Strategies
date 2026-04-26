using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoreManager : MonoBehaviour
{
    [System.Serializable]
    public class LoreEntry
    {
        public string title;
        [TextArea(4, 10)]
        public string content;
    }

    [Header("Lore Content - paste your text here")]
    public LoreEntry tape1;
    public LoreEntry tape2;
    public LoreEntry tape3;
    public LoreEntry document1;
    public LoreEntry document2;
    public LoreEntry document3;
    public LoreEntry newspaper1;
    public LoreEntry newspaper2;
    public LoreEntry newspaper3;
    public LoreEntry journal;

    [Header("Buttons - drag each button in")]
    public Button btnTape1;
    public Button btnTape2;
    public Button btnTape3;
    public Button btnDocument1;
    public Button btnDocument2;
    public Button btnDocument3;
    public Button btnNewspaper1;
    public Button btnNewspaper2;
    public Button btnNewspaper3;
    public Button btnJournal;

    [Header("Detail Panel References")]
    public GameObject detailPanel;
    public TextMeshProUGUI detailTitle;
    public TextMeshProUGUI detailContent;
    public Button closeButton;

    [Header("Locked Panel")]
    public GameObject lockedPanel;

    private const int LVL_TAPE1 = 0;
    private const int LVL_DOCUMENT2 = 2;
    private const int LVL_DOCUMENT1 = 4;
    private const int LVL_NEWSPAPER2 = 6;
    private const int LVL_TAPE2 = 8;
    private const int LVL_NEWSPAPER1 = 10;
    private const int LVL_DOCUMENT3 = 12;
    private const int LVL_NEWSPAPER3 = 14;
    private const int LVL_TAPE3 = 16;
    private const int LVL_JOURNAL = 18;

    void Start()
    {
        Debug.Log($"Player Level: {PlayerProgress.playerLevel}, XP: {PlayerProgress.currentXP}");

        detailPanel.SetActive(false);
        if (lockedPanel != null) lockedPanel.SetActive(false);
        closeButton.onClick.AddListener(() => detailPanel.SetActive(false));

        ApplyLockState(btnTape1, LVL_TAPE1);
        ApplyLockState(btnTape2, LVL_TAPE2);
        ApplyLockState(btnTape3, LVL_TAPE3);
        ApplyLockState(btnDocument1, LVL_DOCUMENT1);
        ApplyLockState(btnDocument2, LVL_DOCUMENT2);
        ApplyLockState(btnDocument3, LVL_DOCUMENT3);
        ApplyLockState(btnNewspaper1, LVL_NEWSPAPER1);
        ApplyLockState(btnNewspaper2, LVL_NEWSPAPER2);
        ApplyLockState(btnNewspaper3, LVL_NEWSPAPER3);
        ApplyLockState(btnJournal, LVL_JOURNAL);
    }

    void ApplyLockState(Button btn, int requiredLevel)
    {
        if (btn == null) return;
        bool unlocked = PlayerProgress.playerLevel >= requiredLevel;

        // Dim the button if locked
        Image img = btn.GetComponent<Image>();
        if (img != null)
            img.color = unlocked ? Color.white : new Color(0.4f, 0.4f, 0.4f, 1f);

        // Also dim the text
        TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
            tmp.color = unlocked ? Color.black : new Color(0.5f, 0.5f, 0.5f, 1f);
    }

    public void OpenTape1() => TryOpen(tape1, LVL_TAPE1);
    public void OpenTape2() => TryOpen(tape2, LVL_TAPE2);
    public void OpenTape3() => TryOpen(tape3, LVL_TAPE3);
    public void OpenDocument1() => TryOpen(document1, LVL_DOCUMENT1);
    public void OpenDocument2() => TryOpen(document2, LVL_DOCUMENT2);
    public void OpenDocument3() => TryOpen(document3, LVL_DOCUMENT3);
    public void OpenNewspaper1() => TryOpen(newspaper1, LVL_NEWSPAPER1);
    public void OpenNewspaper2() => TryOpen(newspaper2, LVL_NEWSPAPER2);
    public void OpenNewspaper3() => TryOpen(newspaper3, LVL_NEWSPAPER3);
    public void OpenJournal() => TryOpen(journal, LVL_JOURNAL);

    void TryOpen(LoreEntry entry, int requiredLevel)
    {
        if (PlayerProgress.playerLevel < requiredLevel)
        {
            if (lockedPanel != null)
                lockedPanel.SetActive(true);
            return;
        }
        Open(entry);
    }

    void Open(LoreEntry entry)
    {
        detailTitle.text = entry.title;
        detailContent.text = entry.content;
        detailPanel.SetActive(true);
    }
}