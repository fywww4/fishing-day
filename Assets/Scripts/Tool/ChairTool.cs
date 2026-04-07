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
        isHeld = false;
        IsHoldingChair = false;
        transform.SetParent(null);

        rb.isKinematic = false;
        if (col != null) col.isTrigger = false;

        rb.AddForce(handAnchor.forward * throwForce, ForceMode.Impulse);
    }
}