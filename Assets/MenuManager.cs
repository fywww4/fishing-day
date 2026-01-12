using UnityEngine;
using UnityEngine.SceneManagement; // 用來切換場景
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("場景設定")]
    public string firstLevelName = "Day1_Scene"; // 第一關場景名稱
    public string creditsSceneName = "Credits";  //新增：製作人員名單場景名稱

    [Header("UI 面板")]
    public GameObject settingsPanel; // 設定面板 (齒輪)

    void Start()
    {
        // 確保滑鼠可見
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (settingsPanel) settingsPanel.SetActive(false);
    }

    // --- 按鈕功能 ---

    public void StartGame()
    {
        SceneManager.LoadScene(firstLevelName);
    }

    //修改：直接載入 Credits 場景
    public void OpenCredits()
    {
        SceneManager.LoadScene(creditsSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("退出遊戲");
        Application.Quit();
    }

    // 設定頁面控制
    public void OpenSettings()
    {
        if (settingsPanel) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel) settingsPanel.SetActive(false);
    }
}