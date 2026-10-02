using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float walkSpeed = 5.2f;
    [SerializeField] float sprintSpeed = 8.4f;
    [SerializeField] float gravity = -13.0f;
    [SerializeField] float jumpHeight = 1.8f;
    [SerializeField] float moveSmoothTime = 0.18f;
    [SerializeField] float mouseSmoothTime = 0.03f;
    [SerializeField] bool lockCursor = false;

    float cameraPitch;
    float velocityY;
    CharacterController controller;
    ColonyClimber climber;
    Vector2 currentDir;
    Vector2 currentDirVelocity;
    Vector2 currentMouseDelta;
    Vector2 currentMouseDeltaVelocity;

    public bool InputEnabled { get; set; }
    public Transform LookTransform => playerCamera;
    public bool IsMoving { get; private set; }
    public bool IsGrounded => controller != null && controller.isGrounded;

    public void BindCamera(Transform cam)
    {
        playerCamera = cam;
    }

    public void ResetMotor(Vector3 position, Quaternion rotation)
    {
        controller.enabled = false;
        transform.SetPositionAndRotation(position, rotation);
        controller.enabled = true;
        climber?.Release();
        cameraPitch = velocityY = 0f;
        currentDir = currentDirVelocity = currentMouseDelta = currentMouseDeltaVelocity = Vector2.zero;
        playerCamera.localRotation = Quaternion.identity;
        GetComponent<ColonyAcidGun>()?.ResetWeapon();
    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        climber = GetComponent<ColonyClimber>();
    }

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (!InputEnabled || playerCamera == null)
        {
            return;
        }

        bool commanding = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        Drive(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            commanding ? Vector2.zero : new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")),
            Input.GetKey(KeyCode.LeftShift), Input.GetButtonDown("Jump"));
    }

    public void Drive(Vector2 movement, Vector2 look, bool sprint, bool jump)
    {
        if (!InputEnabled) return;
        UpdateMouseLook(look);
        UpdateMovement(movement, sprint, jump);
    }

    void UpdateMouseLook(Vector2 target)
    {
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, target, ref currentMouseDeltaVelocity, mouseSmoothTime);
        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
        playerCamera.localEulerAngles = Vector3.right * cameraPitch;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMovement(Vector2 input, bool sprint, bool jump)
    {
        input = Vector2.ClampMagnitude(input, 1f);
        currentDir = Vector2.SmoothDamp(currentDir, input, ref currentDirVelocity, moveSmoothTime);
        float speed = sprint ? sprintSpeed : walkSpeed;
        Vector3 planar = (transform.forward * currentDir.y + transform.right * currentDir.x);
        if (climber != null && climber.PlayerClimb(planar, speed, jump, ref velocityY))
        {
            IsMoving = planar.sqrMagnitude > 0.04f;
            return;
        }

        if (controller.isGrounded)
        {
            velocityY = -2f;
            if (jump)
            {
                velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            velocityY += gravity * Time.deltaTime;
        }

        Vector3 velocity = planar * speed + Vector3.up * velocityY;
        controller.Move(velocity * Time.deltaTime);
        IsMoving = currentDir.magnitude > 0.12f && controller.isGrounded;
    }
}
