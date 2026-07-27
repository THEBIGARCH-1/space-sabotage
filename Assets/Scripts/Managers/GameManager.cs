using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int minPlayersToStart = 2;
    [SerializeField] private int impostorCount = 2;
    [SerializeField] private Transform[] crewmateSpawnPoints;
    [SerializeField] private Transform[] impostorSpawnPoints;
    
    private static GameManager instance;
    private List<PlayerController> allPlayers = new();
    private bool gameStarted = false;
    
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
        if (!NetworkManager.Singleton.IsServer)
        {
            enabled = false;
            return;
        }
        
        Debug.Log("[GameManager] Initialized on server");
    }
    
    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;
        
        // Check if enough players to start game
        int playerCount = GetPlayerCount();
        
        if (!gameStarted && playerCount >= minPlayersToStart)
        {
            // Auto-start after min players (for demo purposes)
            // In real game, wait for Ready button or countdown
            gameStarted = true;
            StartGame();
        }
    }
    
    /// <summary>
    /// Start the game and assign roles
    /// </summary>
    public void StartGame()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;
        
        allPlayers.Clear();
        
        // Get all connected players
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            // Find player controller for this client
            foreach (var obj in FindObjectsOfType<NetworkObject>())
            {
                if (obj.OwnerClientId == clientId && obj.TryGetComponent<PlayerController>(out var controller))
                {
                    allPlayers.Add(controller);
                    break;
                }
            }
        }
        
        Debug.Log($"[GameManager] Starting game with {allPlayers.Count} players");
        
        // Assign roles
        AssignRoles();
        
        // Transition to playing state
        GameStateManager.Instance.SetGameState(GameStateManager.GameState.Playing);
    }
    
    /// <summary>
    /// Randomly assign Impostor and Crewmate roles
    /// </summary>
    private void AssignRoles()
    {
        // Shuffle player list
        ShuffleList(allPlayers);
        
        for (int i = 0; i < allPlayers.Count; i++)
        {
            PlayerModel playerModel = allPlayers[i].GetComponent<PlayerModel>();
            
            if (i < impostorCount)
            {
                playerModel.SetRole(PlayerModel.PlayerRole.Impostor);
                Debug.Log($"[GameManager] Player {i} assigned as Impostor");
            }
            else
            {
                playerModel.SetRole(PlayerModel.PlayerRole.Crewmate);
                Debug.Log($"[GameManager] Player {i} assigned as Crewmate");
            }
        }
    }
    
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            
            // Swap
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    
    /// <summary>
    /// Get total player count
    /// </summary>
    public int GetPlayerCount()
    {
        return NetworkManager.Singleton.ConnectedClientsIds.Count;
    }
    
    public static GameManager Instance => instance;
}
