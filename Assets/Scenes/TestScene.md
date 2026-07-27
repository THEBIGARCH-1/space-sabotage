# Test Scene Setup Guide

## How to Create TestScene.unity

### Step 1: Create New Scene
1. Right-click in Assets/Scenes folder
2. Create → Scene
3. Name it "TestScene.unity"
4. Save it

### Step 2: Create Ground
1. Right-click in hierarchy → 3D Object → Plane
2. Name it "Ground"
3. Scale: (100, 1, 100) - makes large floor
4. Position: (0, 0, 0)
5. Add Collider (Mesh Collider, convex = false)
6. Drag Green material on it (or create new)

### Step 3: Create NetworkManager
1. Right-click in hierarchy → Create Empty
2. Name it "NetworkManager"
3. Position: (0, 0, 0)
4. Add Component → NetworkManager (script)
5. Assign in Inspector:
   - Player Prefab: (drag PlayerPrefab from Assets/Prefabs)
   - Spawn Point: Create empty object at (0, 1, 0), assign it
   - Max Players: 10

### Step 4: Create GameManager
1. Right-click in hierarchy → Create Empty
2. Name it "GameManager"
3. Add Component → GameManager (script)
4. Add Component → GameStateManager (script)
5. Assign:
   - Min Players To Start: 2
   - Impostor Count: 1 (for testing)
   - Crewmate Spawn Points: Create 5 empty objects around map
   - Impostor Spawn Points: Create 2 empty objects

### Step 5: Create LobbyManager
1. Right-click in hierarchy → Create Empty
2. Name it "LobbyManager"
3. Add Component → LobbyManager (script)
4. Assign:
   - Max Players Per Lobby: 10
   - Min Players To Start: 2

### Step 6: Create Player Prefab
1. Right-click → Create Empty → name "Player"
2. Add Child → 3D Object → Capsule (this is the body)
3. Add Child → Camera (this is the view)
4. Set camera position to (0, 0.6, 0) to be at eye level
5. Add Component → NetworkObject (from Netcode)
6. Add Component → PlayerNetworkSync
7. Add Component → PlayerController
8. Add Component → PlayerModel
9. Add Component → PlayerInteraction
10. Add Rigidbody:
    - Mass: 1
    - Drag: 0.1
    - Angular Drag: 0.05
    - Freeze Rotation X, Y, Z (to prevent tipping)
11. Assign Ground Layer to Rigidbody Ground Detection
12. Drag this into Assets/Prefabs/Player as "PlayerPrefab"
13. Delete from hierarchy

### Step 7: Unity Netcode Setup
1. Create empty GameObject → name "NetcodeSetup"
2. Add Component → Unity.Netcode.NetworkManager
3. Settings:
   - Network Prefabs list → Add "PlayerPrefab"
   - Connection Approval: Disable for now
   - Scene Management: Server Loads Scene for Clients

### Step 8: Test
1. Save scene
2. Press Play
3. Open second Unity instance
4. Press Play in second instance
5. Both should connect

### Troubleshooting
- If players don't spawn: Check PlayerPrefab is in Network Prefabs
- If movement doesn't sync: Verify PlayerNetworkSync is enabled
- If no connection: Ensure both instances use same build settings
