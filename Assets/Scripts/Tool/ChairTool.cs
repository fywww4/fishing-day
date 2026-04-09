using UnityEngine;

public class ChairTool : MonoBehaviour
{
    public static bool IsHoldingChair = false; // 記錄是否拿著椅子

    [Header("設定")]
    public Transform handAnchor;
    public float throwForce = 2f;

    private Rigidbody rb;
    private Collider col;
    private bool isHeld = false;

    [Header("音效設定")]
    public AudioSource audioSource;
    public AudioClip dropSound;

    // 新增：紀錄這把工具是不是「剛被玩家丟出來還沒落地」
    private bool isDroppedAndFalling = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void Update()
    {
        if (isHeld && Input.GetKeyDown(KeyCode.Q)) Drop();
    }

    public void PickUp(Transform anchor)
    {
        if (rb == null) return;

        isHeld = true;
        IsHoldingChair = true;
        handAnchor = anchor;

        rb.isKinematic = true;
        if (col != null) col.isTrigger = true;

        transform.SetParent(handAnchor);
        transform.localPosition = Vector3.zero;

        // 椅子拿在手上的角度 (如果拿起來擋住視線，可以調一下這裡)
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void Drop()
    {
        IsHoldingChair = false;
        transform.SetParent(null);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            Vector3 dropDirection = Camera.main.transform.forward + Vector3.up * 0.5f;
            rb.AddForce(dropDirection * 2f, ForceMode.Impulse);
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        //  標記：它現在在半空中飛了！
        isDroppedAndFalling = true;
        col.isTrigger = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 只有在「被丟出且還沒落地」的狀態下，撞到東西才發出聲音
        if (isDroppedAndFalling)
        {
            if (audioSource != null && dropSound != null)
            {
                audioSource.PlayOneShot(dropSound);
            }

            // 聲音播完了，把開關關掉！
            // 這樣它在地上滾動或玩家踢到它時，就不會一直發出噪音
            isDroppedAndFalling = false;
        }
    }
}