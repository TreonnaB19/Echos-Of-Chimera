using UnityEngine;

public class CharacterControllerMovement : MonoBehaviour
{
    private Vector3 velocity;
    private float xRot;
    private float idleTime;

    [Header("Refs")]
    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private CharacterController Controller;
    [SerializeField] private Animator CharacterAnimator;

    [Header("Speeds")]
    [SerializeField] private float walkSpeed   = 1.5f;
    [SerializeField] private float runSpeed    = 4f;
    [SerializeField] private float crouchSpeed = 0.75f;

    [Header("Mouse")]
    [SerializeField] private float sensitivity = 2f;

    [Header("Physics")]
    [SerializeField] private float gravity = -9.81f;   // keep negative

    [Header("Crouch (view & capsule)")]
    [SerializeField] private float standCameraY   = 1.6f;
    [SerializeField] private float crouchCameraY  = 1.0f;
    [SerializeField] private float cameraLerpSpeed = 8f;

    [SerializeField] private float standHeight   = 2.0f;   // CC height when standing
    [SerializeField] private float crouchHeight  = 1.4f;   // CC height when crouched
    [SerializeField] private float heightLerpSpeed = 12f;  // how fast capsule changes height

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // If not set in Inspector, use current camera Y as standing Y
        if (Mathf.Approximately(standCameraY, 0f))
            standCameraY = PlayerCamera.localPosition.y;

        CharacterAnimator.SetBool("IsCrouching", false);
        CharacterAnimator.SetBool("IsTrulyIdle", false);
    }

    void Update()
    {
        HandleMovement();
        HandleCamera();
        HandleAnimations();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0f, v);
        if (input.sqrMagnitude > 1f) input.Normalize();

        // modifiers
        bool running   = Input.GetKey(KeyCode.LeftShift);
        bool crouching = Input.GetKey(KeyCode.LeftControl);

        float moveSpeed = crouching ? crouchSpeed : (running ? runSpeed : walkSpeed);

        // gravity
        if (Controller.isGrounded && velocity.y < 0f)
            velocity.y = -1f;               // small stick-to-ground bias
        velocity.y += gravity * Time.deltaTime;

        // move
        Vector3 moveWorld = (transform.right * h + transform.forward * v) * moveSpeed;
        Controller.Move(moveWorld * Time.deltaTime);
        Controller.Move(velocity * Time.deltaTime);

        // animate locomotion: Speed 0..2 (0 idle, 1 walk, 2 run)
        float inputMag = Mathf.Clamp01(input.magnitude);
        float targetSpeedParam =
            crouching ? 0f :
            (running ? Mathf.Lerp(1f, 2f, inputMag)
                     : Mathf.Lerp(0f, 1f, inputMag));

        float current = CharacterAnimator.GetFloat("Speed");
        float smoothed = Mathf.MoveTowards(current, targetSpeedParam, Time.deltaTime * 10f);
        CharacterAnimator.SetFloat("Speed", smoothed);

        // crouch flag (drives AnyState→Crouching)
        CharacterAnimator.SetBool("IsCrouching", crouching);

        // >>> Smooth camera + capsule for crouch <<<
        ApplyCrouchAdjustments(crouching);
    }

    private void ApplyCrouchAdjustments(bool crouching)
    {
        // --- Camera height ---
        float targetCamY = crouching ? crouchCameraY : standCameraY;
        Vector3 camLocal = PlayerCamera.localPosition;
        camLocal.y = Mathf.MoveTowards(camLocal.y, targetCamY, cameraLerpSpeed * Time.deltaTime);
        PlayerCamera.localPosition = camLocal;

        // --- CharacterController height/center ---
        // Keep the bottom of the capsule fixed: bottom = center.y - height/2
        float bottom = Controller.center.y - Controller.height * 0.5f;

        float targetHeight = crouching ? crouchHeight : standHeight;
        float newHeight = Mathf.MoveTowards(Controller.height, targetHeight, heightLerpSpeed * Time.deltaTime);
        Controller.height = newHeight;

        // Recompute center so the bottom stays where it was
        Vector3 c = Controller.center;
        c.y = bottom + newHeight * 0.5f;
        Controller.center = c;
    }

    private void HandleCamera()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        // avoid micro drift
        if (Mathf.Abs(mouseX) < 0.0005f) mouseX = 0f;
        if (Mathf.Abs(mouseY) < 0.0005f) mouseY = 0f;

        xRot = Mathf.Clamp(xRot - mouseY * sensitivity, -90f, 90f);
        PlayerCamera.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        transform.Rotate(0f, mouseX * sensitivity, 0f, Space.Self);
    }

    private void HandleAnimations()
    {
        bool moving = (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);

        if (moving)
        {
            idleTime = 0f;
            CharacterAnimator.SetBool("IsTrulyIdle", false);
        }
        else
        {
            idleTime += Time.deltaTime;
            if (idleTime >= 20f)
                CharacterAnimator.SetBool("IsTrulyIdle", true);
        }
    }
}
