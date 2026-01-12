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
    public string nextSceneName; // 下一關場景名稱 (例如 Day2_Scene 或 Credits)
    public int startDialogueID = 10;

    [Header("Day 3 物件 (Day 3 場景才需要拖)")]
    public GameObject dadObject;
    public GameObject bloodStainObject;
    public GameObject bloodTrailObject;
    public GameObject policeCarObject;

    [Header("Day 3 進度狀態")]
    public int day3Stage = 0;

    [Header("音效控制")]
    public AudioSource audioSource;
    public AudioClip monsterScreamClip;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 遊戲開始，淡入並執行開場
        StartCoroutine(ShowDayRoutine("Day " + currentDay, startDialogueID));

        // 如果是 Day 3，初始化場景物件
        if (currentDay == 3) UpdateDay3WorldState(0);
    }


    // 1. 切換到下一個場景 (Day 1 -> Day 2)
    public void GoToNextLevel()
    {
        StartCoroutine(LoadNextLevelRoutine());
    }

    // 2. 時間跳躍 (Day 2 下午 -> 傍晚)
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
            case 1: break; // 拿到魚
            case 2: // 老爸消失，出現血跡
                if (dadObject) dadObject.SetActive(false);
                if (bloodStainObject) bloodStainObject.SetActive(true);
                break;
            case 3: // 發現血跡，出現警車
                if (bloodTrailObject) bloodTrailObject.SetActive(true);
                if (policeCarObject) policeCarObject.SetActive(true);
                break;
        }
    }

    public void TriggerEnding()
    {
        StartCoroutine(EndingSequence());
    }

    public void PlayMonsterSound()
    {
        if (audioSource && monsterScreamClip) audioSource.PlayOneShot(monsterScreamClip);
    }

    public void QuitGame()
    {
        Debug.Log("遊戲強制結束");
        Application.Quit();
    }


    // 開場流程
    IEnumerator ShowDayRoutine(string text, int nextDialogueID = -1)
    {
        transitionPanel.SetActive(true);
        dayText.text = text;
        dayText.gameObject.SetActive(true);
        dayText.color = Color.white;

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

    // 換場景流程 (Day 1 -> Day 2)
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

    // 時間跳躍流程 (Day 2 專用)
    IEnumerator TimeSkipRoutine(int nextDialogueID)
    {
        transitionPanel.SetActive(true);
        Image bg = transitionPanel.GetComponent<Image>();

        // 淡入黑屏
        float timer = 0f;
        while (timer < 1.0f)
        {
            timer += Time.deltaTime;
            bg.color = new Color(0, 0, 0, Mathf.Lerp(0f, 1f, timer));
            yield return null;
        }

        yield return new WaitForSeconds(1.5f); // 模擬時間流逝

        // 淡出黑屏
        timer = 0f;
        while (timer < 1.0f)
        {
            timer += Time.deltaTime;
            bg.color = new Color(0, 0, 0, Mathf.Lerp(1f, 0f, timer));
            yield return null;
        }
        transitionPanel.SetActive(false);

        // 接續對話
        DialogueManager.Instance.StartConversation(nextDialogueID);
    }

    // 結局演出流程 (Day 3 專用)
    IEnumerator EndingSequence()
    {
        transitionPanel.SetActive(true);
        Image bg = transitionPanel.GetComponent<Image>();
        bg.color = Color.black;

        dayText.gameObject.SetActive(true);
        dayText.color = Color.white;
        dayText.fontSize = 36;

        dayText.text = "???：抓到同夥";
        yield return new WaitForSeconds(2.5f);

        dayText.text = "???：帶回去";
        yield return new WaitForSeconds(2.0f);

        dayText.text = "???：可是他只是一個小孩欸";
        yield return new WaitForSeconds(2.5f);

        dayText.text = "???：共匪不分年齡別忘記了";
        yield return new WaitForSeconds(3.0f);

        dayText.text = "???：等等那是什麼";
        yield return new WaitForSeconds(1.5f);

        if (audioSource && monsterScreamClip) audioSource.PlayOneShot(monsterScreamClip);

        dayText.color = Color.red;
        dayText.fontSize = 50;
        dayText.text = "???：你不要過來啊!!!";

        yield return new WaitForSeconds(4.0f);

        dayText.text = "";

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("遊戲結束 (請設定 Next Scene Name 為 Credits)");
            Application.Quit();
        }
    }
}