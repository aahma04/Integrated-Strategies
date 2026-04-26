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
        public Sprite icon;
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

    [Header("Detail Panel References")]
    public GameObject detailPanel;
    public Image detailIcon;
    public TextMeshProUGUI detailTitle;
    public TextMeshProUGUI detailContent;
    public Button closeButton;

    void Start()
    {
        detailPanel.SetActive(false);
        closeButton.onClick.AddListener(() => detailPanel.SetActive(false));
    }

    public void OpenTape1() => Open(tape1);
    public void OpenTape2() => Open(tape2);
    public void OpenTape3() => Open(tape3);
    public void OpenDocument1() => Open(document1);
    public void OpenDocument2() => Open(document2);
    public void OpenDocument3() => Open(document3);
    public void OpenNewspaper1() => Open(newspaper1);
    public void OpenNewspaper2() => Open(newspaper2);
    public void OpenNewspaper3() => Open(newspaper3);
    public void OpenJournal() => Open(journal);

    void Open(LoreEntry entry)
    {
        detailTitle.text = entry.title;
        detailContent.text = entry.content;
        if (detailIcon != null)
        {
            detailIcon.sprite = entry.icon;
            detailIcon.gameObject.SetActive(entry.icon != null);
        }
        detailPanel.SetActive(true);
    }
}