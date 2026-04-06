using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactLayer;

    void Update()
    {
        // 1. 如果密碼鎖 UI 開著，滑鼠是用來點按鈕的，直接收工
        if (KeypadController.Instance != null && KeypadController.Instance.keypadUI.activeSelf)
        {
            return;
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        bool seeInteractable = false;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {

            PlankInteract plank = hit.collider.GetComponent<PlankInteract>();
            if (plank != null)
            {
                //只有在「拿著槌子」的情況下，準心外圈才會跑出來！
                if (HammerTool.IsHoldingHammer)
                {
                    seeInteractable = true;
                    if (Input.GetMouseButtonDown(0))
                    {
                        plank.BreakPlank();
                    }
                }
            }

            // 確保打到的物件是在 NPC Layer (槌子和密碼鎖模型都要設為這個 Layer)
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("NPC"))
            {
                seeInteractable = true;

                if (Input.GetMouseButtonDown(0))
                {
                    // 優先級 1：檢查是不是槌子
                    HammerTool hammer = hit.collider.GetComponent<HammerTool>();
                    if (hammer != null)
                    {
                        Transform myHand = GameObject.Find("HandAnchor").transform;
                        hammer.PickUp(myHand);
                        // 撿起後直接結束這次點擊，不往下跑
                        DialogueManager.Instance.SetHoverState(false);
                        return;
                    }

                    // 優先級 2：檢查是不是電子鎖 (模型上的 KeypadController)
                    KeypadController targetKeypad = hit.collider.GetComponent<KeypadController>();
                    if (targetKeypad != null)
                    {
                        targetKeypad.Interact();
                        return;
                    }

                    // 優先級 3：NPC 對話
                    NPCData npc = hit.collider.GetComponent<NPCData>();
                    if (npc != null)
                    {
                        DialogueManager.Instance.StartConversation(npc.dialogueID);
                        return;
                    }
                }
            }
        }

        DialogueManager.Instance.SetHoverState(seeInteractable);
    }
}