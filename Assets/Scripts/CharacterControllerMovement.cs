using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerMovement : MonoBehaviour
{
    private Vector3 Velocity;
    private Vector3 PlayerMovementInput;
    private Vector2 PlayerMouseInput;
    private float xRot;
    private float idleTime;

    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private CharacterController Controller;
    [SerializeField] private Animator CharacterAnimator;
    [Space]
    [SerializeField] private float Speed;
    [SerializeField] private float RunSpeed;
    [SerializeField] private float Sensitivity;
    [SerializeField] private float Gravity = -9.81f;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MovePlayer();
        MovePlayerCamera();
        UpdateAnimations();
    }

    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput);

        if (Controller.isGrounded)
        {
            Velocity.y = -1f;
        }
        else
        {
            Velocity.y -= Gravity * -2f * Time.deltaTime;
        }

        float currentSpeed = Speed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = RunSpeed;
        }

        // Set the animator's speed parameter based on the current movement speed
        CharacterAnimator.SetFloat("Speed", MoveVector.magnitude * currentSpeed);

        Controller.Move(MoveVector * currentSpeed * Time.deltaTime);
        Controller.Move(Velocity * Time.deltaTime);
    }

    private void MovePlayerCamera()
    {
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        xRot -= PlayerMouseInput.y * Sensitivity;

        transform.Rotate(0f, PlayerMouseInput.x * Sensitivity, 0f);
        PlayerCamera.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }

    private void UpdateAnimations()
    {
        bool isMoving = (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);

        if (isMoving)
        {
            idleTime = 0f;
            CharacterAnimator.SetBool("IsTrulyIdle", false);
        }
        else
        {
            idleTime += Time.deltaTime;
            if (idleTime >= 20f)
            {
                CharacterAnimator.SetBool("IsTrulyIdle", true);
            }
        }
    }
}