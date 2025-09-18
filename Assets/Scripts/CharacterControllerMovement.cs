using UnityEngine;

public class CharacterControllerMovement : MonoBehaviour
{
    private Vector3 velocity;
    private float xRot;
    private float idleTime;

    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private CharacterController Controller;
    [SerializeField] private Animator CharacterAnimator;
    [Space]
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float crouchSpeed = 0.75f;
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float gravity = -9.81f;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // makes sure animator starts with safe defaults
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

        // apply crouch or run modifiers
        bool running = Input.GetKey(KeyCode.LeftShift);
        bool crouching = Input.GetKey(KeyCode.LeftControl);

        float moveSpeed;
        if (crouching) moveSpeed = crouchSpeed;
        else if (running) moveSpeed = runSpeed;
        else moveSpeed = walkSpeed;

        // gravity
        if (Controller.isGrounded && velocity.y < 0f)
            velocity.y = -1f;
        velocity.y += gravity * Time.deltaTime;

        // move controller
        Controller.Move((transform.right * h + transform.forward * v) * moveSpeed * Time.deltaTime);
        Controller.Move(velocity * Time.deltaTime);

        // drive animator "Speed" (0=idle, 1=walk, 2=run)
        float inputMag = Mathf.Clamp01(input.magnitude);
        float targetSpeedParam;
        if (crouching)
            targetSpeedParam = 0f; // Locomotion is overridden by crouch, so stay at 0
        else if (running)
            targetSpeedParam = Mathf.Lerp(1f, 2f, inputMag);
        else
            targetSpeedParam = Mathf.Lerp(0f, 1f, inputMag);

        float current = CharacterAnimator.GetFloat("Speed");
        float smoothed = Mathf.MoveTowards(current, targetSpeedParam, Time.deltaTime * 10f);
        CharacterAnimator.SetFloat("Speed", smoothed);

        // crouch param
        CharacterAnimator.SetBool("IsCrouching", crouching);
    }

    private void HandleCamera()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        if (Mathf.Abs(mouseX) < 0.0005f) mouseX = 0;
        if (Mathf.Abs(mouseY) < 0.0005f) mouseY = 0;

        xRot = Mathf.Clamp(xRot - mouseY * sensitivity, -90f, 90f);
        PlayerCamera.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        transform.Rotate(0f, mouseX * sensitivity, 0f, Space.Self);
    }

    private void HandleAnimations()
    {
        // idle detection
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
