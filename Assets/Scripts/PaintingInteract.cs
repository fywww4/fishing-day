using UnityEngine;

public class PaintingInteract : MonoBehaviour
{
    [Header("地板上的觸發點物件")]
    public ChairPlaceTarget placementTrigger;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    public void InteractPainting()
    {
        // 只有當觸發點的 isPlaced 為 true，才准掉落
        if (placementTrigger != null && placementTrigger.isPlaced)
        {
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(transform.forward * 2f, ForceMode.Impulse);
            }
            this.enabled = false;
        }
        else
        {
            // 沒放椅子，跳出劇情對話 (ID 48)
            DialogueManager.Instance.StartConversation(48);
        }
    }
}