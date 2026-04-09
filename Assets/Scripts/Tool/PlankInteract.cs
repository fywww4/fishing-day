using UnityEngine;
using System.Collections;

public class PlankInteract : MonoBehaviour
{
    [Header("音效設定")]
    public AudioSource audioSource;
    public AudioClip breakSound;

    [Header("音高校準 (讓聲音更多變)")]
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    private bool isBroken = false;

    // 當玩家點擊木板時呼叫
    public void BreakPlank()
    {
        if (isBroken) return;
        StartCoroutine(BreakRoutine());
    }

    IEnumerator BreakRoutine()
    {
        isBroken = true;

        // 1. 讓木板「物理消失」但物件還在
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh != null) mesh.enabled = false;

        // 2. 處理音效
        if (audioSource != null && breakSound != null)
        {
            // 小技巧：隨機改變音高，聽起來會更自然
            audioSource.pitch = Random.Range(minPitch, maxPitch);

            audioSource.PlayOneShot(breakSound);

            // 等待聲音播放完成
            yield return new WaitForSeconds(breakSound.length);
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
        }

        // 3. 徹底刪除物件
        Destroy(gameObject);
    }
}