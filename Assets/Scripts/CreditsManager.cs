using UnityEngine;
using UnityEngine.SceneManagement; // 如果你想做"回到標題"功能會用到

public class CreditsManager : MonoBehaviour
{
    [Header("設定")]
    public string menuSceneName = "Menu"; //主選單的場景名稱
    void Start()
    {
        //關鍵步驟：進入此場景時，強制解鎖滑鼠 
        Cursor.lockState = CursorLockMode.None; // 解除鎖定
        Cursor.visible = true;                  // 顯示滑鼠
    }

    // 給按鈕呼叫的函式
    public void QuitGame()
    {
        Debug.Log("玩家按下退出，遊戲關閉");
        Application.Quit();
    }

    // (選用) 如果你想做回到標題畫面
    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}