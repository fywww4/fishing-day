using UnityEngine;
using TMPro;

public class KeypadController : MonoBehaviour
{

    public static KeypadController Instance;
    public bool isSolved = false;

    [Header("UI 設定")]
    public GameObject keypadUI;       // 記得把你的 Panel 拖拉到這個欄位裡！
    public TMP_Text displayScreen;    // 上方的螢幕文字
    public string correctPassword = "1234"; // 設定你的過關密碼

    private string currentInput = ""; // 記錄目前的輸入

    void Start()
    {
        displayScreen.text = ""; 
        keypadUI.SetActive(false); // 確保遊戲開始時 UI 是關閉的
    }

    void Awake()
    {
        Instance = this;
    }

    // 💡 當玩家點擊場景中這個 3D 密碼鎖時呼叫
    public void Interact()
    {
        // 直接呼叫 DialogueManager 播放 ID 40 的劇本！
        DialogueManager.Instance.StartConversation(45);
        Debug.Log("玩家點到密碼鎖了！"); // 加入這行測試
    }

    public void OpenKeypad()
    {
        keypadUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 🌟 這裡要非常明確地指名道姓
        if (DialogueManager.Instance != null && DialogueManager.Instance.centerDotObject != null)
        {
            DialogueManager.Instance.centerDotObject.SetActive(false);
            Debug.Log("準心應該要被關掉了！"); // 加這行可以在 Console 視窗確認有沒有執行
        }

        if (DialogueManager.Instance.playerController != null)
            DialogueManager.Instance.playerController.enabled = false;
    }

    public void CloseKeypad()
    {
        keypadUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //離開時再把它還原
        if (DialogueManager.Instance != null && DialogueManager.Instance.centerDotObject != null)
        {
            DialogueManager.Instance.centerDotObject.SetActive(true);
        }

        if (DialogueManager.Instance.playerController != null)
            DialogueManager.Instance.playerController.enabled = true;
    }

    public void AddNumber(string number)
    {
        currentInput += number; 
        displayScreen.text = currentInput; 
    }

    public void CheckPassword()
    {
        if (currentInput == correctPassword)
        {
            isSolved = true; // 🌟 密碼正確，標記為成功！
            displayScreen.text = "OPEN";
            // 這裡可以加個開鎖成功的音效
            Invoke("CloseKeypad", 1f);
        }
        else
        {
            currentInput = "";
            displayScreen.text = "ERROR";
        }
    }

    public void ClearInput()
    {
        currentInput = "";
        displayScreen.text = "";
        displayScreen.color = Color.white;
    }

    private void ResetScreen()
    {
        displayScreen.text = "";
        displayScreen.color = Color.white;
    }


}