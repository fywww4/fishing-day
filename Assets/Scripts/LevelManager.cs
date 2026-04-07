using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("UI 組件")]
    public GameObject transitionPanel;
    public TMP_Text dayText;
    public float fadeDuration = 2.0f;
    public float textStayDuration = 2.0f;

    [Header("關卡設定")]
    public int currentDay = 1;
    public string nextSceneName;
    public int startDialogueID = 10;

    [Header("Day 3 物件")]
    public GameObject dadObject;
    public GameObject bloodStainObject;
    public GameObject bloodTrailObject;
    public GameObject policeCarObject;

    [Header("Day 3 進度狀態")]
    public int day3Stage = 0;

    [Header("音效控制")]
    public AudioSource audioSource;
    public AudioClip monsterScreamClip;

    [Header("結局專用音效")]
    public AudioClip hitSoundClip;
    public AudioClip mysteryVoiceClip;
    public AudioClip chipEatClip;
    public float typingSpeed = 0.05f;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        string dayDisplayText = (currentDay == 4) ? "???" : "Day " + currentDay;
        StartCoroutine(ShowDayRoutine(dayDisplayText, startDialogueID));
        if (currentDay == 3) UpdateDay3WorldState(0);
    }

    public void GoToNextLevel()
    {
        StartCoroutine(LoadNextLevelRoutine());
    }

    public void TriggerTimeSkip(int nextDialogueID)
    {
        StartCoroutine(TimeSkipRoutine(nextDialogueID));
    }

    public void SetDay3Stage(int stage)
    {
        day3Stage = stage;
        UpdateDay3WorldState(stage);
    }

    void UpdateDay3WorldState(int stage)
    {
        if (currentDay != 3) return;
        switch (stage)
        {
            case 0:
                if (dadObject) dadObject.SetActive(true);
                if (bloodStainObject) bloodStainObject.SetActive(false);
                if (bloodTrailObject) bloodTrailObject.SetActive(false);
                if (policeCarObject) policeCarObject.SetActive(false);
                break;
            case 2:
                if (dadObject) dadObject.SetActive(false);
                if (bloodStainObject) bloodStainObject.SetActive(true);
                break;
            case 3:
                if (bloodTrailObject) bloodTrailObject.SetActive(true);
                if (policeCarObject) policeCarObject.SetActive(true);
                break;
        }
    }

    public void TriggerEnding()
    {
        StartCoroutine(EndingSequence());
    }

    // ==========================================
    // 結局演出
    // ==========================================
    IEnumerator EndingSequence()
    {
        if (audioSource && hitSoundClip) audioSource.PlayOneShot(hitSoundClip);

        transitionPanel.SetActive(true);
        Image bg = transitionPanel.GetComponent<Image>();
        bg.color = Color.black;

        dayText.text = "";
        dayText.gameObject.SetActive(true);
        dayText.alignment = TextAlignmentOptions.Center;

        RectTransform rt = dayText.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        bool isEnglish = PlayerPrefs.GetInt("Language", 0) == 1;

        string[] lines = {
            isEnglish ? "???: Got the accomplice." : "???：抓到同夥",
            isEnglish ? "???: Take him in." : "???：帶回去",
            isEnglish ? "???: But he's just a kid!" : "???：可是他只是一個小孩欸",
            isEnglish ? "???: Don't forget, commie spies come in all ages." : "???：共匪不分年齡別忘記了",
            isEnglish ? "???: Wait, what the hell is that?" : "???：等等那是什麼",
            isEnglish ? "Stay away!!!" : "你不要過來啊!!!"
        };

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < lines.Length; i++)
        {
            Color textColor = (i == 5) ? Color.red : Color.white;
            int fontSize = (i == 5) ? 70 : 40;

            if (i == 4 && audioSource && chipEatClip) audioSource.PlayOneShot(chipEatClip);
            if (i == 5 && audioSource && monsterScreamClip) audioSource.PlayOneShot(monsterScreamClip);

            yield return StartCoroutine(TypewriterEffect(lines[i], textColor, fontSize));
            yield return new WaitForSeconds(1.8f);
        }

        yield return new WaitForSeconds(2.0f);
        dayText.text = "";

        // 載入 Home 場景
        SceneManager.LoadScene("Home");
    }

    IEnumerator TypewriterEffect(string content, Color color, int fontSize)
    {
        dayText.text = "";
        dayText.color = color;
        dayText.fontSize = fontSize;

        foreach (char letter in content.ToCharArray())
        {
            dayText.text += letter;
            if (audioSource && mysteryVoiceClip && !string.IsNullOrWhiteSpace(letter.ToString()))
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(mysteryVoiceClip);
            }
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // --- 補回：天數轉場與過場 ---

    IEnumerator ShowDayRoutine(string text, int nextDialogueID = -1)
    {
        transitionPanel.SetActive(true);
        dayText.text = text;
        dayText.gameObject.SetActive(true);
        dayText.alignment = TextAlignmentOptions.Center;
        dayText.color = Color.white;
        dayText.fontSize = 60;

        Image bg = transitionPanel.GetComponent<Image>();
        bg.color = Color.black;

        yield return new WaitForSeconds(textStayDuration);
        dayText.gameObject.SetActive(false);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            bg.color = new Color(0, 0, 0, Mathf.Lerp(1f, 0f, timer / fadeDuration));
            yield return null;
        }
        transitionPanel.SetActive(false);
        if (nextDialogueID != -1) DialogueManager.Instance.StartConversation(nextDialogueID);
    }

    //  補回：載入下一個關卡 (淡出)
    IEnumerator LoadNextLevelRoutine()
    {
        transitionPanel.SetActive(true);
        Image bg = transitionPanel.GetComponent<Image>();
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            bg.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        if (!string.IsNullOrEmpty(nextSceneName)) SceneManager.LoadScene(nextSceneName);
    }

    // 補回：時間跳轉 (淡入淡出並啟動對話)
    IEnumerator TimeSkipRoutine(int nextDialogueID)
    {
        transitionPanel.SetActive(true);
        Image bg = transitionPanel.GetComponent<Image>();

        // 淡入黑色
        float timer = 0f;
        while (timer < 1.0f)
        {
            timer += Time.deltaTime;
            bg.color = new Color(0, 0, 0, Mathf.Lerp(0f, 1f, timer));
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        // 淡出黑色
        timer = 0f;
        while (timer < 1.0f)
        {
            timer += Time.deltaTime;
            bg.color = new Color(0, 0, 0, Mathf.Lerp(1f, 0f, timer));
            yield return null;
        }
        transitionPanel.SetActive(false);
        DialogueManager.Instance.StartConversation(nextDialogueID);
    }
}