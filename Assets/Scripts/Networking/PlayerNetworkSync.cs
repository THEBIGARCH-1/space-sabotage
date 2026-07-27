using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkSync : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private float groundAcceleration = 10f;
    [SerializeField] private LayerMask groundLayer;
    
    private Vector3 velocity = Vector3.zero;
    private Rigidbody rb;
    private bool isGrounded;
    private float groundCheckDistance = 0.1f;
    
    // Networked movement state
    private NetworkVariable<Vector3> networkPosition = new(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Quaternion> networkRotation = new(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector3> networkVelocity = new(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    private PlayerController playerController;
    private float networkUpdateTimer = 0f;
    private const float NetworkUpdateRate = 0.1f; // 10 updates per second (adjustable)
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
        
        if (rb == null)
            Debug.LogError("[PlayerNetworkSync] Rigidbody not found on player");
    }
    
    private void Start()
    {
        // Subscribe to network variable changes
        networkPosition.OnValueChanged += OnNetworkPositionChanged;
        networkRotation.OnValueChanged += OnNetworkRotationChanged;
        networkVelocity.OnValueChanged += OnNetworkVelocityChanged;
    }
    
    private void OnDestroy()
    {
        networkPosition.OnValueChanged -= OnNetworkPositionChanged;
        networkRotation.OnValueChanged -= OnNetworkRotationChanged;
        networkVelocity.OnValueChanged -= OnNetworkVelocityChanged;
    }
    
    private void Update()
    {
        if (!IsOwner)
            return;
        
        // Handle input and movement for local player
        HandleMovementInput();
        CheckGroundContact();
        
        // Sync position/rotation to network periodically
        networkUpdateTimer += Time.deltaTime;
        if (networkUpdateTimer >= NetworkUpdateRate)
        {
            networkUpdateTimer = 0f;
            networkPosition.Value = transform.position;
            networkRotation.Value = transform.rotation;
            networkVelocity.Value = velocity;
        }
    }
    
    private void FixedUpdate()
    {
        if (!IsOwner)
            return;
        
        ApplyMovement();
    }
    
    private void HandleMovementInput()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        
        // Get forward and right relative to camera/player orientation
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        
        // Calculate desired movement direction
        Vector3 moveDirection = (forward * moveZ + right * moveX).normalized;
        
        // Apply horizontal movement with acceleration
        Vector3 targetVelocity = moveDirection * moveSpeed;
        targetVelocity.y = velocity.y; // Preserve vertical velocity
        
        // Smooth acceleration on ground
        if (isGrounded)
        {
            velocity = Vector3.Lerp(velocity, targetVelocity, groundAcceleration * Time.deltaTime);
        }
        else
        {
            // Air movement (less responsive)
            velocity.x = Mathf.Lerp(velocity.x, targetVelocity.x, groundAcceleration * 0.5f * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, targetVelocity.z, groundAcceleration * 0.5f * Time.deltaTime);
        }
        
        // Handle jumping
        if (jumpPressed && isGrounded)
        {
            velocity.y = jumpForce;
            isGrounded = false;
        }
    }
    
    private void ApplyMovement()
    {
        // Apply gravity
        velocity.y -= 9.81f * Time.fixedDeltaTime;
        
        // Apply drag on ground
        if (isGrounded)
        {
            velocity = new Vector3(
                velocity.x * (1f - groundDrag * Time.fixedDeltaTime),
                velocity.y,
                velocity.z * (1f - groundDrag * Time.fixedDeltaTime)
            );
        }
        
        // Update rigidbody velocity
        rb.velocity = velocity;
    }
    
    private void CheckGroundContact()
    {
        // Raycast downward to check if grounded
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );
    }
    
    // Network variable callbacks for remote players
    private void OnNetworkPositionChanged(Vector3 oldPosition, Vector3 newPosition)
    {
        if (IsOwner)
            return;
        
        // Smoothly interpolate remote player position
        StartCoroutine(SmoothPositionTransition(oldPosition, newPosition));
    }
    
    private void OnNetworkRotationChanged(Quaternion oldRotation, Quaternion newRotation)
    {
        if (IsOwner)
            return;
        
        // Smoothly interpolate remote player rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, 0.1f);
    }
    
    private void OnNetworkVelocityChanged(Vector3 oldVelocity, Vector3 newVelocity)
    {
        if (IsOwner)
            return;
        
        // For remote players, use network velocity for animations/predictions
    }
    
    private System.Collections.IEnumerator SmoothPositionTransition(Vector3 from, Vector3 to)
    {
        float elapsedTime = 0f;
        float transitionTime = NetworkUpdateRate * 0.8f; // Slightly faster than network update rate
        
        while (elapsedTime < transitionTime)
        {
            if (IsOwner) yield break; // Stop if we became the owner
            
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionTime;
            transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }
        
        transform.position = to;
    }
    
    /// <summary>
    /// Set player position (server-side authority)
    /// </summary>
    [Rpc(SendTo.Everyone)]
    public void SetPositionRpc(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}
