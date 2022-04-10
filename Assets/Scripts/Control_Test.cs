using Unity.Netcode;
using UnityEngine;

public class Control_Test : NetworkBehaviour
{
    public enum PlayerState
    {
        Movement,
        isGrounded,
    }

    [SerializeField]
    private float speed = 0.01f;

    [SerializeField]
    private float rotationSpeed = 1.5f;

    [SerializeField]
    private Vector2 defautInitialPlanePosition = new Vector2(-4, 4);

    [SerializeField]
    private NetworkVariable<Vector3> networkPositionDirection = new NetworkVariable<Vector3>();

    [SerializeField]
    private NetworkVariable<Vector3> networkRotationDirection = new NetworkVariable<Vector3>();

    [SerializeField]
    private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();

    private CharacterController characterController;

    private Animator animator;

    // client caches position
    private Vector3 prevInputPosition;
    private Vector3 prevInputRotation;
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
        }
    }

    private void Update()
    {
        if (IsClient && IsOwner)
        {
            ClientInput();
        }

        ClientMoveAndRotate();
        ClientVisuals();
    }

    private void ClientInput()
    {
        Vector3 inputRotation = new Vector3(0, Input.GetAxis("Horizontal"), 0);

        Vector3 direction = transform.TransformDirection(Vector3.forward);
        float forwardInput = Input.GetAxis("Vertical");
        Vector3 inputPosition = direction * forwardInput;

        if (prevInputPosition != inputPosition || prevInputRotation != inputRotation)
        {
            prevInputPosition = inputPosition;
            prevInputRotation = inputRotation;
            UpdateClientPositionAndRotationServerRpc(inputPosition * speed, inputRotation * rotationSpeed);
        }

        // player state changes based on input
        if (forwardInput > 0)
        {
            UpdatePlayerStateServerRpc(PlayerState.Movement);
        }
    }

    private void ClientMoveAndRotate()
    {
        if (networkPositionDirection.Value != Vector3.zero)
        {
            characterController.SimpleMove(networkPositionDirection.Value);
        }

        if (networkRotationDirection.Value != Vector3.zero)
        {
            transform.Rotate(networkRotationDirection.Value);
        }
    }

    private void ClientVisuals()
    {
        if (networkPlayerState.Value == PlayerState.Movement)
        {
            animator.SetFloat("Velocity", 0.5f);
        }
        else
        {
            animator.SetFloat("Velocity", 0f);
        }
    }

    [ServerRpc]
    public void UpdateClientPositionAndRotationServerRpc(Vector3 newPositionDirection, Vector3 newRotationDirection)
    {
        networkPositionDirection.Value = newPositionDirection;
        networkRotationDirection.Value = newRotationDirection;
    }

    [ServerRpc]
    public void UpdatePlayerStateServerRpc(PlayerState newState)
    {
        networkPlayerState.Value = newState;
    }
}
