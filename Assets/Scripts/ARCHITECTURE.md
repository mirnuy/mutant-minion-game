# 🎮 Mutant Minion Character Controller Architecture

## System Design Philosophy

This character controller follows **SOLID principles** with clean separation of concerns, making it highly modular and extensible for your mutation-based game.

## 📐 Architecture Diagram

```
PlayerController (Orchestrator)
    │
    ├── InputManager (Input Events)
    │   └── Distributes to all systems
    │
    ├── PlayerMovement (Locomotion)
    │   ├── Walk/Sprint
    │   ├── Jump
    │   └── Ground Detection
    │
    ├── PlayerCamera (Third-Person View)
    │   ├── Mouse Look
    │   ├── Zoom
    │   └── Collision Detection
    │
    ├── PlayerCombat (Combat System)
    │   ├── Light Attack
    │   └── Block (Reserved)
    │
    ├── PlayerStamina (Resource Management)
    │   ├── Sprint Drain
    │   ├── Ability Cost
    │   └── Regeneration
    │
    ├── PlayerInteraction (World Interaction)
    │   └── IInteractable Interface
    │
    └── PlayerAbility (Mutation System)
        ├── Wolf (Speed + Damage)
        ├── Frog (Jump Height)
        └── Custom (Extensible)
```

## 🔄 Data Flow

### Input Flow
```
User Input 
  → Unity Input System
    → InputManager
      → Specific Component (Movement/Combat/etc.)
        → Game Logic
          → Visual/Audio Feedback
```

### Ability Activation Flow
```
Q Key Press
  → InputManager.OnAbilityPerformed()
    → PlayerAbility.OnUseAbility()
      → Check Stamina (PlayerStamina)
        → Apply Mutation Effects
          → Modify Movement/Combat Stats
            → Event Notification (UI Updates)
```

## 🧩 Component Responsibilities

### PlayerController.cs
**Purpose:** Central orchestrator and component manager
- Auto-assigns all player components
- Provides unified access point
- Manages component lifecycle
- Debug event logging

**Dependencies:** All player systems
**Used By:** External systems (UI, game managers)

---

### InputManager.cs
**Purpose:** Centralized input handling using New Input System
- Routes input to appropriate components
- Supports input remapping
- Enables/disables input for game states
- Supports multiple input devices

**Dependencies:** All player systems
**Used By:** Unity Input System

**Key Features:**
- Event-driven architecture
- Gamepad + Keyboard/Mouse support
- Easily extendable for new actions

---

### PlayerMovement.cs
**Purpose:** Character locomotion and physics
- WASD movement relative to camera
- Sprint with stamina consumption
- Jump with ground detection
- Smooth rotation toward movement

**Dependencies:** 
- CharacterController (Unity)
- PlayerStamina
- Main Camera

**Mutation Support:**
- `ApplySpeedBoost(float)` - Wolf mutation
- `ApplyJumpBoost(float)` - Frog mutation

**Properties:**
- `IsMoving` - For animations
- `IsSprinting` - For animations/UI
- `IsGrounded` - For animations
- `CurrentSpeed` - Current movement speed

---

### PlayerCamera.cs
**Purpose:** Third-person camera control
- Mouse look with sensitivity
- Vertical angle clamping
- Camera zoom
- Collision avoidance

**Dependencies:**
- Player Transform (target)

**Key Features:**
- Smooth rotation interpolation
- Raycast-based collision
- Configurable offset and distance
- Cursor lock management

---

### PlayerCombat.cs
**Purpose:** Combat system and damage dealing
- Light attack with cooldown
- Sphere overlap detection
- Direction-based hit detection
- Damage multiplier support

**Dependencies:**
- PlayerStamina
- Animator (optional)

**Interfaces:**
- Implements damage dealing
- Uses `IDamageable` interface for enemies

**Mutation Support:**
- `ApplyDamageMultiplier(float)` - Wolf mutation

**Future Extensions:**
- Block system (reserved)
- Combo system
- Heavy attacks
- Special moves

---

### PlayerStamina.cs
**Purpose:** Resource management system
- Sprint stamina drain
- Ability cost management
- Regeneration with delay
- Event notifications

**Events:**
- `OnStaminaChanged` - For UI updates

**Key Methods:**
- `UseSprint(float)` - Sprint consumption
- `UseAbilityStamina(float)` - Ability consumption
- `RestoreStamina(float)` - For pickups
- `IncreaseMaxStamina(float)` - For upgrades

**Mutation Support:**
- `ModifyRegenRate(float)` - Mutation effects

---

### PlayerInteraction.cs
**Purpose:** World object interaction system
- Proximity detection
- Interaction prompts
- Interface-based design

**Interfaces:**
- Uses `IInteractable` interface

**Key Features:**
- Sphere overlap detection
- Direction-based filtering
- Closest object prioritization
- Enter/exit notifications

**IInteractable Methods:**
```csharp
void Interact(GameObject player);
bool CanInteract(GameObject player);
string GetInteractionPrompt();
void OnInteractableEnter(GameObject player);
void OnInteractableExit(GameObject player);
```

---

