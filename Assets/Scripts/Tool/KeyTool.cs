using UnityEngine;

public class KeyTool : MonoBehaviour
{
    public static bool IsHoldingKey = false; // 讓全世界知道玩家現在有沒有拿鑰匙
    private static bool hasPlayedDialogue = false; // 記錄是否說過話 (如果需要的話)

    [Header("設定")]
    public Transform handAnchor;
    public float throwForce = 5f;

    private Rigidbody rb;
    private Collider col;
    private bool isHeld = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void Update()
    {
        // 按 Q 丟掉鑰匙
        if (isHeld && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }

    public void PickUp(Transform anchor)
    {
        if (rb == null) return;

        isHeld = true;
        IsHoldingKey = true; //標記為拿起鑰匙
        handAnchor = anchor;

        rb.isKinematic = true;
        if (col != null) col.isTrigger = true;

        transform.SetParent(handAnchor);
        transform.localPosition = Vector3.zero;

        
        transform.localRotation = Quaternion.Euler(0, 90, 0);

        
        if (!hasPlayedDialogue)
        {
            DialogueManager.Instance.StartConversation(47);
            hasPlayedDialogue = true;
        }
    }

    public void Drop()
    {
        isHeld = false;
        IsHoldingKey = false; //標記為丟掉鑰匙
        transform.SetParent(null);

        rb.isKinematic = false;
        if (col != null) col.isTrigger = false;

        rb.AddForce(handAnchor.forward * throwForce, ForceMode.Impulse);
    }
}