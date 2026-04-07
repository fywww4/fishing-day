using UnityEngine;

public class ChairPlaceTarget : MonoBehaviour
{
    [Header("地板上的那張椅子")]
    public GameObject floorChair; // 拖入那張要徹底隱藏的椅子

    // 讓畫作腳本讀取的標記
    public bool isPlaced = false;

    public void PlaceChair()
    {
        // 1. 標記已放置
        isPlaced = true;

        // 2. 關鍵修改：直接把「整個椅子物件」喚醒！(外觀和 Collider 會一起出現)
        if (floorChair != null)
        {
            floorChair.SetActive(true);
        }

        // 3. 更新玩家狀態
        ChairTool.IsHoldingChair = false;

        // 4. 銷毀玩家手上拿著的那個椅子物件
        ChairTool heldChair = FindObjectOfType<ChairTool>();
        if (heldChair != null)
        {
            Destroy(heldChair.gameObject);
        }

        // 5. 放置成功後，把這個地板觸發點關掉，避免重複放置
        gameObject.SetActive(false);
    }
}