using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DogEvent : MonoBehaviour
{
    [Header("目標與參數")]
    public Transform player;
    public float stopDistance = 3.0f;
    public GameObject dialogueUI;

    [Header("視角鎖定設定 (新功能)")]
    // 這裡可以拖曳狗的頭部骨架，或者留空(程式會自動抓頭頂)
    public Transform lookAtTarget;
    public float heightOffset = 0.5f; // 如果不拖物件，就用這個高度來補償

    [Header("組件引用")]
    public NavMeshAgent agent;
    public Animator anim;
    public FirstPersonController playerScript;
    public Transform playerCamera;

    private bool hasTriggered = false;

    void Start()
    {
        if (dialogueUI != null) dialogueUI.SetActive(false);
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (hasTriggered) return;

        agent.SetDestination(player.position);

        if (agent.velocity.magnitude > 0.1f)
            anim.SetBool("IsRunning", true);
        else
            anim.SetBool("IsRunning", false);

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= stopDistance)
        {
            TriggerStory();
        }
    }

    void TriggerStory()
    {
        hasTriggered = true;

        // 停止移動
        agent.isStopped = true;
        anim.SetBool("IsRunning", false);

        // 鎖定玩家操作
        if (playerScript != null) playerScript.enabled = false;

        // --- 修正後的視角鎖定邏輯 ---
        Vector3 targetPosition;

        if (lookAtTarget != null)
        {
            // 1. 如果你有指定要看「頭部骨架」，就看骨架
            targetPosition = lookAtTarget.position;
        }
        else
        {
            // 2. 如果沒指定，就看「狗的腳底 + 往上墊高 0.5 公尺」
            targetPosition = transform.position + Vector3.up * heightOffset;
        }

        // 計算方向
        Vector3 directionToDog = targetPosition - playerCamera.position;
        // 瞬間轉頭
        playerCamera.rotation = Quaternion.LookRotation(directionToDog);

        // 顯示 UI
        if (dialogueUI != null) dialogueUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}