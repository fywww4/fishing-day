using UnityEngine;

public class StoryTrigger : MonoBehaviour
{
    [Header("要啟動的事件")]
    public DogEvent dogScript; // 把狗身上那個 DogEvent 腳本拖進來

    void OnTriggerEnter(Collider other)
    {
        // 檢查撞到的是不是玩家 (Player)
        // 為了避免路邊的石頭或樹葉觸發劇情，我們要過濾對象
        if (other.CompareTag("Player"))
        {
            // 1. 喚醒狗的腳本 (開始追!)
            if (dogScript != null)
            {
                dogScript.enabled = true;
            }

            // 2. 銷毀自己 (避免來回走一直重複觸發)
            Destroy(gameObject);
        }
    }
}