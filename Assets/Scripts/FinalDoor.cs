using UnityEngine;

public class FinalDoor : MonoBehaviour
{
    public void TryOpenDoor()
    {
        // 檢查條件 1：密碼鎖是否解開
        bool keypadDone = KeypadController.Instance != null && KeypadController.Instance.isSolved;

        // 檢查條件 2：場景中是否還有鎖頭 (LockInteract)
        bool lockDone = FindObjectOfType<LockInteract>() == null;

        // 檢查條件 3：場景中是否還有木板 (PlankInteract)
        bool plankDone = FindObjectOfType<PlankInteract>() == null;

        // 最終判定：三者都達成
        if (keypadDone && lockDone && plankDone)
        {
            Debug.Log("所有障礙已移除，門開了！");
            Destroy(gameObject); // 直接把大門刪除
        }
        else
        {
            // 如果條件沒達成，芝麻可以碎碎念提示玩家
            DialogueManager.Instance.StartConversation(41);
        }
    }
}