using UnityEngine;

public class LockInteract : MonoBehaviour
{
    public void Unlock()
    {
        // 讓鎖頭消失
        Destroy(gameObject);
    }
}