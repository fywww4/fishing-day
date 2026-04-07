using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingTrigger : MonoBehaviour
{
    private bool activated = false;

    // 當玩家「碰到」這面黑牆時觸發
    private void OnTriggerEnter(Collider other)
    {
        if (!activated && other.CompareTag("Player"))
        {
            activated = true;
            // 觸發最後的對話 (我們設 ID 為 60)
            DialogueManager.Instance.StartConversation(49);
        }
    }
}