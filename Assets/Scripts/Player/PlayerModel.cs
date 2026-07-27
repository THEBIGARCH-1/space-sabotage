using Unity.Netcode;
using UnityEngine;

public class PlayerModel : NetworkBehaviour
{
    public enum PlayerRole { Crewmate, Impostor, None }
    
    [SerializeField] private string playerName = "Player";
    [SerializeField] private Color playerColor = Color.white;
    
    private NetworkVariable<PlayerRole> playerRole = new(PlayerRole.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> hasCompletedFakeTasks = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    private int tasksCompleted = 0;
    private int eliminationsCount = 0;
    
    private void Start()
    {
        if (IsOwner)
        {
            playerName = $"Player_{OwnerClientId}";
        }
        
        Debug.Log($"[PlayerModel] Player {playerName} spawned with role: {playerRole.Value}");
    }
    
    /// <summary>
    /// Assign role to player (server-side only)
    /// </summary>
    public void SetRole(PlayerRole role)
    {
        if (!IsServer)
            return;
        
        playerRole.Value = role;
        Debug.Log($"[PlayerModel] {playerName} assigned role: {role}");
    }
    
    /// <summary>
    /// Get player's role
    /// </summary>
    public PlayerRole GetRole() => playerRole.Value;
    
    /// <summary>
    /// Increment completed tasks
    /// </summary>
    public void CompleteTask()
    {
        if (!IsOwner)
            return;
        
        tasksCompleted++;
        Debug.Log($"[PlayerModel] {playerName} completed task {tasksCompleted}");
    }
    
    /// <summary>
    /// Increment elimination count
    /// </summary>
    public void IncrementElimination()
    {
        if (!IsOwner)
            return;
        
        eliminationsCount++;
        Debug.Log($"[PlayerModel] {playerName} eliminations: {eliminationsCount}");
    }
    
    public string GetPlayerName() => playerName;
    public Color GetPlayerColor() => playerColor;
    public int GetTasksCompleted() => tasksCompleted;
    public int GetEliminations() => eliminationsCount;
}
