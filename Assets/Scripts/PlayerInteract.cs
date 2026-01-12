using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactLayer; // 設定 NPC 的 Layer

    void Update()
    {
        // 1. 準備一條射線，從螢幕正中心射出去
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // 預設我們「沒看到」NPC
        bool seeNPC = false;

        // 2. 發射射線！
        // 注意：這裡我們暫時拿掉 interactLayer，直接打全世界，確保不會因為 Layer 設定錯誤而失敗
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // 3. 檢查打到了什麼 (檢查 Layer 是 NPC 或者 Tag 是 NPC)
            // 這裡用 Layer 名稱比較直觀
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("NPC"))
            {
                seeNPC = true; // 只要符合條件，就標記為「看到了」

                // 如果按左鍵，就開啟對話
                if (Input.GetMouseButtonDown(0))
                {
                    // 嘗試取得 NPC 身上的 ID
                    NPCData data = hit.collider.GetComponent<NPCData>();
                    if (data != null)
                    {
                        // 傳入 NPC 指定的 ID
                        DialogueManager.Instance.StartConversation(data.dialogueID);
                    }
                    else
                    {
                        // 沒掛腳本的預設值 (例如老頭)
                        DialogueManager.Instance.StartConversation(0);
                    }
                }
            }
        }
        DialogueManager.Instance.SetHoverState(seeNPC);
    }
}