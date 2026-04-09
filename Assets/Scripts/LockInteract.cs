using UnityEngine;
using System.Collections; // 協程需要這個

public class LockInteract : MonoBehaviour
{
    [Header("音效設定")]
    public AudioSource audioSource;
    public AudioClip unlockSound;

    private bool isUnlocking = false; // 防止玩家連續狂點

    // 這是原本被 PlayerInteract 呼叫的方法
    public void Unlock()
    {
        if (isUnlocking) return;
        StartCoroutine(UnlockRoutine());
    }

    IEnumerator UnlockRoutine()
    {
        isUnlocking = true;

        // 1. 關閉碰撞體，讓射線點不到，準心也不會再亮起
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 2. 隱藏模型外觀 (如果是單一物件)
        // 如果你的鎖頭模型是在子物件裡，你可能需要用 gameObject.transform.GetChild(0).gameObject.SetActive(false);
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh != null) mesh.enabled = false;

        // 3. 播放開鎖音效
        if (audioSource != null && unlockSound != null)
        {
            audioSource.PlayOneShot(unlockSound);

            // 等待這段音效的長度播完
            yield return new WaitForSeconds(unlockSound.length);
        }
        else
        {
            // 防呆機制：如果忘記掛音效，還是稍微等一下
            yield return new WaitForSeconds(0.5f);
        }

        // 4. 聲音播完了，真正刪除鎖頭
        Destroy(gameObject);

        // 如果解鎖後還要觸發開門之類的邏輯，可以寫在這裡
        // 例如： door.Open();
    }
}