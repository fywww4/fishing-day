using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5.0f;

    [Header("視角設定")]
    public float mouseSensitivity = 100f; // 預設值
    public Transform playerCamera;

    [Header("物理組件")]
    public Rigidbody rb;

    [Header("腳步聲設定")]
    public AudioSource footstepSource;
    public AudioClip[] footstepSounds;
    public float stepInterval = 0.5f;
    public float stepVolume = 0.5f;

    private float xRotation = 0f;
    private float stepTimer = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // 關鍵修改：讀取靈敏度
        float savedSens = PlayerPrefs.GetFloat("Sensitivity", 100f);

        // 防呆機制：如果讀出來太小 (例如 0)，強制設為 100
        // 這樣就能保證視角絕對不會卡死
        if (savedSens <= 1f)
        {
            savedSens = 100f;
            PlayerPrefs.SetFloat("Sensitivity", 100f); // 順便修復存檔
        }

        mouseSensitivity = savedSens;
    }

    void Update()
    {
        // 使用讀取到的 mouseSensitivity
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 移動邏輯
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        Vector3 finalVelocity = move * moveSpeed;
        rb.velocity = new Vector3(finalVelocity.x, rb.velocity.y, finalVelocity.z);

        HandleFootsteps(x, z);
    }

    void HandleFootsteps(float xInput, float zInput)
    {
        if ((Mathf.Abs(xInput) > 0.1f || Mathf.Abs(zInput) > 0.1f) && footstepSounds.Length > 0)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                PlayRandomFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0;
        }
    }

    void PlayRandomFootstep()
    {
        if (footstepSource == null) return;
        int index = Random.Range(0, footstepSounds.Length);
        footstepSource.pitch = Random.Range(0.8f, 1.1f);
        footstepSource.PlayOneShot(footstepSounds[index], stepVolume);
    }
}