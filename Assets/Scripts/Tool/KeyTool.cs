using UnityEngine;

public class KeyTool : MonoBehaviour
{
    // 供外部 (如 PlayerInteract) 讀取的狀態
    public static bool IsHoldingKey = false;

    private Rigidbody rb;
    private Collider col;
    private bool isHeld = false;
    private Transform handAnchor;
    private bool hasPlayedDialogue = false; // 紀錄是否講過話了

    [Header("音效設定")]
    public AudioSource audioSource;
    public AudioClip pickupSound;   // 拿取音效
    public AudioClip dropSound;     // 掉落音效

    // 紀錄是否剛被丟出去，用來觸發掉落聲音
    private bool isDroppedAndFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    // ---  撿起鑰匙 ---
    public void PickUp(Transform anchor)
    {
        if (rb == null) return;

        // 1. 播放拿取音效 (新增)
        if (audioSource != null && pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        // 2. 狀態與物理設定 (你原本的邏輯)
        isHeld = true;
        IsHoldingKey = true;
        handAnchor = anchor;

        rb.isKinematic = true;
        if (col != null) col.isTrigger = true; // 拿在手上變成幽靈

        // 3. 綁定到手上
        transform.SetParent(handAnchor);
        transform.localPosition = Vector3.zero;

        //  4. 你要求的特定旋轉角度
        transform.localRotation = Quaternion.Euler(0, 90, 0);

        //  5. 觸發劇情對話 (只觸發一次)
        if (!hasPlayedDialogue)
        {
            DialogueManager.Instance.StartConversation(47);
            hasPlayedDialogue = true;
        }
    }

    // ---  丟棄鑰匙 ---
    public void Drop()
    {
        if (!isHeld) return; // 防呆：沒拿在手上就不能丟

        isHeld = false;
        IsHoldingKey = false;
        transform.SetParent(null); // 解除與手的綁定

        isDroppedAndFalling = true; // 開啟掉落偵測開關！

        if (rb != null)
        {
            rb.isKinematic = false;
            // 讓它沿著視角前方加上一點往上的力道拋出
            Vector3 dropDirection = Camera.main.transform.forward + Vector3.up * 0.5f;
            rb.AddForce(dropDirection * 2f, ForceMode.Impulse);
        }

        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = false; //  關鍵：關閉 Trigger，變回實體，避免掉入虛空！
        }
    }

    // --- 碰撞偵測 (掉落音效) ---
    void OnCollisionEnter(Collision collision)
    {
        // 如果是剛被丟出去的狀態，撞到東西就發出聲音
        if (isDroppedAndFalling)
        {
            if (audioSource != null && dropSound != null)
            {
                audioSource.PlayOneShot(dropSound);
            }

            // 聲音播完了，把開關關掉，這樣它在地上被踢到才不會一直發出聲音
            isDroppedAndFalling = false;
        }
    }
}