using Unity.Netcode;
using UnityEngine;

public class GameStateManager : NetworkBehaviour
{
    public enum GameState { Lobby, Loading, Playing, Meeting, Voting, GameOver }
    
    [SerializeField] private float gameplayDuration = 600f; // 10 minutes default
    [SerializeField] private int crewmateWinTaskThreshold = 5;
    
    private NetworkVariable<GameState> currentState = new(GameState.Lobby, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<float> gameTimer = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> crewmatesWon = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    private static GameStateManager instance;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
    }
    
    private void Start()
    {
        if (!IsServer)
            return;
        
        currentState.OnValueChanged += OnGameStateChanged;
    }
    
    private void Update()
    {
        if (!IsServer || currentState.Value != GameState.Playing)
            return;
        
        // Update game timer
        gameTimer.Value += Time.deltaTime;
        
        // Check win conditions
        CheckWinConditions();
    }
    
    /// <summary>
    /// Transition to a new game state
    /// </summary>
    public void SetGameState(GameState newState)
    {
        if (!IsServer)
        {
            Debug.LogWarning("[GameStateManager] Only server can change game state");
            return;
        }
        
        if (currentState.Value == newState)
            return;
        
        currentState.Value = newState;
        Debug.Log($"[GameStateManager] State changed to: {newState}");
    }
    
    private void OnGameStateChanged(GameState oldState, GameState newState)
    {
        Debug.Log($"[GameStateManager] Game state: {oldState} → {newState}");
        
        switch (newState)
        {
            case GameState.Playing:
                OnGameplayStarted();
                break;
            case GameState.Meeting:
                OnMeetingStarted();
                break;
            case GameState.GameOver:
                OnGameOver();
                break;
        }
    }
    
    private void OnGameplayStarted()
    {
        gameTimer.Value = 0f;
        Debug.Log("[GameStateManager] Gameplay started");
    }
    
    private void OnMeetingStarted()
    {
        // Pause game timer during meeting
        Debug.Log("[GameStateManager] Emergency meeting started");
    }
    
    private void OnGameOver()
    {
        Debug.Log($"[GameStateManager] Game Over - Crewmates Won: {crewmatesWon.Value}");
    }
    
    private void CheckWinConditions()
    {
        // TODO: Implement win condition logic
        // - All crewmates completed tasks → crewmates win
        // - All impostors eliminated → crewmates win
        // - Impostors equal or outnumber crewmates → impostors win
        // - Time expired → tie or impostor win
    }
    
    public GameState GetCurrentState() => currentState.Value;
    public float GetGameTimer() => gameTimer.Value;
    public bool DidCrewmatesWin() => crewmatesWon.Value;
    
    public static GameStateManager Instance => instance;
}
