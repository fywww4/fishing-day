using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // 引用場景管理

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("UI 組件")]
    public GameObject transitionPanel;
    public TMP_Text dayText;
    public float fadeDuration = 2.0f;
    public float textStayDuration = 2.0f;

    [Header("關卡設定 (手動設定)")]
    // 在 Day 1 場景就把這裡設為 1，在 Day 2 場景設為 2
    public int currentDay = 1;

    // 下一關的場景名稱 (例如 "Day2_Scene")
    public string nextSceneName;

    // 該場景開場時要執行的對話 ID (Day1=10, Day2=20)
    public int startDialogueID = 10;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 遊戲開始，顯示 Day X，並在淡出後執行開場對話
        StartCoroutine(ShowDayRoutine("Day " + currentDay, startDialogueID));
    }

    // 外部呼叫：切換到下一個場景 (Day 1 -> Day 2)
    public void GoToNextLevel()
    {
        StartCoroutine(LoadNextLevelRoutine());
    }

    // 外部呼叫：時間跳躍 (Day 2 下午 -> 傍晚，不換場景)
    public void TriggerTimeSkip(int nextDialogueID)
    {
        StartCoroutine(TimeSkipRoutine(nextDialogueID));
    }

    // --- 流程協程 ---

    // 開場流程：黑幕 -> 顯示 Day 文字 -> 淡出 -> 對話
    IEnumerator ShowDayRoutine(string text, int nextDialogueID = -1)
    {
        transitionPanel.SetActive(true);
        dayText.text = text;
        dayText.gameObject.SetActive(true);

        Image bg = transitionPanel.GetComponent<Image>();
        Color c = bg.color;
        c.a = 1f; // 全黑
        bg.color = c;

        // 停頓顯示 Day X
        yield return new WaitForSeconds(textStayDuration);

        dayText.gameObject.SetActive(false);

        // 淡出 (畫面變亮)
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

        // 觸發開場獨白
        if (nextDialogueID != -1)
        {
            DialogueManager.Instance.StartConversation(nextDialogueID);
        }
    }

    // 換場景流程：淡入黑幕 -> 載入新場景
    IEnumerator LoadNextLevelRoutine()
    {
        // 1. 開啟黑幕
        transitionPanel.SetActive(true);
        Image bg = transitionPanel.GetComponent<Image>();

        // 2. 淡入 (變黑)
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            bg.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // 3. 載入下一個場景
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("請在 Inspector 設定 Next Scene Name！");
        }
    }

    // 時間跳躍 (場景內黑屏過場)
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
}