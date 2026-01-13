using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI 組件")]
    public GameObject dialoguePanel;
    public TMP_Text nameText;
    public TMP_Text bodyText;
    public GameObject optionA_Object;
    public GameObject optionB_Object;

    [Header("準心控制")]
    public GameObject centerDotObject;
    public GameObject outerCircleObject;

    [Header("語音設定")]
    public AudioSource voiceSource;
    public AudioClip zhimaClip;
    public AudioClip dadClip;
    public AudioClip dogClip;
    public AudioClip mysteryClip;
    public float typingSpeed = 0.05f;

    private bool isTalking = false;
    private int currentStep = 0;
    private int conversationID = 0;

    // 打字機相關變數
    private bool isTyping = false;
    private string currentFullText = "";
    private Coroutine typingCoroutine;

    private Color originalNameColor;
    private Color originalBodyColor;

    public MonoBehaviour playerController;

    void Awake() { Instance = this; }

    void Start()
    {
        dialoguePanel.SetActive(false);
        if (optionA_Object) optionA_Object.SetActive(false);
        if (optionB_Object) optionB_Object.SetActive(false);

        if (nameText) originalNameColor = nameText.color;
        if (bodyText) originalBodyColor = bodyText.color;
    }

    void Update()
    {
        if (isTalking) HandleInput();
    }

    void HandleInput()
    {
        // 1. 打字中按空白鍵 -> 瞬間顯示
        if (isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            StopCoroutine(typingCoroutine);
            bodyText.text = currentFullText;
            isTyping = false;
            return;
        }

        // 2. 打完字後按空白鍵 -> 下一句
        if (!isTyping)
        {
            if (optionA_Object.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.A)) ChooseOption(0);
                else if (Input.GetKeyDown(KeyCode.B)) ChooseOption(1);
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    currentStep++;
                    NextSentence();
                }
            }
        }
    }

    public void StartConversation(int id)
    {
        if (isTalking) return;
        conversationID = id;
        isTalking = true;
        dialoguePanel.SetActive(true);
        currentStep = 0;

        if (playerController != null) playerController.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (centerDotObject) centerDotObject.SetActive(false);
        if (outerCircleObject) outerCircleObject.SetActive(false);

        ResetTextColor();
        NextSentence();
    }

    public void NextSentence()
    {
        if (optionA_Object) optionA_Object.SetActive(false);
        if (optionB_Object) optionB_Object.SetActive(false);

        switch (conversationID)
        {
            case 0: Script_OldMan(); break;
            case 1: Script_Dog(); break;
            case 2: Script_Dad(); break;

            case 10: Script_Day1_Start(); break;
            case 20: Script_Day2_Start(); break;

            case 21: Script_Dad_Day2_Part2(); break; // Day 2 下半場

            case 30: Script_Day3_Start(); break;
            case 31: Script_Blood(); break;
            case 32: Script_Police(); break;
        }
    }

    // --- 顯示文字 (打字機核心) ---
    void ShowText(string name, string content)
    {
        nameText.text = name;
        currentFullText = content;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        AudioClip voice = null;
        if (name.Contains("芝麻")) voice = zhimaClip;
        else if (name.Contains("老爸") || name.Contains("父親") || name.Contains("老頭") || name.Contains("老人")) voice = dadClip;
        else if (name.Contains("狗")) voice = dogClip;
        else if (name.Contains("???")) voice = mysteryClip;

        typingCoroutine = StartCoroutine(TypeWriterEffect(content, voice));
    }

    IEnumerator TypeWriterEffect(string content, AudioClip voiceClip)
    {
        isTyping = true;
        bodyText.text = "";

        foreach (char letter in content.ToCharArray())
        {
            bodyText.text += letter;

            if (voiceSource && voiceClip && !string.IsNullOrWhiteSpace(letter.ToString()))
            {
                voiceSource.pitch = Random.Range(0.9f, 1.1f);
                voiceSource.PlayOneShot(voiceClip);
            }
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    // --- 劇本區 ---

    void Script_Dog()
    {
        switch (currentStep)
        {
            case 0: ShowText("黑狗", "汪汪汪！"); break;
            case 1: ShowText("芝麻", "..."); ShowOptions("[A] 乖狗狗", "[B] 滾開"); break;
            default: EndConversation(); break;
        }
    }

    void Script_OldMan()
    {
        int day = LevelManager.Instance.currentDay;
        int stage = LevelManager.Instance.day3Stage;

        if (day == 1)
        {
            switch (currentStep)
            {
                case 0: ShowText("老人", "中午好，芝麻，最近如何啊"); break;
                case 1: ShowText("芝麻", "很不錯"); break;
                case 2: ShowText("老人", "又去找你老爸一起釣魚去了?"); ShowOptions("[A] 對阿 今天可不能空手而歸", "[B] 乾你屁事啊 老東西"); break;
                case 3: ShowText("老人", "呵呵 祝你好運"); break;
                default: EndConversation(); break;
            }
        }
        else if (day == 2)
        {
            switch (currentStep)
            {
                case 0: ShowText("老人", "芝麻，這個給你，剛剛路上一個奇怪的人給我的，還對我說聖誕節快樂"); break;
                case 1: ShowText("芝麻", "酷"); break;
                case 2: ShowText("芝麻", "但你要留意一下政府的公告，應該說耶誕節快樂"); break;
                case 3: ShowText("芝麻", "我收下你的好意了"); break;
                default: EndConversation(); break;
            }
        }
        else if (day == 3)
        {
            //Day 3 修復重點：單句對話也要用 switch 包起來！

            if (stage == 0) // 還沒找老爸
            {
                switch (currentStep)
                {
                    case 0: ShowText("芝麻", "(老頭在睡覺...)"); break;
                    default: EndConversation(); break; // 按空白鍵後 (Step變1)，直接結束
                }
            }
            else if (stage == 1) // 拿到魚了
            {
                switch (currentStep)
                {
                    case 0: ShowText("芝麻", "阿伯，這是給你的，我父親說要跟你分享"); break;
                    case 1: ShowText("老頭", "喔，不用啦"); break;
                    case 2: ShowText("芝麻", "沒事沒事，有福同享"); break;
                    case 3: ShowText("老頭", "謝謝啦"); break;
                    default: EndConversation(); break;
                }
            }
            else // 送完禮後 (stage >= 2)
            {
                switch (currentStep)
                {
                    case 0: ShowText("老頭", "(繼續睡覺...)"); break;
                    default: EndConversation(); break;
                }
            }
        }
    }

    void Script_Dad()
    {
        int day = LevelManager.Instance.currentDay;
        if (day == 1) { switch (currentStep) { case 0: ShowText("老爸", "你來了阿"); break; case 1: ShowText("老爸", "你的魚竿在這裡"); break; case 2: ShowText("芝麻", "釣魚喽"); break; case 3: ShowText("老爸", "好了，明天再來吧，今天收獲不錯"); break; case 4: ShowText("芝麻", "好啊"); break; default: EndConversation(); break; } }
        else if (day == 2) { switch (currentStep) { case 0: ShowText("芝麻", "這些是什麼啊，太恐怖了吧"); break; case 1: ShowText("父親", "我也不知道，這些魚長的都蠻醜的"); break; case 2: ShowText("父親", "看看這魚回去能不能賣個好價錢"); break; case 3: ShowText("芝麻", "我覺得這些魚可以直接丟回去"); break; case 4: ShowText("父親", "不好說，搞不好是稀有的魚"); break; default: EndConversation(); break; } } // Day 2 上半場結束，EndConversation 會處理
        else if (day == 3) { switch (currentStep) { case 0: ShowText("父親", "你來啦，剛剛我收穫不錯，今天可以來個最後收尾"); break; case 1: ShowText("父親", "你幫我把這些拿去給小販的老闆，算是跟他分享一下我們的收穫"); break; case 2: ShowText("芝麻", "(獲得了一些魚)"); break; default: EndConversation(); break; } }
    }

    void Script_Dad_Day2_Part2()
    {
        switch (currentStep)
        {
            case 0: ShowText("父親", "今天收穫不是很好，明天再來最後一次，我們就準備換地點吧"); break;
            case 1: ShowText("芝麻", "好"); break;
            default: EndConversation(); break; // 這裡會觸發 Day 3 轉場
        }
    }

    void Script_Day1_Start() { switch (currentStep) { case 0: ShowText("芝麻", "美好的一天從釣魚開始"); break; case 1: ShowText("芝麻", "我記得沒錯，沿著道路然後往左走就可以到河邊看到老爸了"); break; default: EndConversation(); break; } }
    void Script_Day2_Start() { switch (currentStep) { case 0: ShowText("芝麻", "今天天氣有點糟糕，但不影響我釣魚的心情"); break; default: EndConversation(); break; } }
    void Script_Day3_Start() { switch (currentStep) { case 0: ShowText("芝麻", "今天天氣還是很糟"); break; default: EndConversation(); break; } }
    void Script_Blood() { switch (currentStep) { case 0: ShowText("芝麻", "..."); break; case 1: ShowText("芝麻", "老爸呢，跑哪去了"); break; case 2: ShowText("芝麻", "地板好像有血跡，跟著血跡走好了"); break; default: EndConversation(); break; } }
    void Script_Police() { switch (currentStep) { case 0: ShowText("芝麻", "警察?"); break; default: EndConversation(); break; } }

    // --- 工具與結束控制 ---

    public void SetHoverState(bool showCircle)
    {
        if (isTalking)
        {
            if (outerCircleObject != null && outerCircleObject.activeSelf) outerCircleObject.SetActive(false);
            if (centerDotObject != null && centerDotObject.activeSelf) centerDotObject.SetActive(false);
            return;
        }
        if (outerCircleObject != null) outerCircleObject.SetActive(showCircle);
        if (centerDotObject != null && !centerDotObject.activeSelf) centerDotObject.SetActive(true);
    }

    void ShowOptions(string textA, string textB)
    {
        if (optionA_Object) { optionA_Object.SetActive(true); optionA_Object.GetComponentInChildren<TMP_Text>().text = textA; }
        if (optionB_Object) { optionB_Object.SetActive(true); optionB_Object.GetComponentInChildren<TMP_Text>().text = textB; }
    }

    void ChooseOption(int index) { currentStep++; NextSentence(); }

    void ResetTextColor()
    {
        if (nameText) nameText.color = originalNameColor;
        if (bodyText) bodyText.color = originalBodyColor;
    }

    void EndConversation()
    {
        if (conversationID == 33) return;

        ResetTextColor();

        isTalking = false;
        dialoguePanel.SetActive(false);
        if (playerController != null) playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (centerDotObject) centerDotObject.SetActive(true);

        int day = LevelManager.Instance.currentDay;

        //Day 1 & 2 & 3 轉場邏輯總整理

        // Day 3 事件
        if (day == 3)
        {
            if (conversationID == 2) LevelManager.Instance.SetDay3Stage(1);
            else if (conversationID == 0 && LevelManager.Instance.day3Stage == 1) LevelManager.Instance.SetDay3Stage(2);
            else if (conversationID == 31) LevelManager.Instance.SetDay3Stage(3);
            else if (conversationID == 32) LevelManager.Instance.TriggerEnding();
        }
        // Day 2 結束邏輯
        else if (day == 2)
        {
            if (conversationID == 2) // 老爸上半場結束 -> 時間跳躍
            {
                LevelManager.Instance.TriggerTimeSkip(21);
            }
            else if (conversationID == 21) // 老爸下半場結束 -> 去 Day 3
            {
                LevelManager.Instance.GoToNextLevel();
            }
        }
        // Day 1 結束邏輯
        else if (day == 1)
        {
            if (conversationID == 2) // 老爸給釣竿結束 -> 去 Day 2
            {
                LevelManager.Instance.GoToNextLevel();
            }
        }
    }
}