using UnityEngine;

public class PlankInteract : MonoBehaviour
{
    // 木板被敲掉的功能
    public void BreakPlank()
    {
        Destroy(gameObject); // 木板消失
    }
}