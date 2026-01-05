using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5.0f;

    [Header("視角設定")]
    public float mouseSensitivity = 100f; // 滑鼠靈敏度
    public Transform playerCamera;        // 拖曳你的 Main Camera 到這裡

    [Header("物理組件")]
    public Rigidbody rb;

    private float xRotation = 0f; // 用來紀錄上下看的角度

    void Start()
    {
        // 隱藏並鎖定滑鼠游標到螢幕中央 (FPS 遊戲標準設定)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (rb == null) rb = GetComponent<Rigidbody>();

        // 確保剛體不會跌倒
        rb.freezeRotation = true;
    }

    void Update()
    {
        // --- 1. 處理視角旋轉 (Mouse Look) ---

        // 取得滑鼠移動量
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 計算上下看的角度 (注意這裡是減法，因為滑鼠往上是負旋轉)
        xRotation -= mouseY;

        // 限制抬頭低頭的角度 (像是人類頸椎極限，不能轉360度)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // A. 上下轉動：只轉動「攝影機」
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // B. 左右轉動：轉動整個「玩家身體」
        transform.Rotate(Vector3.up * mouseX);


        // --- 2. 處理移動 (Movement) ---

        // 取得鍵盤輸入
        float x = Input.GetAxis("Horizontal"); // A, D
        float z = Input.GetAxis("Vertical");   // W, S

        // 計算移動方向 (永遠相對於玩家目前的面向)
        // transform.right 是右方，transform.forward 是前方
        Vector3 move = transform.right * x + transform.forward * z;

        // 應用速度到剛體 (保留原本的 Y 軸重力速度)
        Vector3 finalVelocity = move * moveSpeed;
        rb.velocity = new Vector3(finalVelocity.x, rb.velocity.y, finalVelocity.z);
    }
}   