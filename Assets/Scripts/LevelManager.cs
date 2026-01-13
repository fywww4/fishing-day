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
    public AudioClip hitSoundClip;

    [Header("結局專用音效 (新功能)")]
    public AudioClip mysteryVoiceClip;  // 神秘人講話的嘟嘟聲
    public AudioClip chipEatClip;       // 吃洋芋片的搞笑聲音
    public float typingSpeed = 0.05f;   // 打字速度

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(ShowDayRoutine("Day " + currentDay, startDialogueID));
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
            case 1: break;
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

    public void QuitGame()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("遊戲結束");
            Application.Quit();
        }
    }

  
    IEnumerator EndingSequence()
    {

        // 1. 播放敲擊聲 (蹦！)
        if (audioSource && hitSoundClip) audioSource.PlayOneShot(hitSoundClip);

        // 瞬間黑屏
        transitionPanel.SetActive(true);
        Image bg = transitionPanel.GetComponent<Image>();
        bg.color = Color.black;

        dayText.gameObject.SetActive(true);

        // 2. 開始播放結局對話
        // 使用 yield return StartCoroutine(...) 等待打字打完，再等待閱讀時間

        yield return StartCoroutine(TypewriterEffect("???：抓到同夥", Color.white, 36));
        yield return new WaitForSeconds(1.5f); // 閱讀時間

        yield return StartCoroutine(TypewriterEffect("???：帶回去", Color.white, 36));
        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(TypewriterEffect("???：可是他只是一個小孩欸", Color.white, 36));
        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(TypewriterEffect("???：共匪不分年齡別忘記了", Color.white, 36));
        yield return new WaitForSeconds(2.0f);

        yield return StartCoroutine(TypewriterEffect("???：等等那是什麼", Color.white, 36));
        yield return new WaitForSeconds(0.5f);

        // 3. 搞笑反轉：播放吃洋芋片聲音
        if (audioSource && chipEatClip)
        {
            // 稍微改變音調讓咀嚼聲更有趣
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(chipEatClip);
        }
        yield return new WaitForSeconds(1.5f); // 讓玩家聽一下咀嚼聲


        // 最後一句變紅色、字體變大
        yield return StartCoroutine(TypewriterEffect("???：你不要過來啊!!!", Color.red, 60));

        yield return new WaitForSeconds(3.0f);

        dayText.text = "";
        QuitGame();
    }

    //迷你打字機協程 (LevelManager 專用)
    IEnumerator TypewriterEffect(string content, Color color, int fontSize)
    {
        dayText.text = ""; // 清空
        dayText.color = color;
        dayText.fontSize = fontSize;

        foreach (char letter in content.ToCharArray())
        {
            dayText.text += letter;

            // 播放語音 (如果有設定的話)
            if (audioSource && mysteryVoiceClip && !string.IsNullOrWhiteSpace(letter.ToString()))
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f); // 隨機音調
                audioSource.PlayOneShot(mysteryVoiceClip);
            }

            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // ==========================================
    // 其他協程 (Day1/Day2 用)
    // ==========================================

    IEnumerator ShowDayRoutine(string text, int nextDialogueID = -1)
    {
        transitionPanel.SetActive(true);
        dayText.text = text;
        dayText.gameObject.SetActive(true);
        dayText.color = Color.white;
        dayText.fontSize = 50; // Day X 字體大一點

        Image bg = transitionPanel.GetComponent<Image>();
        Color c = bg.color;
        c.a = 1f;
        bg.color = c;

        yield return new WaitForSeconds(textStayDuration);
        dayText.gameObject.SetActive(false);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            bg.color = c;
            yield return null;
        }
        c.a = 0f;
        bg.color = c;
        transitionPanel.SetActive(false);

        if (nextDialogueID != -1) DialogueManager.Instance.StartConversation(nextDialogueID);
    }

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

    IEnumerator TimeSkipRoutine(int nextDialogueID)
    {
        transitionPanel.SetActive(true);
        Image bg = transitionPanel.GetComponent<Image>();

        float timer = 0f;
        while (timer < 1.0f)
        {
            timer += Time.deltaTime;
            bg.color = new Color(0, 0, 0, Mathf.Lerp(0f, 1f, timer));
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

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