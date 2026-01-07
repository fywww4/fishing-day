using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactLayer; // 設定 NPC 的 Layer

    void Update()
    {
        // 從螢幕中心發射射線
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            // 1. 碰到 NPC，改變準心
            DialogueManager.Instance.SetHoverState(true);

            // 2. 按左鍵對話
            if (Input.GetMouseButtonDown(0))
            {
                // 呼叫對話管理器開始
                DialogueManager.Instance.StartConversation();
            }
        }
        else
        {
            // 沒碰到，恢復準心
            DialogueManager.Instance.SetHoverState(false);
        }
    }
}