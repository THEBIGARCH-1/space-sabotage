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
3. Scale: (100, 1, 100)
4. Position: (0, 0, 0)
5. Add Mesh Collider

### Step 3: Create NetworkManager
1. Right-click → Create Empty
2. Name: "NetworkManager"
3. Add Component → NetworkManager script
4. Assign Player Prefab and Spawn Point

### Step 4: Create GameManager
1. Right-click → Create Empty
2. Name: "GameManager"
3. Add Component → GameManager script
4. Add Component → GameStateManager script

### Step 5: Test
1. Save scene
2. Press Play
3. Open second Unity instance
4. Both should connect
