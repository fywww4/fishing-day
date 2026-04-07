using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactLayer;

    void Update()
    {
        if (KeypadController.Instance != null && KeypadController.Instance.keypadUI.activeSelf) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        bool seeInteractable = false;

        // 1. 只有射線打到東西時，才進入這個區塊
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // A1. 偵測木板
            PlankInteract plank = hit.collider.GetComponent<PlankInteract>();
            if (plank != null && HammerTool.IsHoldingHammer)
            {
                seeInteractable = true;
                if (Input.GetMouseButtonDown(0)) plank.BreakPlank();
            }

            // A2. 偵測鎖頭
            LockInteract doorLock = hit.collider.GetComponent<LockInteract>();
            if (doorLock != null && KeyTool.IsHoldingKey)
            {
                seeInteractable = true;
                if (Input.GetMouseButtonDown(0)) doorLock.Unlock();
            }

            // A3. 偵測椅子目標
            ChairPlaceTarget chairTarget = hit.collider.GetComponent<ChairPlaceTarget>();
            if (chairTarget != null && ChairTool.IsHoldingChair)
            {
                seeInteractable = true;
                if (Input.GetMouseButtonDown(0)) chairTarget.PlaceChair();
            }

            // A4. 偵測畫作
            PaintingInteract painting = hit.collider.GetComponent<PaintingInteract>();
            if (painting != null)
            {
                seeInteractable = true;
                if (Input.GetMouseButtonDown(0)) painting.InteractPainting();
            }
            
            // A5. 偵測最終大門 (搬進來這裡了！)
            FinalDoor finalDoor = hit.collider.GetComponent<FinalDoor>();
            if (finalDoor != null)
            {
                seeInteractable = true;
                if (Input.GetMouseButtonDown(0)) finalDoor.TryOpenDoor();
            }

            // B. 偵測 NPC Layer 的東西 (放在最後，且用 else if 區隔)
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("NPC"))
            {
                seeInteractable = true;

                if (Input.GetMouseButtonDown(0))
                {
                    HammerTool hammer = hit.collider.GetComponent<HammerTool>();
                    if (hammer != null) { hammer.PickUp(GameObject.Find("HandAnchor").transform); return; }

                    KeyTool key = hit.collider.GetComponent<KeyTool>();
                    if (key != null) { key.PickUp(GameObject.Find("HandAnchor").transform); return; }

                    KeypadController targetKeypad = hit.collider.GetComponent<KeypadController>();
                    if (targetKeypad != null) { targetKeypad.Interact(); return; }

                    ChairTool chair = hit.collider.GetComponent<ChairTool>();
                    if (chair != null) { chair.PickUp(GameObject.Find("HandAnchor").transform); return; }

                    NPCData npc = hit.collider.GetComponent<NPCData>();
                    if (npc != null) { DialogueManager.Instance.StartConversation(npc.dialogueID); return; }
                }
            }
        } // 這裡才是 Physics.Raycast 的結束括號

        // 更新準心
        DialogueManager.Instance.SetHoverState(seeInteractable);
    }
}