using Unity.Netcode;
using Unity.Netcode.Samples;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]

public class PlayerControl : NetworkBehaviour
{
    public enum PlayerState
    {
        Walking,
        Running,
        Idle,
        isGrounded,
        didJump, 
    }

    [SerializeField]
    private float speed = 0.01f;

    [SerializeField]
    private float runSpeedOffset = 2.0f;

    [SerializeField]
    private float rotationSpeed = 1.5f;

    [SerializeField]
    private Vector2 defautInitialPlanePosition = new Vector2(-4, 4);

    [SerializeField]
    private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();

    private CharacterController characterController;

    private Animator animator;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (IsClient && IsOwner)
        {
            transform.position = new Vector3(Random.Range(defautInitialPlanePosition.x, defautInitialPlanePosition.y), 0, 
                Random.Range(defautInitialPlanePosition.x, defautInitialPlanePosition.y));

            PlayerCameraFollow.Instance.FollowPlayer(transform.Find("CameraFollow"));
        }
    }

    private void Update()
    {
        if (IsClient && IsOwner)
        {
            ClientInput();
        }

        ClientVisuals();
    }

    private void ClientInput()
    {
        Vector3 inputRotation = new Vector3(0, Input.GetAxis("Horizontal"), 0);

        Vector3 direction = transform.TransformDirection(Vector3.forward);
        float forwardInput = Input.GetAxis("Vertical");
        Vector3 inputPosition = direction * forwardInput;

        // player state changes based on input
        if (forwardInput == 0)
        {
            UpdatePlayerStateServerRpc(PlayerState.Idle);
        }
        else if (forwardInput > 0 && forwardInput <= 1 && !ActiveRunningActionKey())
        {
            UpdatePlayerStateServerRpc(PlayerState.Walking);
        }
        else if (forwardInput > 0 && forwardInput <= 1 && ActiveRunningActionKey())
        {
            inputPosition = direction * runSpeedOffset;
            UpdatePlayerStateServerRpc(PlayerState.Running);
        }

        // client side movement
        characterController.SimpleMove(inputPosition * speed);
        transform.Rotate(inputRotation * rotationSpeed, Space.World);
    }

    private static bool ActiveRunningActionKey()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private void ClientVisuals()
    {
        if (networkPlayerState.Value == PlayerState.Walking)
        {
            animator.SetFloat("Velocity", 0.5f);
        }
        else if (networkPlayerState.Value == PlayerState.Running)
        {
            animator.SetFloat("Velocity", 1f);
        }
        else
        {
            animator.SetFloat("Velocity", 0f);
        }
    }

    [ServerRpc]
    public void UpdatePlayerStateServerRpc(PlayerState newState)
    {
        networkPlayerState.Value = newState;
    }
}
