using Unity.Netcode;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private float killCooldown = 45f;
    [SerializeField] private float killRange = 1f;
    
    private float lastKillTime = -100f;
    private Camera playerCamera;
    private PlayerModel playerModel;
    
    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        playerModel = GetComponent<PlayerModel>();
    }
    
    /// <summary>
    /// Attempt to interact with nearby task or object
    /// </summary>
    public void TryInteract()
    {
        if (playerCamera == null)
            return;
        
        if (!Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactionRange))
        {
            Debug.Log("[PlayerInteraction] No interaction target");
            return;
        }
        
        // Check if hit object is a task
        if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
        {
            interactable.Interact(this);
            Debug.Log($"[PlayerInteraction] Interacted with {hit.collider.gameObject.name}");
        }
    }
    
    /// <summary>
    /// Attempt to eliminate a nearby player (Impostor only)
    /// </summary>
    public void TryKill()
    {
        if (!IsOwner)
            return;
        
        if (playerModel.GetRole() != PlayerModel.PlayerRole.Impostor)
        {
            Debug.LogWarning("[PlayerInteraction] Only Impostors can kill");
            return;
        }
        
        // Check kill cooldown
        if (Time.time - lastKillTime < killCooldown)
        {
            float remainingCooldown = killCooldown - (Time.time - lastKillTime);
            Debug.Log($"[PlayerInteraction] Kill on cooldown: {remainingCooldown:F1}s remaining");
            return;
        }
        
        // Raycast for nearby player
        if (!Physics.Raycast(GetComponentInChildren<Camera>().transform.position, 
            GetComponentInChildren<Camera>().transform.forward, 
            out RaycastHit hit, killRange))
        {
            Debug.Log("[PlayerInteraction] No player in kill range");
            return;
        }
        
        // Check if hit a player
        if (hit.collider.TryGetComponent<PlayerController>(out var targetPlayer))
        {
            AttemptKillRpc(targetPlayer.GetComponent<NetworkObject>().NetworkObjectId);
            lastKillTime = Time.time;
        }
    }
    
    /// <summary>
    /// RPC to attempt killing a player (server validates)
    /// </summary>
    [Rpc(SendTo.Server)]
    private void AttemptKillRpc(uint targetNetworkObjectId)
    {
        if (!IsServer)
            return;
        
        // Validate: Caller must be Impostor
        if (playerModel.GetRole() != PlayerModel.PlayerRole.Impostor)
        {
            Debug.LogWarning($"[PlayerInteraction] Client {OwnerClientId} attempted kill but is not Impostor");
            return;
        }
        
        // Validate: Cooldown
        if (Time.time - lastKillTime < killCooldown)
        {
            Debug.LogWarning($"[PlayerInteraction] Client {OwnerClientId} kill on cooldown");
            return;
        }
        
        // Validate: Distance
        if (Vector3.Distance(transform.position, NetworkSpawnManager.SpawnedObjects[targetNetworkObjectId].transform.position) > killRange)
        {
            Debug.LogWarning($"[PlayerInteraction] Client {OwnerClientId} target out of range");
            return;
        }
        
        // Kill successful
        var targetController = NetworkSpawnManager.SpawnedObjects[targetNetworkObjectId].GetComponent<PlayerController>();
        targetController.EliminatePlayerRpc();
        playerModel.IncrementElimination();
        
        Debug.Log($"[PlayerInteraction] Player {OwnerClientId} killed {targetNetworkObjectId}");
    }
    
    /// <summary>
    /// Report a dead body
    /// </summary>
    [Rpc(SendTo.Server)]
    public void ReportBodyRpc()
    {
        Debug.Log($"[PlayerInteraction] Body reported by player {OwnerClientId}");
        
        // TODO: Call GameManager to trigger emergency meeting
    }
}

/// <summary>
/// Interface for interactive objects (tasks, buttons, etc.)
/// </summary>
public interface IInteractable
{
    void Interact(PlayerInteraction player);
}
