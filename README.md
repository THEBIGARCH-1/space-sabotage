# Space Sabotage - 3D Multiplayer Social Deduction Game

## 🚀 Project Overview

Space Sabotage is a multiplayer 3D social deduction game set in a sci-fi spaceship. Players are divided into two roles:
- **Crewmates**: Complete tasks around the ship
- **Impostors**: Sabotage the ship and eliminate crewmates without getting caught

## 🎮 Core Features

### Multiplayer Architecture
- Lobby system with room codes (max 10 players per lobby)
- Authoritative server model using Netcode for GameObjects
- Real-time player synchronization

### Gameplay Mechanics
- **Player Controller**: First-person or third-person over-the-shoulder perspective
- **Task System**: Interactive minigames around the spaceship
- **Sabotage System**: Impostor-exclusive actions (lights, doors, oxygen)
- **Ventilation**: Quick traversal system for Impostors
- **Emergency Meetings**: Report deaths, discussion UI, voting system

### Win Conditions
- **Crewmates Win**: Complete all tasks OR vote out all Impostors
- **Impostors Win**: Equal or outnumber Crewmates OR sabotage critical systems

## 🏗️ Development Roadmap

### Phase 1: Project Setup & Networking ✅
- [x] Initialize repository structure
- [x] Basic player movement and synchronization
- [x] Networking manager setup
- [x] Game state manager foundation

### Phase 2: Environment & Character Design
- [ ] Spaceship map (Cafeteria, Engine Room, Weapons, MedBay, Electrical, Security)
- [ ] Player prefab with customizable colors and accessories
- [ ] Environmental interactions

### Phase 3: Core Mechanics
- [ ] Task system with minigames (at least 3)
- [ ] Impostor kill mechanics with cooldown
- [ ] Vent traversal system
- [ ] Global task progress tracking

### Phase 4: Meetings & Voting
- [ ] Death reporting system
- [ ] Emergency meeting UI
- [ ] Voting logic and result handling
- [ ] Game state transitions

## 🛠️ Tech Stack

- **Engine**: Unity 2022 LTS or newer
- **Networking**: Netcode for GameObjects
- **UI**: TextMeshPro
- **Art Style**: Low-poly 3D with bright solid colors
- **Language**: C#

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Networking/
│   │   ├── NetworkManager.cs
│   │   ├── PlayerNetworkSync.cs
│   │   └── LobbyManager.cs
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   ├── PlayerMovement.cs
│   │   ├── PlayerInteraction.cs
│   │   └── PlayerModel.cs
│   ├── Managers/
│   │   ├── GameStateManager.cs
│   │   ├── GameManager.cs
│   │   └── RoleManager.cs
│   ├── Tasks/
│   │   ├── TaskManager.cs
│   │   ├── TaskBase.cs
│   │   └── Minigames/
│   ├── Sabotage/
│   │   ├── SabotageManager.cs
│   │   └── VentSystem.cs
│   ├── Meetings/
│   │   ├── MeetingManager.cs
│   │   ├── VotingSystem.cs
│   │   └── ChatUI.cs
│   └── UI/
│       ├── HUD.cs
│       ├── LobbyUI.cs
│       └── MeetingUI.cs
├── Prefabs/
│   ├── Player/
│   ├── Tasks/
│   ├── Environment/
│   └── UI/
├── Scenes/
│   ├── Lobby.unity
│   ├── MainGame.unity
│   └── MeetingRoom.unity
└── Resources/
    ├── Materials/
    ├── Models/
    └── Audio/
```

## 🚀 Getting Started

1. Clone the repository
2. Open in Unity 2022 LTS or newer
3. Install Netcode for GameObjects via Package Manager
4. Navigate to Assets/Scenes/Lobby.unity
5. Press Play to test multiplayer (open 2 instances locally)

## 📝 Notes

- Use Netcode for GameObjects for networking
- All multiplayer-relevant data should use NetworkVariables
- Client-side prediction for smooth movement
- Server-authoritative for game-critical actions (eliminations, tasks, voting)

## 👥 Contributors

- THEBIGARCH-1

## 📄 License

MIT License - See LICENSE file for details
