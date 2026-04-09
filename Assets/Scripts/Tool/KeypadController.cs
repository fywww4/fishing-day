using UnityEngine;
using TMPro;

public class KeypadController : MonoBehaviour
{
    public static KeypadController Instance;
    public bool isSolved = false;

    [Header("UI 設定")]
    public GameObject keypadUI;
    public TMP_Text displayScreen;
    public string correctPassword = "1234";

    private string currentInput = "";

    [Header("音效設定")]
    public AudioSource audioSource;
    public AudioClip buttonPressSound; // 按數字鍵的聲音
    public AudioClip successSound;     // 密碼正確
    public AudioClip failSound;        // 密碼錯誤

    void Start()
    {
        displayScreen.text = "";
        keypadUI.SetActive(false);
    }

    void Awake()
    {
        Instance = this;
    }

    public void Interact()
    {
        DialogueManager.Instance.StartConversation(45);
    }

    public void OpenKeypad()
    {
        keypadUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (DialogueManager.Instance != null && DialogueManager.Instance.centerDotObject != null)
        {
            DialogueManager.Instance.centerDotObject.SetActive(false);
        }

        if (DialogueManager.Instance.playerController != null)
            DialogueManager.Instance.playerController.enabled = false;
    }

    public void CloseKeypad()
    {
        keypadUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (DialogueManager.Instance != null && DialogueManager.Instance.centerDotObject != null)
        {
            DialogueManager.Instance.centerDotObject.SetActive(true);
        }

        if (DialogueManager.Instance.playerController != null)
            DialogueManager.Instance.playerController.enabled = true;
    }

    // 修改：加入按鍵音效
    public void AddNumber(string number)
    {
        if (isSolved) return; // 如果解開了就不要再按了

        currentInput += number;
        displayScreen.text = currentInput;

        // 播放按鍵音
        if (audioSource && buttonPressSound)
            audioSource.PlayOneShot(buttonPressSound);
    }

    // 🌟 修改：加入成功與失敗音效
    public void CheckPassword()
    {
        if (currentInput == correctPassword)
        {
            isSolved = true;
            displayScreen.text = "OPEN";
            displayScreen.color = Color.green; // 變綠色更好看

            // 播放成功音效
            if (audioSource && successSound)
                audioSource.PlayOneShot(successSound);

            Invoke("CloseKeypad", 1.5f); // 稍微留點時間讓玩家聽完聲音
        }
        else
        {
            // 播放失敗音效
            if (audioSource && failSound)
                audioSource.PlayOneShot(failSound);

            displayScreen.text = "ERROR";
            displayScreen.color = Color.red; // 變紅色提醒玩家

            currentInput = "";
            Invoke("ResetScreen", 1f); // 1秒後重置螢幕
        }
    }

    public void ClearInput()
    {
        currentInput = "";
        displayScreen.text = "";
        displayScreen.color = Color.white;

        // 也可以在這裡加一個小小的清除音效
        if (audioSource && buttonPressSound)
            audioSource.PlayOneShot(buttonPressSound);
    }

    private void ResetScreen()
    {
        if (isSolved) return;
        displayScreen.text = "";
        displayScreen.color = Color.white;
    }
}