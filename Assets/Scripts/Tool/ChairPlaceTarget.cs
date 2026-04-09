using UnityEngine;

public class ChairPlaceTarget : MonoBehaviour
{
    public bool isPlaced = false; // 紀錄椅子是不是已經放上去了

    [Header("視覺提示")]
    public GameObject highlightVisual; // 拖入「虛線 Quad 物件」

    [Header("實體椅子設定")]
    public GameObject realChairModel;  // 新增：拖入「要出現在地上的實體椅子模型」

    void Update()
    {
        // 1. 如果椅子已經放上去了，就永遠隱藏提示框，然後不要再執行下面的邏輯
        if (isPlaced)
        {
            if (highlightVisual != null && highlightVisual.activeSelf)
            {
                highlightVisual.SetActive(false);
            }
            return;
        }

        // 2. 如果還沒放上去，就根據「玩家有沒有拿著椅子」來決定要不要顯示
        if (highlightVisual != null)
        {
            if (ChairTool.IsHoldingChair)
            {
                highlightVisual.SetActive(true);  // 拿著椅子：顯示虛線
            }
            else
            {
                highlightVisual.SetActive(false); // 沒拿椅子：隱藏虛線
            }
        }
    }

    // 當玩家點擊這個目標點時執行 (由 PlayerInteract 呼叫)
    public void PlaceChair()
    {
        isPlaced = true;

        //  1. 讓地上的實體椅子出現
        if (realChairModel != null)
        {
            realChairModel.SetActive(true);
        }

        //  2. 消除手上的椅子 (消耗道具)
        ChairTool.IsHoldingChair = false; // 狀態改為「沒拿椅子」

        // 找出場景中玩家手上的那把椅子並銷毀它
        ChairTool heldChair = FindObjectOfType<ChairTool>();
        if (heldChair != null)
        {
            Destroy(heldChair.gameObject);
        }
    }
}