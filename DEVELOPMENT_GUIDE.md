# Space Sabotage - Development Guide

## Architecture Overview

### Networking Architecture

We use **Netcode for GameObjects** for client-server networking:
- **Server-Authoritative**: Game logic and validation happen on the server
- **Client-Side Prediction**: Smooth movement and immediate feedback on client
- **NetworkVariables**: Synchronized game state
- **RPC Calls**: Event-driven communications

### Game State Flow

```
Lobby → Loading → Gameplay → Meeting → Voting → Lobby
```

### Role System

- **Crewmates**: Complete tasks, vote to find Impostors
- **Impostors**: Eliminate Crewmates, sabotage systems, complete fake tasks
- Role assignment happens server-side after minimum players ready

## Script Breakdown

### Networking Layer

#### `NetworkManager.cs`
- Handles connection/disconnection
- Manages lobby creation and joining
- Spawns player network objects
- Manages game state synchronization

#### `PlayerNetworkSync.cs`
- Synchronizes player position, rotation, animation state
- Handles movement over the network
- Client-side prediction for local player
- Server-side validation for movement bounds

#### `LobbyManager.cs`
- Manages player list in lobbies
- Ready state tracking
- Start game when all players ready or countdown expires
- Player role assignment (server-side)

### Player System

#### `PlayerController.cs`
- Main player behavior coordinator
- Handles input processing
- Manages player state (alive, dead, voting)
- Coordinates with other systems

#### `PlayerMovement.cs`
- Physics-based movement (WASD + Space)
- Collision detection with spaceship environment
- Gravity and jump mechanics
- Networked synchronization

#### `PlayerInteraction.cs`
- Raycast for task interaction
- Kill prompt detection (Impostors only)
- Vent detection and traversal
- Reporting dead bodies

#### `PlayerModel.cs`
- Player data storage (name, role, color, accessories)
- Statistics tracking (tasks completed, eliminations)
- Loadout customization

### Game Management

#### `GameStateManager.cs`
- Tracks current game state
- Handles state transitions
- Synchronizes state across network
- Manages timers and win conditions

#### `GameManager.cs`
- Main game loop coordinator
- Initializes game systems
- Handles player spawning
- Manages game ending conditions

#### `RoleManager.cs`
- Assigns roles to players
- Tracks role-specific data
- Handles fake task UI for Impostors

## Development Workflow

### 1. Local Testing

```bash
# Test multiplayer locally by opening 2 Unity instances
# 1st Instance: Press Play (acts as host)
# 2nd Instance: Press Play (connects as client)
```

### 2. Commit Guidelines

```
[PHASE] [COMPONENT]: Brief description

Examples:
- [P1] [Network]: Add player movement synchronization
- [P2] [Environment]: Create spaceship lobby room
- [P3] [Task]: Implement wire puzzle minigame
```

### 3. Testing Checklist

- [ ] Movement syncs across clients
- [ ] No clipping through walls
- [ ] Players can join and leave
- [ ] State is consistent across clients
- [ ] No memory leaks in inspector

## Common Issues & Solutions

### Issue: Players desync (position differs on each client)
**Solution**: Ensure PlayerNetworkSync is properly networked and update frequency is high enough (50-100Hz)

### Issue: Input lag or jittery movement
**Solution**: Implement client-side prediction and smooth interpolation between network updates

### Issue: Game crashes on disconnect
**Solution**: Add null checks and unsubscribe from network events properly

## Performance Optimization Tips

1. **Network Update Rate**: Adjust based on player count
   - 2-4 players: 100Hz (every 10ms)
   - 5-7 players: 50Hz (every 20ms)
   - 8-10 players: 30Hz (every 33ms)

2. **Interest Management**: Only sync relevant players to each client

3. **Pooling**: Reuse networked objects instead of Instantiate/Destroy

4. **Physics**: Use layer-based collision to reduce physics checks

## Phase Completion Criteria

### Phase 1 Complete When:
- ✅ Players can join lobby with room code
- ✅ Players can load into game scene
- ✅ Movement synchronizes smoothly
- ✅ Game state transitions work
- ✅ 2-10 players can play simultaneously

### Phase 2 Complete When:
- ✅ Spaceship map is explorable
- ✅ Player models are customizable
- ✅ All 6 rooms are accessible
- ✅ Environmental interactions work (doors, buttons)

### Phase 3 Complete When:
- ✅ At least 3 minigames are functional
- ✅ Task progress bar updates in real-time
- ✅ Impostors can eliminate Crewmates with cooldown
- ✅ Vent system works for Impostors

### Phase 4 Complete When:
- ✅ Dead body reporting works
- ✅ Emergency meetings function
- ✅ Voting UI and logic are complete
- ✅ Game end conditions trigger correctly

## Next Steps

1. Open the project in Unity
2. Install Netcode for GameObjects from Package Manager
3. Review NetworkManager.cs to understand networking flow
4. Test local multiplayer with 2+ instances
5. Proceed to Phase 2 when Phase 1 is stable
