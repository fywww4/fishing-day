using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactLayer;

    // 用來記錄目前手上拿著的實體物件
    private GameObject currentHoldingObject = null;

    void Update()
    {
        //  關鍵修復 1：把 Q 鍵丟東西放在最上面！這樣絕對不會被其他的 return 吃掉按鍵訊號
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryDropCurrentItem();
        }

        // 密碼鎖 UI 打開時，中斷射線互動 (但不會中斷上面的丟東西)
        if (KeypadController.Instance != null && KeypadController.Instance.keypadUI.activeSelf) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        bool seeInteractable = false;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // --- A. 偵測互動 (木板、鎖頭、椅子目標、畫作、大門) ---
            PlankInteract plank = hit.collider.GetComponent<PlankInteract>();
            if (plank != null && HammerTool.IsHoldingHammer)
            {
                seeInteractable = true;
                if (Input.GetMouseButtonDown(0)) plank.BreakPlank();
            }

            LockInteract doorLock = hit.collider.GetComponent<LockInteract>();
            if (doorLock != null && KeyTool.IsHoldingKey)
            {
                seeInteractable = true;
                if (Input.GetMouseButtonDown(0)) doorLock.Unlock();
            }

            ChairPlaceTarget chairTarget = hit.collider.GetComponent<ChairPlaceTarget>();
            if (chairTarget != null && ChairTool.IsHoldingChair)
            {
                seeInteractable = true;
                if (Input.GetMouseButtonDown(0)) chairTarget.PlaceChair();
            }

            PaintingInteract painting = hit.collider.GetComponent<PaintingInteract>();
            if (painting != null)
            {
                seeInteractable = true;
                if (Input.GetMouseButtonDown(0)) painting.InteractPainting();
            }

            FinalDoor finalDoor = hit.collider.GetComponent<FinalDoor>();
            if (finalDoor != null)
            {
                seeInteractable = true;
                if (Input.GetMouseButtonDown(0)) finalDoor.TryOpenDoor();
            }

            // --- B. 偵測 NPC Layer (撿取道具、密碼鎖、對話) ---
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("NPC"))
            {
                seeInteractable = true;

                if (Input.GetMouseButtonDown(0))
                {
                    GameObject handObject = GameObject.Find("HandAnchor");
                    Transform hand = (handObject != null) ? handObject.transform : null;

                    // 處理槌子
                    HammerTool hammer = hit.collider.GetComponent<HammerTool>();
                    if (hammer != null)
                    {
                        HandleReplacement(hammer.gameObject); // 檢查替換
                        hammer.PickUp(hand);
                        currentHoldingObject = hammer.gameObject;
                        return;
                    }

                    // 處理鑰匙
                    KeyTool key = hit.collider.GetComponent<KeyTool>();
                    if (key != null)
                    {
                        HandleReplacement(key.gameObject); // 檢查替換
                        key.PickUp(hand);
                        currentHoldingObject = key.gameObject;
                        return;
                    }

                    // 處理椅子
                    ChairTool chair = hit.collider.GetComponent<ChairTool>();
                    if (chair != null)
                    {
                        HandleReplacement(chair.gameObject); // 檢查替換
                        chair.PickUp(hand);
                        currentHoldingObject = chair.gameObject;
                        return;
                    }

                    // 其他不屬於「道具」的互動 (密碼鎖、NPC)
                    KeypadController targetKeypad = hit.collider.GetComponent<KeypadController>();
                    if (targetKeypad != null) { targetKeypad.Interact(); return; }

                    NPCData npc = hit.collider.GetComponent<NPCData>();
                    if (npc != null) { DialogueManager.Instance.StartConversation(npc.dialogueID); return; }
                }
            }
        }

        DialogueManager.Instance.SetHoverState(seeInteractable);
    }

    //  關鍵修復 2：補上獨立的「主動丟棄」方法
    private void TryDropCurrentItem()
    {
        // 確定手上真的有拿東西
        if (currentHoldingObject != null)
        {
            // 找出舊道具身上是哪種 Tool 並呼叫它的 Drop 方法
            HammerTool hammer = currentHoldingObject.GetComponent<HammerTool>();
            if (hammer != null) hammer.Drop();

            KeyTool key = currentHoldingObject.GetComponent<KeyTool>();
            if (key != null) key.Drop();

            ChairTool chair = currentHoldingObject.GetComponent<ChairTool>();
            if (chair != null) chair.Drop();

            // 東西丟出去了，手上的紀錄要清空！
            currentHoldingObject = null;
        }
    }

    // 核心功能：檢查並替換道具 (撿新丟舊)
    private void HandleReplacement(GameObject newItem)
    {
        // 如果手上已經有拿東西，而且不是現在要撿的這一個
        if (currentHoldingObject != null && currentHoldingObject != newItem)
        {
            // 找出舊道具身上是哪種 Tool 並呼叫它的丟掉方法
            HammerTool oldHammer = currentHoldingObject.GetComponent<HammerTool>();
            if (oldHammer != null) oldHammer.Drop();

            KeyTool oldKey = currentHoldingObject.GetComponent<KeyTool>();
            if (oldKey != null) oldKey.Drop();

            ChairTool oldChair = currentHoldingObject.GetComponent<ChairTool>();
            if (oldChair != null) oldChair.Drop();

            currentHoldingObject = null;
        }
    }
}