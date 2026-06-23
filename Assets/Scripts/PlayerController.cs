using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpForce = 3f;

    [Header("Look Settings")]
    // camera sensitivity 
    public float roationSpeed = 0.1f;
    // limits how far camera can look down
    public float minLookAngle = -80f;
    // limits how far camera can loo up
    public float maxLookAngle = 80f;

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform;
    // store camera's currently up/down rotation
    private float cameraPitch;

    [Header("Gravity Settings")]
    public float gravity = -9f;
    public float groundedGravity = -2f;
    // store player's curent up/down speed
    private float verticalVelocity;

    [Header("Input Actions")]
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction jumpAction;
    [SerializeField] private InputAction lookAction;
    [SerializeField] private InputAction runAction;
    private PlayerInput playerInput;
    private CharacterController characterController;

    private bool jumpPressed;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        moveAction = playerInput.actions.FindAction("Move");
        jumpAction = playerInput.actions.FindAction("Jump");
        lookAction = playerInput.actions.FindAction("Look");
        runAction = playerInput.actions.FindAction("Run");
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        // locks mouse to center of game window
        Cursor.lockState = CursorLockMode.Locked;
        // hide the mouse cursor while playing
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        RotatePlayer();
    }

    private void OnDisable()
    {   
        if (jumpAction != null)
        {
            // stop listening for jump input
            jumpAction.performed -= Jump;
            // disable jump input
            jumpAction.Disable();
        }
        // disable movement input
        if (moveAction != null)
        {
            moveAction.Disable();
        }
        // disable look input
        if (lookAction != null)
        {
            lookAction.Disable();
        }
    }

    private void OnEnable()
    {
        if (jumpAction != null)
        {
            jumpAction.Enable();
            jumpAction.performed += Jump;
        }
        // enable movement input
        if (moveAction != null)
        {
            moveAction.Enable();
        }
        // enable look input
        if (lookAction != null)
        {
            lookAction.Enable();
        }
    }
    private void MovePlayer()
    {
        // read movement input from the input system
        Vector2 input = moveAction.ReadValue<Vector2>();
        // check if player is pressing movement keys
        bool isMoving = input.sqrMagnitude > 0.01f;
        
        // convert 2D to 3D movement
        Vector3 horizontalMove = new Vector3(input.x, 0f, input.y);

        // check if the run button is being held down
        bool isRunning = runAction.ReadValue<float>() > 0f;

        // use run spped if running, else use walk speed
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // normalize prevents diagonal movement from being faster
        horizontalMove = horizontalMove.normalized * currentSpeed;

        // if player is grounded
        if (characterController.isGrounded)
        {
            verticalVelocity = groundedGravity;
            // if (isMoving && verticalVelocity < 0f)
            // {
            //     verticalVelocity = groundedGravity;
            // }
            // else if (!isMoving)
            // {
            //     verticalVelocity = 0f;
            // }
        }
        else
        {
            // if player is in the air, apply gravity
            verticalVelocity += gravity * Time.deltaTime;
        }

        // jump
        if (jumpPressed) // TODO: jump broken when standing still whelps
        {
            if (characterController.isGrounded)
            {
                verticalVelocity = jumpForce;
            }
            jumpPressed = false;
        }

        // combine horizontal and vertial movement
        Vector3 finalMove = horizontalMove;
        finalMove.y = verticalVelocity;

        // move relative to player facing direction
        Vector3 worldMove = transform.TransformDirection(finalMove);

        // move CharacterController
        characterController.Move(worldMove * Time.deltaTime);
    }

    private void RotatePlayer()
    {
        // read mouse input
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        // rotate player's body left and right
        transform.Rotate(Vector3.up * lookInput.x * roationSpeed);

        // rotate camera up and down
        cameraPitch -= lookInput.y * roationSpeed;

        // clamp prevents player from looking too far up or down
        cameraPitch = Mathf.Clamp(cameraPitch, minLookAngle, maxLookAngle);

        // apply the up/down roation to the camera only
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        jumpPressed = true;
    }

}
