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

    [Header("Day 3 特別物件控制 (請拖曳場景物件)")]
    public GameObject dadObject;        // 老爸物件
    public GameObject bloodStainObject; // 地板血跡 (觸發點)
    public GameObject policeCarObject;  // 警車 (結局觸發點)
    public GameObject bloodTrailObject; // 引導路徑的血跡 (裝飾)

    [Header("Day 3 進度狀態")]
    // 0:剛開始, 1:拿到魚, 2:送完魚(老爸消失), 3:發現血跡
    public int day3Stage = 0;

    [Header("音效控制")]
    public AudioSource audioSource;
    public AudioClip monsterScreamClip; // 怪物叫聲

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 遊戲開始，淡入並執行開場
        StartCoroutine(ShowDayRoutine("Day " + currentDay, startDialogueID));

        // 如果是 Day 3，初始化場景物件
        if (currentDay == 3)
        {
            UpdateDay3WorldState(0);
        }
    }

    // 外部呼叫：更新 Day 3 進度
    public void SetDay3Stage(int stage)
    {
        day3Stage = stage;
        UpdateDay3WorldState(stage);
    }

    // 控制物件顯隱
    void UpdateDay3WorldState(int stage)
    {
        if (currentDay != 3) return;

        switch (stage)
        {
            case 0: // 剛開始：老爸在，沒血跡，沒警車
                if (dadObject) dadObject.SetActive(true);
                if (bloodStainObject) bloodStainObject.SetActive(false);
                if (policeCarObject) policeCarObject.SetActive(false);
                if (bloodTrailObject) bloodTrailObject.SetActive(false);
                break;
            case 1: // 拿到魚：狀態不變
                break;
            case 2: // 送完魚：老爸消失，出現血跡
                if (dadObject) dadObject.SetActive(false); // 老爸消失
                if (bloodStainObject) bloodStainObject.SetActive(true); // 血跡觸發點出現
                break;
            case 3: // 檢查完血跡：出現警車與引導血跡
                if (bloodTrailObject) bloodTrailObject.SetActive(true);
                if (policeCarObject) policeCarObject.SetActive(true);
                break;
        }
    }

    // 外部呼叫：觸發結局
    public void TriggerEnding()
    {
        StartCoroutine(EndingRoutine());
    }

    // --- 流程協程 ---

    IEnumerator ShowDayRoutine(string text, int nextDialogueID = -1)
    {
        transitionPanel.SetActive(true);
        dayText.text = text;
        dayText.gameObject.SetActive(true);

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

        if (nextDialogueID != -1)
        {
            DialogueManager.Instance.StartConversation(nextDialogueID);
        }
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

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    //結局流程
    IEnumerator EndingRoutine()
    {
        // 1. 瞬間黑屏 (蹦!)
        transitionPanel.SetActive(true);
        Image bg = transitionPanel.GetComponent<Image>();
        bg.color = Color.black;

        // 2. 開始播放結局對話 (ID 33)
        // 注意：這裡我們不等待淡出，直接在黑屏下對話
        DialogueManager.Instance.StartConversation(33);

        // 等待對話結束 (這裡用一個簡單的檢查，或是依賴 DialogueManager 呼叫下一步)
        // 但為了配合音效，我們可以在這裡寫死一些時間點，或者讓對話系統呼叫音效
        yield return null;
    }

    // 播放怪物音效 (給 DialogueManager 呼叫)
    public void PlayMonsterSound()
    {
        if (audioSource && monsterScreamClip)
        {
            audioSource.PlayOneShot(monsterScreamClip);
        }
    }

    // 真的結束遊戲
    public void QuitGame()
    {
        Debug.Log("遊戲結束");
        Application.Quit(); // 打包後才有效
    }
}