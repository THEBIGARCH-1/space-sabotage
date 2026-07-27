using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class TaskSystem : NetworkBehaviour
{
    [System.Serializable]
    public class TaskData
    {
        public string taskName;
        public int taskId;
        public Vector3 taskLocation;
        public float completionTime = 5f;
        public bool isCompleted;
    }
    
    [SerializeField] private TaskData[] allTasks;
    [SerializeField] private int requiredTasksToWin = 5;
    
    private NetworkVariable<int> completedTaskCount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<float> globalTaskProgress = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    private Dictionary<int, bool> taskCompletionStatus = new();
    private static TaskSystem instance;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        InitializeTasks();
    }
    
    private void InitializeTasks()
    {
        allTasks = new TaskData[]
        {
            new TaskData { taskName = "Align Engine Output", taskId = 1, taskLocation = new Vector3(30, 1, 0), completionTime = 8f },
            new TaskData { taskName = "Download Data", taskId = 2, taskLocation = new Vector3(-30, 1, 0), completionTime = 6f },
            new TaskData { taskName = "Fix Wiring", taskId = 3, taskLocation = new Vector3(0, 1, -30), completionTime = 7f },
            new TaskData { taskName = "Scan Card", taskId = 4, taskLocation = new Vector3(0, 1, 30), completionTime = 5f },
            new TaskData { taskName = "Verify Scan", taskId = 5, taskLocation = new Vector3(30, 1, 30), completionTime = 4f }
        };
        
        foreach (var task in allTasks)
        {
            taskCompletionStatus[task.taskId] = false;
        }
    }
    
    [Rpc(SendTo.Server)]
    public void CompleteTaskRpc(int taskId)
    {
        if (!IsServer) return;
        
        if (taskCompletionStatus[taskId]) return;
        
        taskCompletionStatus[taskId] = true;
        completedTaskCount.Value++;
        globalTaskProgress.Value = (float)completedTaskCount.Value / requiredTasksToWin;
        
        if (completedTaskCount.Value >= requiredTasksToWin)
        {
            GameStateManager.Instance.SetGameState(GameStateManager.GameState.GameOver);
        }
    }
    
    public TaskData[] GetAllTasks() => allTasks;
    public float GetTaskProgress() => globalTaskProgress.Value;
    public int GetCompletedTaskCount() => completedTaskCount.Value;
    public static TaskSystem Instance => instance;
}