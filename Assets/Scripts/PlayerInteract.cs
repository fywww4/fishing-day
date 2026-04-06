using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactLayer; // 設定 NPC 的 Layer

    void Update()
    {
        if (KeypadController.Instance != null && KeypadController.Instance.keypadUI.activeSelf)
        {
            return;
        }

        KeypadController keypad = FindObjectOfType<KeypadController>();
        if (keypad != null && keypad.keypadUI.activeSelf) 
        {
            // 既然 UI 開著，我們就強行關閉準心並結束偵測
            DialogueManager.Instance.SetHoverState(false);
            return; 
        }
        // 1. 準備一條射線，從螢幕正中心射出去
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // 預設我們「沒看到」可互動的物件
        bool seeInteractable = false;

        // 2. 發射射線！
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // 如果打到的物件 Layer 是 NPC (密碼鎖現在也用這個 Layer)
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("NPC"))
            {
                seeInteractable = true; // 標記為看到了，讓準心變化

                // 如果按左鍵
                if (Input.GetMouseButtonDown(0))
                {
                    // ? 檢查一：它是 NPC 嗎？
                    NPCData data = hit.collider.GetComponent<NPCData>();
                    if (data != null)
                    {
                        DialogueManager.Instance.StartConversation(data.dialogueID);
                    }
                    else
                    {
                        // ? 檢查二：它是密碼鎖嗎？ (新增這段)
                        KeypadController Keypad = hit.collider.GetComponent<KeypadController>();
                        if (keypad != null)
                        {
                            // 如果是密碼鎖，呼叫密碼鎖的 Interact 功能 (觸發 ID 40)
                            keypad.Interact();
                        }
                        else
                        {
                            // 什麼都沒掛的預設值 (老人)
                            DialogueManager.Instance.StartConversation(0);
                        }
                    }
                }
            }
        }
        
        // 更新準心狀態 (看有沒有碰到可互動的物件)
        DialogueManager.Instance.SetHoverState(seeInteractable);
    }
}