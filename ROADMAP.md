# Space Sabotage - Development Roadmap

## Phase 1: Project Setup & Networking ✅

**Status**: In Progress

### Objectives
- [x] Repository initialization with proper .gitignore
- [x] Core networking scripts structure
- [x] Player movement and synchronization framework
- [x] Game state manager
- [x] Lobby manager
- [ ] Test with 2-10 players locally
- [ ] Verify movement sync accuracy
- [ ] Optimize network update rate

### Key Files Created
- `NetworkManager.cs` - Connection/disconnection handling
- `PlayerNetworkSync.cs` - Position/rotation synchronization
- `PlayerController.cs` - Input and camera control
- `PlayerModel.cs` - Player data and role storage
- `PlayerInteraction.cs` - Task and action system foundation
- `GameStateManager.cs` - Game state transitions
- `GameManager.cs` - Player and role management
- `LobbyManager.cs` - Lobby and ready system

### Testing Checklist
- [ ] 2 players can connect locally
- [ ] Movement syncs smoothly between clients
- [ ] No clipping through walls
- [ ] Disconnect handling works properly
- [ ] Roles assign correctly
- [ ] Game state transitions smoothly

### Next Steps
1. Create basic test scene with Netcode setup
2. Create player prefab with required components
3. Test with 2 instances of game
4. Fix any sync issues
5. Proceed to Phase 2

---

## Phase 2: Environment & Character Design

**Status**: Not Started

### Objectives
- [ ] Blockout spaceship map
- [ ] Create all 6 rooms (Cafeteria, Engine, Weapons, MedBay, Electrical, Security)
- [ ] Player model customization (colors, accessories)
- [ ] Door and environmental interactions
- [ ] Spawn points for both roles
- [ ] Lighting and atmosphere

### Deliverables
- Spaceship scene with interconnected rooms
- Player prefab with color customization
- Environmental interactive elements
- Spawn manager for multiple spawn points

---

## Phase 3: Core Mechanics

**Status**: Not Started

### Objectives
- [ ] Implement at least 3 minigames:
  - [ ] Align Engine Output
  - [ ] Download Data
  - [ ] Fix Wiring
- [ ] Global task progress tracking
- [ ] Impostor kill system with cooldown (45 seconds default)
- [ ] Vent system with defined vent locations
- [ ] Sabotage system (lights, doors, oxygen)
- [ ] HUD updates in real-time

### Deliverables
- Functional task system
- At least 3 playable minigames
- Kill cooldown mechanics
- Vent traversal system
- Sabotage mechanics with global effects

---

## Phase 4: Meetings & Voting

**Status**: Not Started

### Objectives
- [ ] Dead body reporting system
- [ ] Emergency meeting trigger (from Cafeteria button)
- [ ] Meeting UI with discussion tab
- [ ] Chat system (text-based)
- [ ] Voting UI and mechanics
- [ ] Voting result calculation and player ejection
- [ ] Post-vote game state handling

### Deliverables
- Complete meeting system
- Voting logic and UI
- Chat system
- Win condition checks
- Game end sequences

---

## Post-Launch Features (Backlog)

- [ ] Cosmetics shop (hats, colors, pets)
- [ ] Progression system (levels, achievements)
- [ ] Ranked mode
- [ ] Replay system
- [ ] Custom game options
- [ ] Mobile version
- [ ] Cross-platform multiplayer
- [ ] Voice chat integration
- [ ] Spectator mode
- [ ] Anti-cheat system

---

## Current Sprint: Phase 1

### This Week
1. Set up test scene with Netcode networking
2. Create player prefab
3. Test 2-player sync
4. Debug movement desync issues
5. Optimize network update rate

### Blocking Issues
- None currently

### Notes
- Use NetworkVariables for state that needs synchronization
- Use RPCs for immediate actions (kills, voting)
- Test frequently with multiple instances