### PlayerAbility.cs
**Purpose:** Mutation ability system
- Single ability slot
- Mutation-specific effects
- Cooldown management
- Stamina cost handling

**Mutation Types:**
- **Wolf** - Speed + Damage boost
- **Frog** - Jump enhancement
- **Custom** - Extensible for new mutations

**Events:**
- `OnAbilityActivated` - When ability starts
- `OnAbilityDeactivated` - When ability ends
- `OnAbilityChanged` - When ability equipped

**Key Methods:**
- `EquipAbility(AbilityData)` - Change ability
- `OnUseAbility()` - Activate ability
- `ApplyAbilityEffects()` - Apply mutation
- `RemoveAbilityEffects()` - Revert mutation

**AbilityData (ScriptableObject):**
```csharp
- abilityName: string
- mutationType: MutationType
- staminaCost: float
- duration: float
- speedMultiplier: float
- jumpMultiplier: float
- damageMultiplier: float
```

---

## 🔌 Interfaces

### IDamageable
**Purpose:** Standard interface for entities that can take damage

```csharp
public interface IDamageable
{
    void TakeDamage(float damage);
}
```

**Implemented By:**
- ExampleEnemy
- Future: Bosses, Destructibles, Player (for health)

---

### IInteractable
**Purpose:** Standard interface for interactive world objects

```csharp
public interface IInteractable
{
    void Interact(GameObject player);
    bool CanInteract(GameObject player);
    string GetInteractionPrompt();
    void OnInteractableEnter(GameObject player);
    void OnInteractableExit(GameObject player);
}
```

**Implemented By:**
- ExampleInteractable
- Future: Doors, Chests, NPCs, Pickups, Mutation Stations

---

## 🎯 Design Patterns Used

### 1. **Component Pattern**
Each system is a self-contained component with clear responsibilities.

### 2. **Observer Pattern**
Events for system communication (stamina changes, ability state, etc.)

### 3. **Strategy Pattern**
Mutation system allows swapping abilities at runtime

### 4. **Interface Segregation**
Small, focused interfaces (IDamageable, IInteractable)

### 5. **Dependency Injection**
Components get references through Unity's component system

---

## 🔄 Extension Points

### Adding New Mutations
1. Add enum value to `MutationType`
2. Create `ApplyXEffects()` method
3. Create `RemoveXEffects()` method
4. Create ScriptableObject ability data

### Adding New Input Actions
1. Add action to Input Action Asset
2. Add event handler in InputManager
3. Add corresponding method in target component

### Adding New Interactable Types
1. Create new class implementing `IInteractable`
2. Configure detection layers
3. Implement interaction logic

### Adding New Enemy Types
1. Create new class implementing `IDamageable`
2. Add enemy-specific AI/behavior
3. Configure combat layers

---

## 📊 Performance Considerations

### Optimizations
- **Ground Check:** Single sphere cast per frame
- **Interaction:** Sphere overlap with layer masking
- **Combat:** Sphere overlap only during attack
- **Camera:** Raycast only for collision detection

### Best Practices
- Layer masks reduce physics overhead
- Gizmos only in editor (OnDrawGizmosSelected)
- Object pooling ready for future implementation
- Event-driven updates (not polling)

---

## 🧪 Testing Strategy

### Unit Testing Points
- Stamina drain/regen calculations
- Ability cooldown logic
- Damage calculations with multipliers
- Ground detection accuracy

### Integration Testing
- Input → Movement flow
- Combat → Enemy damage flow
- Ability → Stat modification flow
- Interaction → Object response flow

---

## 🚀 Future Enhancements

### Planned Features
1. **Health System** - Player HP with death/respawn
2. **Block Mechanic** - Right-click blocking
3. **Animation Integration** - Full animator setup
4. **Combo System** - Attack chaining
5. **More Mutations** - Bear, Eagle, Snake, etc.
6. **Mutation Transformation** - Visual model swapping
7. **UI System** - Health/Stamina/Ability bars
8. **Sound System** - Audio feedback
9. **Particle Effects** - Visual feedback
10. **Save System** - Ability persistence

### Extensibility Ready
- Inventory system hooks
- Quest system integration
- Dialogue system support
- Stats/RPG system foundation
- Multiplayer-ready architecture

---

## 📝 Code Standards

### Naming Conventions
- **Public Fields:** PascalCase with `[SerializeField]`
- **Private Fields:** camelCase
- **Methods:** PascalCase
- **Events:** PascalCase with "On" prefix
- **Interfaces:** PascalCase with "I" prefix

### Documentation
- XML comments on public methods
- Summary comments on classes
- Inline comments for complex logic
- Gizmos for visual debugging

### File Organization
```
Assets/
├── Scripts/
│   ├── Player/           (All player systems)
│   ├── Examples/         (Example implementations)
│   └── SETUP_GUIDE.md    (Setup instructions)
```

---

## 🎓 Learning Resources

This system demonstrates:
- Unity's New Input System
- CharacterController physics
- Event-driven architecture
- Interface-based design
- ScriptableObject patterns
- Clean code principles
- Modular game systems

---

**Built with ❤️ for mutation-based gameplay**
