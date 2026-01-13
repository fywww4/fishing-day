using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("場景設定")]
    public string firstLevelName = "Day1_Scene";
    public string creditsSceneName = "Credits";

    [Header("UI 面板")]
    public GameObject settingsPanel;

    [Header("語言設定")]
    public TMP_Text languageButtonText;

    [Header("靈敏度設定")]
    public Slider sensitivitySlider;      // 把 Slider 拖進來
    public TMP_Text sensitivityValueText; // (選用) 顯示數值的文字

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (settingsPanel) settingsPanel.SetActive(false);

        UpdateLanguageButtonText();
        
        // 讀取存檔的靈敏度，並更新滑桿位置
        // 預設值 100 (如果不喜歡太快，可以把這裡的 100 改小一點，例如 50)
        float savedSens = PlayerPrefs.GetFloat("Sensitivity", 100f);

        // 防呆：如果讀出來是 0，強制改成 100
        if (savedSens <= 1f) savedSens = 100f;

        if (sensitivitySlider)
        {
            sensitivitySlider.value = savedSens;
            UpdateSensitivityText(savedSens);
        }
    }

    // --- 靈敏度功能 (綁定給 Slider 的 OnValueChanged) ---
    public void SetSensitivity(float val)
    {
        // 1. 存檔
        PlayerPrefs.SetFloat("Sensitivity", val);
        PlayerPrefs.Save();

        // 2. 更新文字顯示
        UpdateSensitivityText(val);
    }

    void UpdateSensitivityText(float val)
    {
        if (sensitivityValueText)
        {
            sensitivityValueText.text = Mathf.RoundToInt(val).ToString();
        }
    }

    // --- 語言切換功能 ---
    public void ToggleLanguage()
    {
        int currentLang = PlayerPrefs.GetInt("Language", 0);
        int newLang = (currentLang == 0) ? 1 : 0;
        PlayerPrefs.SetInt("Language", newLang);
        PlayerPrefs.Save();
        UpdateLanguageButtonText();
    }

    void UpdateLanguageButtonText()
    {
        if (languageButtonText)
        {
            int lang = PlayerPrefs.GetInt("Language", 0);
            languageButtonText.text = (lang == 0) ? "語言: 中文" : "Language: English";
        }
    }

    // --- 其他按鈕功能 ---
    public void StartGame() { SceneManager.LoadScene(firstLevelName); }
    public void OpenCredits() { SceneManager.LoadScene(creditsSceneName); }
    public void QuitGame() { Application.Quit(); }
    public void OpenSettings() { if (settingsPanel) settingsPanel.SetActive(true); }
    public void CloseSettings() { if (settingsPanel) settingsPanel.SetActive(false); }
}