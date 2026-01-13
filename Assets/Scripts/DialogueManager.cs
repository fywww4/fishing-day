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

    private bool isTyping = false;
    private string currentFullText = "";
    private Coroutine typingCoroutine;

    private Color originalNameColor;
    private Color originalBodyColor;

    // 新增：語言設定 (false=中文, true=英文)
    private bool isEnglish = false;

    public MonoBehaviour playerController;

    void Awake() { Instance = this; }

    void Start()
    {
        dialoguePanel.SetActive(false);
        if (optionA_Object) optionA_Object.SetActive(false);
        if (optionB_Object) optionB_Object.SetActive(false);

        if (nameText) originalNameColor = nameText.color;
        if (bodyText) originalBodyColor = bodyText.color;

        //讀取語言設定 (0=中文, 1=英文)
        isEnglish = PlayerPrefs.GetInt("Language", 0) == 1;
    }

    // 新增：翻譯小工具
    // 用法：ShowText(GetText("芝麻", "Sesame"), GetText("你好", "Hello"));
    public string GetText(string cn, string en)
    {
        return isEnglish ? en : cn;
    }

    void Update()
    {
        if (isTalking) HandleInput();
    }

    void HandleInput()
    {
        if (isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            StopCoroutine(typingCoroutine);
            bodyText.text = currentFullText;
            isTyping = false;
            return;
        }

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

        // 確保開始對話時，重新讀取一次語言設定 (避免選單改了沒生效)
        isEnglish = PlayerPrefs.GetInt("Language", 0) == 1;

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

            case 21: Script_Dad_Day2_Part2(); break;

            case 30: Script_Day3_Start(); break;
            case 31: Script_Blood(); break;
            case 32: Script_Police(); break;
            case 33: Script_Ending(); break;
        }
    }

    // --- 顯示文字 (打字機核心) ---
    void ShowText(string name, string content)
    {
        nameText.text = name;
        currentFullText = content;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        AudioClip voice = null;
        if (name.Contains("芝麻") || name.Contains("Sesame")) voice = zhimaClip;
        else if (name.Contains("老爸") || name.Contains("Dad") || name.Contains("老人") || name.Contains("Old Man") || name.Contains("Father")) voice = dadClip;
        else if (name.Contains("狗") || name.Contains("Dog")) voice = dogClip;
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

    // --- 劇本區 (中英雙語生動版) ---

    void Script_Dog()
    {
        switch (currentStep)
        {
            case 0: ShowText(GetText("黑狗", "Dog"), GetText("汪汪汪！", "Woof! Woof!")); break;
            case 1:
                ShowText(GetText("芝麻", "Sesame"), "...");
                ShowOptions(GetText("[A] 乖狗狗", "[A] Good boy"), GetText("[B] 滾開", "[B] Scram!"));
                break;
            default: EndConversation(); break;
        }
    }

    void Script_OldMan()
    {
        int day = LevelManager.Instance.currentDay;
        int stage = LevelManager.Instance.day3Stage;
        string name = GetText("老人", "Old Man");
        string sesame = GetText("芝麻", "Sesame");

        if (day == 1)
        {
            switch (currentStep)
            {
                case 0: ShowText(name, GetText("最近如何啊", "How's it going?")); break;
                case 1: ShowText(sesame, GetText("還可以啦", "Getting by, I guess.")); break;
                case 2:
                    ShowText(name, GetText("你跟你阿爸一起釣魚去了?", "Gone fishing with your pops?"));
                    ShowOptions(GetText("[A] 對阿 今天可不能空手而歸", "[A] Yup. Can't go home empty-handed."),
                                GetText("[B] 乾你屁事啊 老東西", "[B] None of your business, old geezer.")); break;
                case 3: ShowText(name, GetText("唉呦 自己記得要吃飽飯阿", "Oh my. Just remember to eat well, kid.")); break;
                default: EndConversation(); break;
            }
        }
        else if (day == 2)
        {
            switch (currentStep)
            {
                case 0: ShowText(name, GetText("芝麻，這個給你，路上一個奇怪的人給我的，還對我說聖誕節快樂", "Sesame, take this. Some weirdo gave it to me on the road. Said 'Merry Christmas(聖誕節)'.")); break;
                case 1: ShowText(sesame, GetText("聖誕帽?", "Christmas hat?")); break;
                case 2: ShowText(sesame, GetText("但是政府好像有說，應該說耶誕節快樂", "But didn't the government say we should call it 'Merry Christmas(耶誕節)'?")); break;
                case 3: ShowText(sesame, GetText("謝謝阿 阿伯", "Thanks, Uncle.")); break;
                default: EndConversation(); break;
            }
        }
        else if (day == 3)
        {
            if (stage == 0)
            {
                switch (currentStep)
                {
                    case 0: ShowText(sesame, GetText("(老頭在睡覺...)", "(The old man is asleep...)")); break;
                    default: EndConversation(); break;
                }
            }
            else if (stage == 1)
            {
                switch (currentStep)
                {
                    case 0: ShowText(sesame, GetText("阿伯，這是給你的，我父親說要跟感謝你的幫忙", "Uncle, this is for you. Dad wanted to thank you for your help.")); break;
                    case 1: ShowText(name, GetText("喔，免啦", "Oh, nah, don't worry about it.")); break;
                    case 2: ShowText(sesame, GetText("放這邊啦，掰掰", "Just putting it here. Bye!")); break;
                    case 3: ShowText(name, GetText("多謝啦", "Thanks a bunch.")); break;
                    default: EndConversation(); break;
                }
            }
            else
            {
                switch (currentStep)
                {
                    case 0: ShowText(name, GetText("(繼續睡覺...)", "(Continues sleeping...)")); break;
                    default: EndConversation(); break;
                }
            }
        }
    }

    void Script_Dad()
    {
        int day = LevelManager.Instance.currentDay;
        string dad = GetText("老爸", "Dad"); // Day 1 使用
        string father = GetText("父親", "Father"); // Day 2/3 使用
        string sesame = GetText("芝麻", "Sesame");

        if (day == 1)
        {
            switch (currentStep)
            {
                case 0: ShowText(dad, GetText("你來了阿", "You made it.")); break;
                case 1: ShowText(dad, GetText("你的魚竿在這裡", "Here's your fishing rod.")); break;
                case 2: ShowText(sesame, GetText("釣魚喽", "Let's fish!")); break;
                case 3: ShowText(dad, GetText("差不多了，明天再來吧，回去吃飯了阿", "That's about it. Let's come back tomorrow. Time to eat.")); break;
                case 4: ShowText(sesame, GetText("好啊", "Sure.")); break;
                default: EndConversation(); break;
            }
        }
        else if (day == 2)
        {
            switch (currentStep)
            {
                case 0: ShowText(sesame, GetText("哭阿，太恐怖了吧", "Damn! That looks terrifying.")); break;
                case 1: ShowText(father, GetText("我怎麼知道阿，魚長的都很醜", "How should I know? All fish look ugly.")); break;
                case 2: ShowText(father, GetText("看看這魚回去能不能賣個好價錢", "Let's see if this fetches a good price.")); break;
                case 3: ShowText(sesame, GetText("這些魚直接丟回去啦", "Just throw them back, seriously.")); break;
                case 4: ShowText(father, GetText("免啦，搞不好是稀有的魚", "Nah, might be a rare species.")); break;
                default: EndConversation(); break;
            }
        }
        else if (day == 3)
        {
            switch (currentStep)
            {
                case 0: ShowText(father, GetText("太慢了吧，今天讚的哩，來個最後收尾", "Took you long enough! Today's looking great. Let's wrap this up.")); break;
                case 1: ShowText(father, GetText("你幫我把這些拿去給阿伯，算是跟他分享一下我們的收穫", "Take these to the Uncle. Share our catch with him.")); break;
                case 2: ShowText(sesame, GetText("(獲得了一些魚)", "(Received fish)")); break;
                case 3: ShowText(sesame, GetText("記得要回來啊", "Remember to come back")); break;
                default: EndConversation(); break;
            }
        }
    }

    void Script_Dad_Day2_Part2()
    {
        string father = GetText("父親", "Father");
        string sesame = GetText("芝麻", "Sesame");
        switch (currentStep)
        {
            case 0: ShowText(father, GetText("哭邀，太雖小了吧，明天再這樣就準備換地點吧", "Crap, what rotten luck. If it's like this tomorrow, we're moving spots.")); break;
            case 1: ShowText(sesame, GetText("好", "Okay.")); break;
            default: EndConversation(); break;
        }
    }

    void Script_Day1_Start()
    {
        string sesame = GetText("芝麻", "Sesame");
        switch (currentStep)
        {
            case 0: ShowText(sesame, GetText("美好的一天從釣魚開始", "A beautiful day begins with fishing.")); break;
            case 1: ShowText(sesame, GetText("我記得沒錯，沿著道路然後往左走就可以到河邊看到阿爸了", "If I remember right, follow the road, turn left, and I'll find Dad by the river.")); break;
            default: EndConversation(); break;
        }
    }

    void Script_Day2_Start()
    {
        switch (currentStep)
        {
            case 0: ShowText(GetText("芝麻", "Sesame"), GetText("天氣有點糟糕", "Weather's a bit crap...")); break;
            default: EndConversation(); break;
        }
    }

    void Script_Day3_Start()
    {
        switch (currentStep)
        {
            case 0: ShowText(GetText("芝麻", "Sesame"), GetText("今天天氣太糟了吧", "The weather is just awful today.")); break;
            default: EndConversation(); break;
        }
    }

    void Script_Blood()
    {
        string sesame = GetText("芝麻", "Sesame");
        switch (currentStep)
        {
            case 0: ShowText(sesame, "..."); break;
            case 1: ShowText(sesame, GetText("阿爸呢，跑哪去了", "Where's Dad? Where'd he run off to?")); break;
            case 2: ShowText(sesame, GetText("發生什麼事情了阿，為何都是血阿阿阿阿阿?!", "What the hell happened?! Why is there blood everywhere RRRRRRRR?!")); break;
            case 3: ShowText(sesame, GetText("地板好像有血跡，跟著血跡走好了", "Looks like blood on the ground. I should follow it.")); break;
            default: EndConversation(); break;
        }
    }

    void Script_Police()
    {
        switch (currentStep)
        {
            case 0: ShowText(GetText("芝麻", "Sesame"), GetText("條子?", "Cops?")); break;
            default: EndConversation(); break;
        }
    }

    void Script_Ending()
    {
        nameText.color = Color.white; bodyText.color = Color.white;
        string mystery = "???";
        switch (currentStep)
        {
            case 0: ShowText(mystery, GetText("抓到同夥", "Got the accomplice.")); break;
            case 1: ShowText(mystery, GetText("帶回去", "Take 'em in.")); break;
            case 2: ShowText(mystery, GetText("可是他只是一個小孩欸", "But he's just a kid!")); break;
            case 3: ShowText(mystery, GetText("共匪不分年齡別忘記了", "Don't forget, commie spies come in all ages.")); break;
            case 4: ShowText(mystery, GetText("等等那是什麼", "Wait, what the hell is that?")); break;
            case 5:
                bodyText.color = Color.red;
                ShowText(mystery, GetText("你不要過來啊!!!", "Stay away!!!"));
                break;
            default: if (LevelManager.Instance) LevelManager.Instance.TriggerEnding(); break;
        }
    }

    // --- 工具區 ---

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

    void ShowOptions(string textA, string textB) { if (optionA_Object) { optionA_Object.SetActive(true); optionA_Object.GetComponentInChildren<TMP_Text>().text = textA; } if (optionB_Object) { optionB_Object.SetActive(true); optionB_Object.GetComponentInChildren<TMP_Text>().text = textB; } }
    void ChooseOption(int index) { currentStep++; NextSentence(); }
    void ResetTextColor() { if (nameText) nameText.color = originalNameColor; if (bodyText) bodyText.color = originalBodyColor; }
    void EndConversation() { if (conversationID == 33) return; ResetTextColor(); isTalking = false; dialoguePanel.SetActive(false); if (playerController != null) playerController.enabled = true; Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; if (centerDotObject) centerDotObject.SetActive(true); int day = LevelManager.Instance.currentDay; if (day == 3) { if (conversationID == 2) LevelManager.Instance.SetDay3Stage(1); else if (conversationID == 0 && LevelManager.Instance.day3Stage == 1) LevelManager.Instance.SetDay3Stage(2); else if (conversationID == 31) LevelManager.Instance.SetDay3Stage(3); else if (conversationID == 32) LevelManager.Instance.TriggerEnding(); } else if (day == 2) { if (conversationID == 2) LevelManager.Instance.TriggerTimeSkip(21); else if (conversationID == 21) LevelManager.Instance.GoToNextLevel(); } else if (day == 1) { if (conversationID == 2) LevelManager.Instance.GoToNextLevel(); } }
}