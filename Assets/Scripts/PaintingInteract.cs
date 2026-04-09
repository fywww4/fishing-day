using UnityEngine;

public class PaintingInteract : MonoBehaviour
{
    [Header("地板上的觸發點物件")]
    public ChairPlaceTarget placementTrigger;

    [Header("音效設定")]
    public AudioSource audioSource;
    public AudioClip dropSound;     // 畫作掉落撞擊聲

    private Rigidbody rb;
    private bool hasFallen = false; // 紀錄是否已經掉落，避免重複播放聲音

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    public void InteractPainting()
    {
        // 如果已經掉下來了，就直接 Return，什麼都不做
        if (hasFallen) return;

        // 只有當觸發點的 isPlaced 為 true，才准掉落
        if (placementTrigger != null && placementTrigger.isPlaced)
        {
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(transform.forward * 2f, ForceMode.Impulse);
            }
            // 這裡先不要 enabled = false，讓畫作能去執行下面的 OnCollisionEnter
        }
        else
        {
            // 沒放椅子，跳出劇情對話 (ID 48)
            DialogueManager.Instance.StartConversation(48);
        }
    }

    //  新增：碰撞偵測，當畫撞到任何東西(地板)時觸發
    void OnCollisionEnter(Collision collision)
    {
        // 如果是受到重力掉落中，且還沒發出過聲音
        if (!hasFallen && rb != null && !rb.isKinematic)
        {
            // 播放掉落音效
            if (audioSource != null && dropSound != null)
            {
                audioSource.PlayOneShot(dropSound);
            }

            hasFallen = true;     // 標記為已經掉落
            this.enabled = false; // 聲音播完了，任務達成，把腳本關掉不再互動
        }
    }
}