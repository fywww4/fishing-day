using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance; // 單例模式，方便呼叫

    [Header("UI 組件")]
    public GameObject dialoguePanel;
    public Text nameText;
    public Text bodyText;
    public GameObject optionPanel; // 如果你有把按鈕包在一起，或者直接控制按鈕
    public Button optionA;
    public Button optionB;

    [Header("準心控制")]
    public Image crosshairImage;
    public Sprite defaultIcon;
    public Sprite interactIcon; // 圓球圖示

    // 狀態
    private bool isTalking = false;
    private int currentStep = 0; // 對話進度

    // 為了你的 FPS 控制器
    public MonoBehaviour playerController;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        dialoguePanel.SetActive(false);
        optionA.gameObject.SetActive(false);
        optionB.gameObject.SetActive(false);

        // 設定按鈕監聽
        optionA.onClick.AddListener(() => ChooseOption(0));
        optionB.onClick.AddListener(() => ChooseOption(1));
    }

    // 外部呼叫：變更準心
    public void SetHoverState(bool isHovering)
    {
        if (isTalking) return; // 對話中不變
        crosshairImage.sprite = isHovering ? interactIcon : defaultIcon;
        // 如果想要變大變小也可以在這裡寫 rectTransform.sizeDelta
    }

    // 外部呼叫：開始對話
    public void StartConversation()
    {
        if (isTalking) return;

        isTalking = true;
        dialoguePanel.SetActive(true);
        currentStep = 0;

        // 1. 鎖住玩家移動 & 釋放滑鼠
        if (playerController != null) playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        NextSentence();
    }

    // 處理對話流
    public void NextSentence()
    {
        optionA.gameObject.SetActive(false);
        optionB.gameObject.SetActive(false);

        switch (currentStep)
        {
            case 0:
                ShowText("老人", "中午好，芝麻，最近如何啊");
                break;
            case 1:
                ShowText("芝麻", "很不錯");
                break;
            case 2:
                ShowText("老人", "又去找你老爸一起釣魚去了?");
                // 這裡要跳出選項
                optionA.gameObject.SetActive(true);
                optionB.gameObject.SetActive(true);
                optionA.GetComponentInChildren<Text>().text = "A. 對阿 今天可不能空手而歸";
                optionB.GetComponentInChildren<Text>().text = "B. 乾你屁事啊 老東西";
                break;
            case 3:
                // 結束或回應
                ShowText("老人", "呵呵 祝你好運");
                break;
            default:
                EndConversation();
                return;
        }
    }

    // 按鈕點擊後
    void ChooseOption(int index)
    {
        if (currentStep == 2) // 在選項階段
        {
            if (index == 0) // 選 A
            {
                currentStep = 3; // 去下一句
                NextSentence();
            }
            else // 選 B
            {
                // 如果選 B 也是同一句回覆，就一樣去 3
                // 或者是直接不爽結束
                EndConversation();
            }
        }
    }

    // 點擊對話框繼續 (如果不是在選選項)
    public void OnClickDialogueBox()
    {
        // 如果現在顯示選項中，點對話框無效
        if (optionA.gameObject.activeSelf && currentStep == 2) return;

        // 如果不是選項階段，就繼續
        if (currentStep != 2)
        {
            currentStep++;
            NextSentence();
        }
    }

    void ShowText(string name, string content)
    {
        nameText.text = name;
        bodyText.text = content;
    }

    void EndConversation()
    {
        isTalking = false;
        dialoguePanel.SetActive(false);

        // 恢復移動
        if (playerController != null) playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 讓按鈕或隱形按鈕呼叫用
    public void ContinueButton()
    {
        if (!optionA.gameObject.activeSelf) // 沒選項時才能點擊繼續
        {
            currentStep++;
            NextSentence();
        }
    }
}