using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("UI 參考")]
    public GameObject pausePanel;
    public Slider sensitivitySlider;

    private bool isPaused = false;

    //  1. 改成宣告你的「移動控制腳本」
    private FirstPersonController playerMovement;

    void Start()
    {
        pausePanel.SetActive(false);

        //  2. 尋找場景中的移動控制腳本
        playerMovement = FindObjectOfType<FirstPersonController>();

        if (sensitivitySlider != null)
        {
            float savedSens = PlayerPrefs.GetFloat("Sensitivity", 100f);
            sensitivitySlider.value = savedSens;
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetSensitivity(float val)
    {
        PlayerPrefs.SetFloat("Sensitivity", val);

        //  3. 更新移動腳本裡的靈敏度數值
        if (playerMovement != null)
        {
            playerMovement.mouseSensitivity = val;
        }
    }
}