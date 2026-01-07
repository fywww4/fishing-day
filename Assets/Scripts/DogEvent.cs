using UnityEngine;
using UnityEngine.AI; // 引用導航系統
using UnityEngine.UI; // 引用 UI 系統

public class DogEvent : MonoBehaviour
{
    [Header("目標與參數")]
    public Transform player;          // 把你的 Player 拖進來
    public float stopDistance = 3.0f; // 距離多近會觸發劇情
    public GameObject dialogueUI;     // 把做好的 UI Panel 拖進來

    [Header("組件引用")]
    public NavMeshAgent agent;
    public Animator anim;
    public FirstPersonController playerScript; // 把玩家身上的移動腳本拖進來
    public Transform playerCamera;    // 把玩家的 Main Camera 拖進來

    private bool hasTriggered = false; // 確保劇情只觸發一次

    void Start()
    {
        // 遊戲開始，先關閉 UI
        if (dialogueUI != null) dialogueUI.SetActive(false);
    }

    void Update()
    {
        if (hasTriggered) return; // 如果劇情觸發過了，就什麼都不做

        // 1. 讓狗追玩家
        agent.SetDestination(player.position);

        // 2. 判斷是否在移動，控制動畫
        // agent.velocity.magnitude 是狗目前的移動速度
        if (agent.velocity.magnitude > 0.1f)
        {
            anim.SetBool("IsRunning", true);
        }
        else
        {
            anim.SetBool("IsRunning", false);
        }

        // 3. 計算狗跟玩家的距離
        float distance = Vector3.Distance(transform.position, player.position);

        // 4. 如果距離夠近，觸發劇情
        if (distance <= stopDistance)
        {
            TriggerStory();
        }
    }

    void TriggerStory()
    {
        hasTriggered = true; // 標記已觸發

        // A. 狗停下來
        agent.isStopped = true;
        anim.SetBool("IsRunning", false); // 切回站立動畫

        // B. 玩家不能動 (關閉你的移動腳本)
        if (playerScript != null) playerScript.enabled = false;

        // C. 視角強制鎖定看著狗
        // 算出從攝影機指向狗的方向
        Vector3 directionToDog = transform.position - playerCamera.position;
        // 讓攝影機直接轉向那個方向
        playerCamera.rotation = Quaternion.LookRotation(directionToDog);

        // D. 顯示 UI
        if (dialogueUI != null) dialogueUI.SetActive(true);

        // E. 顯示滑鼠游標 (這樣你才能點擊 UI 關閉它，如果有做按鈕的話)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}