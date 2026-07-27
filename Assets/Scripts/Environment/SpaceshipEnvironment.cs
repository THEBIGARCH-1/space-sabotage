using UnityEngine;

public class SpaceshipEnvironment : MonoBehaviour
{
    [System.Serializable]
    public class Room
    {
        public string roomName;
        public Vector3 position;
        public Vector3 size;
    }
    
    [SerializeField] private Room[] rooms = new Room[6];
    [SerializeField] private Material wallMaterial;
    [SerializeField] private Material floorMaterial;
    
    private void Awake()
    {
        InitializeRooms();
    }
    
    private void InitializeRooms()
    {
        rooms[0] = new Room { roomName = "Cafeteria", position = new Vector3(0, 0, 0), size = new Vector3(20, 3, 20) };
        rooms[1] = new Room { roomName = "Engine", position = new Vector3(30, 0, 0), size = new Vector3(15, 3, 15) };
        rooms[2] = new Room { roomName = "Weapons", position = new Vector3(-30, 0, 0), size = new Vector3(15, 3, 15) };
        rooms[3] = new Room { roomName = "MedBay", position = new Vector3(0, 0, 30), size = new Vector3(15, 3, 15) };
        rooms[4] = new Room { roomName = "Electrical", position = new Vector3(0, 0, -30), size = new Vector3(15, 3, 15) };
        rooms[5] = new Room { roomName = "Security", position = new Vector3(30, 0, 30), size = new Vector3(15, 3, 15) };
    }
    
    public void SpawnEnvironment()
    {
        foreach (var room in rooms)
            CreateRoom(room);
        Debug.Log("[SpaceshipEnvironment] Environment spawned");
    }
    
    private void CreateRoom(Room room)
    {
        GameObject roomObj = new GameObject(room.roomName);
        roomObj.transform.SetParent(transform);
        roomObj.transform.position = room.position;
        
        float wallHeight = room.size.y;
        float wallThickness = 0.3f;
        
        CreateCube(room.roomName + " Floor", new Vector3(room.size.x, 0.2f, room.size.z), Vector3.zero, floorMaterial, roomObj.transform);
        CreateCube(room.roomName + " Front", new Vector3(room.size.x, wallHeight, wallThickness), new Vector3(0, wallHeight / 2, room.size.z / 2), wallMaterial, roomObj.transform);
        CreateCube(room.roomName + " Back", new Vector3(room.size.x, wallHeight, wallThickness), new Vector3(0, wallHeight / 2, -room.size.z / 2), wallMaterial, roomObj.transform);
        CreateCube(room.roomName + " Left", new Vector3(wallThickness, wallHeight, room.size.z), new Vector3(-room.size.x / 2, wallHeight / 2, 0), wallMaterial, roomObj.transform);
        CreateCube(room.roomName + " Right", new Vector3(wallThickness, wallHeight, room.size.z), new Vector3(room.size.x / 2, wallHeight / 2, 0), wallMaterial, roomObj.transform);
        CreateCube(room.roomName + " Ceiling", new Vector3(room.size.x, 0.2f, room.size.z), new Vector3(0, wallHeight, 0), wallMaterial, roomObj.transform);
    }
    
    private GameObject CreateCube(string name, Vector3 size, Vector3 position, Material material, Transform parent)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent);
        cube.transform.position = position;
        cube.transform.localScale = size;
        if (material != null && cube.TryGetComponent<Renderer>(out var renderer))
            renderer.material = material;
        return cube;
    }
}