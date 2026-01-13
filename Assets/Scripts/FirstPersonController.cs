using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5.0f;

    [Header("視角設定")]
    public float mouseSensitivity = 100f;
    public Transform playerCamera;

    [Header("物理組件")]
    public Rigidbody rb;

    [Header("腳步聲設定 (新功能)")]
    public AudioSource footstepSource;    // 用來播聲音的喇叭
    public AudioClip[] footstepSounds;    // 腳步聲素材 (可以用陣列放多個，隨機播比較自然)
    public float stepInterval = 0.5f;     // 多久播一次 (跑步就調小，走路調大)
    public float stepVolume = 0.5f;       // 音量大小

    private float xRotation = 0f;
    private float stepTimer = 0f;         // 內部計時器

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // --- 1. 視角旋轉 (保持原本邏輯) ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // --- 2. 移動邏輯 ---
        float x = Input.GetAxis("Horizontal"); // A, D
        float z = Input.GetAxis("Vertical");   // W, S

        Vector3 move = transform.right * x + transform.forward * z;
        Vector3 finalVelocity = move * moveSpeed;
        rb.velocity = new Vector3(finalVelocity.x, rb.velocity.y, finalVelocity.z);

        // --- 3. 腳步聲處理 (新功能) ---
        HandleFootsteps(x, z);
    }

    void HandleFootsteps(float xInput, float zInput)
    {
        // 如果玩家正在移動 (輸入不為 0) 且 腳步聲來源有設定
        if ((Mathf.Abs(xInput) > 0.1f || Mathf.Abs(zInput) > 0.1f) && footstepSounds.Length > 0)
        {
            stepTimer -= Time.deltaTime; // 倒數計時

            if (stepTimer <= 0)
            {
                PlayRandomFootstep();
                stepTimer = stepInterval; // 重置計時器
            }
        }
        else
        {
            // 如果停下來，重置計時器，這樣下次一動就會馬上播聲音
            stepTimer = 0;
        }
    }

    void PlayRandomFootstep()
    {
        if (footstepSource == null) return;

        // 隨機選一個聲音 (避免聽起來像機器人)
        int index = Random.Range(0, footstepSounds.Length);

        // 微調音調 (Pitch)，讓每一步聽起來都有點不一樣，更真實
        footstepSource.pitch = Random.Range(0.8f, 1.1f);

        // 播放一次
        footstepSource.PlayOneShot(footstepSounds[index], stepVolume);
    }
}