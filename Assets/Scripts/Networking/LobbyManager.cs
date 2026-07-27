using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private int maxPlayersPerLobby = 10;
    [SerializeField] private int minPlayersToStart = 2;
    [SerializeField] private float readyCountdownDuration = 30f;
    
    private NetworkVariable<int> readyPlayerCount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> isCountingDown = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<float> countdownTimeRemaining = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    private Dictionary<ulong, bool> playerReadyStates = new();
    private float countdownTimer = 0f;
    private static LobbyManager instance;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
    }
    
    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;
        
        if (isCountingDown.Value)
        {
            countdownTimer -= Time.deltaTime;
            countdownTimeRemaining.Value = Mathf.Max(0, countdownTimer);
            
            if (countdownTimer <= 0)
            {
                isCountingDown.Value = false;
                GameManager.Instance.StartGame();
            }
        }
    }
    
    /// <summary>
    /// Player marks themselves as ready
    /// </summary>
    [Rpc(SendTo.Server)]
    public void PlayerReadyRpc()
    {
        if (!IsServer)
            return;
        
        if (!playerReadyStates.ContainsKey(OwnerClientId))
        {
            playerReadyStates[OwnerClientId] = true;
        }
        
        readyPlayerCount.Value = playerReadyStates.Count;
        
        Debug.Log($"[LobbyManager] Player {OwnerClientId} ready. Ready count: {readyPlayerCount.Value}");
        
        // Start countdown if min players ready
        if (readyPlayerCount.Value >= minPlayersToStart && !isCountingDown.Value)
        {
            isCountingDown.Value = true;
            countdownTimer = readyCountdownDuration;
            countdownTimeRemaining.Value = readyCountdownDuration;
            
            Debug.Log($"[LobbyManager] Countdown started: {readyCountdownDuration}s");
        }
    }
    
    /// <summary>
    /// Player marks themselves as not ready
    /// </summary>
    [Rpc(SendTo.Server)]
    public void PlayerNotReadyRpc()
    {
        if (!IsServer)
            return;
        
        if (playerReadyStates.ContainsKey(OwnerClientId))
        {
            playerReadyStates.Remove(OwnerClientId);
        }
        
        readyPlayerCount.Value = playerReadyStates.Count;
        
        // Cancel countdown if not enough ready
        if (readyPlayerCount.Value < minPlayersToStart && isCountingDown.Value)
        {
            isCountingDown.Value = false;
            Debug.Log("[LobbyManager] Countdown cancelled - not enough ready players");
        }
        
        Debug.Log($"[LobbyManager] Player {OwnerClientId} not ready. Ready count: {readyPlayerCount.Value}");
    }
    
    public int GetReadyPlayerCount() => readyPlayerCount.Value;
    public bool IsCountingDown() => isCountingDown.Value;
    public float GetCountdownTime() => countdownTimeRemaining.Value;
    public int GetMaxPlayers() => maxPlayersPerLobby;
    public bool IsLobbyFull() => NetworkManager.Singleton.ConnectedClientsIds.Count >= maxPlayersPerLobby;
    
    public static LobbyManager Instance => instance;
}
