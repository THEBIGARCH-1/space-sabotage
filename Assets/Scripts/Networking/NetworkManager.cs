using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxPlayers = 10;
    
    private static NetworkManager instance;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        // Setup network event listeners
        Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        Unity.Netcode.NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        
        Debug.Log("[NetworkManager] Initialized");
    }
    
    private void OnDestroy()
    {
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            Unity.Netcode.NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
    
    /// <summary>
    /// Host starts the server and connects as first client
    /// </summary>
    public void StartAsHost()
    {
        if (Unity.Netcode.NetworkManager.Singleton.StartHost())
        {
            Debug.Log("[NetworkManager] Started as Host");
        }
        else
        {
            Debug.LogError("[NetworkManager] Failed to start as Host");
        }
    }
    
    /// <summary>
    /// Client connects to server
    /// </summary>
    public void ConnectAsClient(string ipAddress, ushort port = 7777)
    {
        Unity.Netcode.NetworkManager.Singleton.GetComponent<Unity.Transport.Utp.UnityTransport>()
            .SetConnectionData(ipAddress, port);
        
        if (Unity.Netcode.NetworkManager.Singleton.StartClient())
        {
            Debug.Log($"[NetworkManager] Connecting as Client to {ipAddress}:{port}");
        }
        else
        {
            Debug.LogError("[NetworkManager] Failed to connect as Client");
        }
    }
    
    /// <summary>
    /// Called when a client connects to the network
    /// </summary>
    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"[NetworkManager] Client connected: {clientId}");
        
        // If this is the server, spawn the player for this client
        if (Unity.Netcode.NetworkManager.Singleton.IsServer)
        {
            SpawnPlayerForClient(clientId);
        }
    }
    
    /// <summary>
    /// Called when a client disconnects
    /// </summary>
    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"[NetworkManager] Client disconnected: {clientId}");
        
        // Clean up player if exists
        if (Unity.Netcode.NetworkManager.Singleton.IsServer)
        {
            // Find and despawn player for this client
            foreach (var networkObject in FindObjectsOfType<NetworkObject>())
            {
                if (networkObject.OwnerClientId == clientId && 
                    networkObject.TryGetComponent<PlayerNetworkSync>(out _))
                {
                    networkObject.Despawn();
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Spawn a player for a specific client (server-side only)
    /// </summary>
    private void SpawnPlayerForClient(ulong clientId)
    {
        if (!Unity.Netcode.NetworkManager.Singleton.IsServer)
            return;
        
        Vector3 randomSpawn = spawnPoint.position + Random.insideUnitSphere * 5f;
        randomSpawn.y = spawnPoint.position.y;
        
        GameObject playerInstance = Instantiate(playerPrefab, randomSpawn, Quaternion.identity);
        
        if (playerInstance.TryGetComponent<NetworkObject>(out var networkObject))
        {
            networkObject.SpawnAsPlayerObject(clientId);
            Debug.Log($"[NetworkManager] Spawned player for client {clientId}");
        }
        else
        {
            Debug.LogError("[NetworkManager] Player prefab missing NetworkObject component");
            Destroy(playerInstance);
        }
    }
    
    /// <summary>
    /// Load scene for all clients (server-only)
    /// </summary>
    public void LoadGameScene(string sceneName)
    {
        if (!Unity.Netcode.NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("[NetworkManager] Only server can load scenes");
            return;
        }
        
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        Debug.Log($"[NetworkManager] Loading scene: {sceneName}");
    }
    
    /// <summary>
    /// Get current player count
    /// </summary>
    public int GetPlayerCount()
    {
        return Unity.Netcode.NetworkManager.Singleton.ConnectedClientsIds.Count;
    }
    
    /// <summary>
    /// Check if max players reached
    /// </summary>
    public bool IsLobbyFull()
    {
        return GetPlayerCount() >= maxPlayers;
    }
}
