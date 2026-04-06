using UnityEngine;

public class HammerTool : MonoBehaviour
{
    [Header("設定")]
    public Transform handAnchor;      // 拖入剛才建立的 HandAnchor
    public float throwForce = 5f;     // 丟掉時的力量

    private Rigidbody rb;
    private Collider col;
    private bool isHeld = false;      // 是否正被拿著
    public static bool IsHoldingHammer = false; // 讓全世界知道玩家現在有沒有拿槌子
    private static bool hasPlayedDialogue = false; // 記錄是否說過話了

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void Update()
    {
        // 丟棄功能
        if (isHeld && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }

        // 揮動功能 (左鍵攻擊)
        if (isHeld && Input.GetMouseButtonDown(0))
        {
            Swing();
        }
    }

    // 拾取方法 (由 PlayerInteract 呼叫)
    public void PickUp(Transform anchor)
    {
        if (rb == null) return;

        isHeld = true;
        IsHoldingHammer = true; // 狀態設為「拿著」
        handAnchor = anchor;

        rb.isKinematic = true;
        if (col != null) col.isTrigger = true;

        transform.SetParent(handAnchor);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(90, 270, 0);

        
        if (!hasPlayedDialogue)
        {
            DialogueManager.Instance.StartConversation(46);
            hasPlayedDialogue = true;
        }
    }

    // 丟棄方法
    public void Drop()
    {
        isHeld = false;
        IsHoldingHammer = false; // 狀態設為「沒拿」
        transform.SetParent(null);
        rb.isKinematic = false;
        if (col != null) col.isTrigger = false;
        rb.AddForce(handAnchor.forward * throwForce, ForceMode.Impulse);
    }

    void Swing()
    {
        // 這裡可以播放揮動動畫
        Debug.Log("揮動槌子！");
        // 簡單做法：可以用程式碼讓槌子轉一下再轉回來，或直接觸發 Animator
    }

    // 偵測撞擊木板
    private void OnTriggerEnter(Collider other)
    {
        if (isHeld && other.CompareTag("Wood"))
        {
            // 撞到木板了！
            Destroy(other.gameObject);
            Debug.Log("木板破裂！");
            // 這裡可以加入播放木頭碎裂聲或特效
        }
    }
}