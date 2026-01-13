using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DogEvent : MonoBehaviour
{
    [Header("目標與參數")]
    public Transform player;
    public float stopDistance = 3.0f;

    [Header("視角鎖定設定")]
    public Transform lookAtTarget;
    public float heightOffset = 0.5f;

    [Header("組件引用")]
    public NavMeshAgent agent;
    public Animator anim;
    public FirstPersonController playerScript;
    public Transform playerCamera;

    [Header("音效設定 (新功能)")]
    public AudioSource audioSource;   // 狗身上的喇叭
    public AudioClip scaryMusicClip;  // 恐怖音樂檔案

    private bool hasTriggered = false;

    // 當這個腳本被 StoryTrigger 開啟時，Start 會執行一次
    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponent<Animator>();

        //新增：腳本一啟動 (代表玩家踩到機關了)，馬上播音樂
        PlayMusic();
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

    void PlayMusic()
    {
        if (audioSource != null && scaryMusicClip != null)
        {
            audioSource.clip = scaryMusicClip;
            audioSource.loop = true; // 設為循環播放 (怕狗跑太久音樂停了)
            audioSource.Play();      // 開始播放
        }
    }

    void TriggerStory()
    {
        hasTriggered = true;

        //新增：在對話開始前，卡掉音樂
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        // 停止移動與動畫
        if (agent != null) agent.isStopped = true;
        if (anim != null) anim.SetBool("IsRunning", false);

        if (playerScript != null) playerScript.enabled = false;

        // 視角鎖定
        Vector3 targetPosition;
        if (lookAtTarget != null) targetPosition = lookAtTarget.position;
        else targetPosition = transform.position + Vector3.up * heightOffset;

        Vector3 directionToDog = targetPosition - playerCamera.position;
        playerCamera.rotation = Quaternion.LookRotation(directionToDog);

        // 呼叫對話 (ID 1 = 狗狗)
        DialogueManager.Instance.StartConversation(1);
    }
}