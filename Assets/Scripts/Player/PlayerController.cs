using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 90f;
    
    private float xRotation = 0f;
    private Camera playerCamera;
    private bool isAlive = true;
    private PlayerModel playerModel;
    private PlayerInteraction playerInteraction;
    
    // Networked player state
    private NetworkVariable<bool> isPlayerAlive = new(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> isInMeeting = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        playerModel = GetComponent<PlayerModel>();
        playerInteraction = GetComponent<PlayerInteraction>();
        
        if (playerCamera == null)
            Debug.LogError("[PlayerController] Camera not found in children");
    }
    
    private void Start()
    {
        if (!IsOwner)
        {
            // Disable camera for remote players
            if (playerCamera != null)
                playerCamera.gameObject.SetActive(false);
            enabled = false;
            return;
        }
        
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Update()
    {
        if (!IsOwner || !isAlive)
            return;
        
        HandleLook();
        HandleInteraction();
        HandleDebugInput();
    }
    
    private void HandleLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Rotate player body (left/right)
        transform.Rotate(0, mouseX, 0);
        
        // Rotate camera (up/down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        
        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        }
    }
    
    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerInteraction != null)
                playerInteraction.TryInteract();
        }
        
        if (IsImpostor() && Input.GetKeyDown(KeyCode.K))
        {
            if (playerInteraction != null)
                playerInteraction.TryKill();
        }
    }
    
    private void HandleDebugInput()
    {
        // Escape to unlock cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        // Tab to relock cursor
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    /// <summary>
    /// Eliminate this player (Impostor kill)
    /// </summary>
    [Rpc(SendTo.Server)]
    public void EliminatePlayerRpc()
    {
        if (!IsServer)
            return;
        
        isPlayerAlive.Value = false;
        OnPlayerEliminated();
    }
    
    private void OnPlayerEliminated()
    {
        isAlive = false;
        Debug.Log($"[PlayerController] Player {OwnerClientId} eliminated");
        
        // Disable player controls
        GetComponent<PlayerNetworkSync>().enabled = false;
        
        // Make player ghost (transparent, can move through objects)
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false; // Or make transparent
        }
    }
    
    /// <summary>
    /// Check if this player is an Impostor
    /// </summary>
    private bool IsImpostor()
    {
        if (playerModel == null)
            return false;
        
        return playerModel.GetRole() == PlayerModel.PlayerRole.Impostor;
    }
    
    /// <summary>
    /// Enter meeting mode
    /// </summary>
    [Rpc(SendTo.Everyone)]
    public void EnterMeetingRpc()
    {
        isInMeeting.Value = true;
        
        // Disable movement
        GetComponent<PlayerNetworkSync>().enabled = false;
        
        // Show meeting UI
        Debug.Log($"[PlayerController] Player {OwnerClientId} entered meeting");
    }
    
    /// <summary>
    /// Exit meeting mode
    /// </summary>
    [Rpc(SendTo.Everyone)]
    public void ExitMeetingRpc()
    {
        isInMeeting.Value = false;
        
        // Re-enable movement
        if (IsOwner && isAlive)
            GetComponent<PlayerNetworkSync>().enabled = true;
        
        Debug.Log($"[PlayerController] Player {OwnerClientId} exited meeting");
    }
    
    public bool IsAlive() => isAlive;
    public bool IsInMeeting() => isInMeeting.Value;
}
