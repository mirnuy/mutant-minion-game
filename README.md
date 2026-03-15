# Mutant Minion Game – MVP Prototype

> **Extraction Survival PvPvE** – Control mutated human minions in dangerous raids.  
> Built with Unity 6 · URP · New Input System

---

## 🎮 Controls

| Action | Keyboard / Mouse | Gamepad |
|---|---|---|
| Move | `WASD` | Left Stick |
| Look / Camera | Mouse Move | Right Stick |
| Sprint | Hold `Left Shift` | Left Shoulder |
| Jump | `Space` | Button South (A / ✕) |
| Light Attack | `Left Mouse Button` | Button West (X / □) |
| Block *(reserved)* | `Right Mouse Button` | Button East (B / ○) |
| Interact | `F` | Button North (Y / △) |
| Ability | `Q` | Right Shoulder |

> All bindings are defined in `Assets/Input/PlayerInputActions.inputactions` and can be remapped in the Settings menu.

---

## 📁 Project Structure

```
Assets/
├── Input/
│   └── PlayerInputActions.inputactions   ← New Input System config (remappable)
├── Prefabs/
│   └── Player/
│       └── Player.prefab                 ← Ready-to-drop player prefab
├── Scenes/
│   └── MainPrototype.unity               ← Test scene: ground + player
└── Scripts/
    ├── Input/
    │   └── InputManager.cs               ← Centralised input handler
    ├── Managers/
    │   └── GameManager.cs                ← Game-state machine
    ├── Player/
    │   ├── PlayerController.cs           ← Master orchestrator / singleton
    │   ├── PlayerMovement.cs             ← WASD, sprint, jump, gravity
    │   ├── PlayerCamera.cs               ← Third-person follow camera
    │   ├── PlayerCombat.cs               ← Light attack + hit detection
    │   ├── PlayerStamina.cs              ← Stamina pool & regen
    │   ├── PlayerAbility.cs              ← Mutation ability slot
    │   └── PlayerInteraction.cs          ← World interaction (loot, extraction)
    └── Utils/
        └── Constants.cs                  ← Shared config values
```

---

## 🚀 Quick Start

### 1. Open in Unity 6
```bash
git clone https://github.com/mirnuy/mutant-minion-game.git
```
Open the project in Unity 6 (URP template).

### 2. Install the New Input System
1. `Window → Package Manager → Unity Registry`
2. Search **Input System** → Install
3. Unity will ask to enable the new backend → click **Yes** and restart

### 3. Open the Prototype Scene
`Assets/Scenes/MainPrototype.unity`

### 4. Set Up the Player Prefab
1. Drag `Assets/Prefabs/Player/Player.prefab` into the scene (or create a primitive Cube).
2. Add these components to the Player object:
   - `CharacterController`
   - `InputManager`
   - `PlayerController`
   - `PlayerMovement`
   - `PlayerCombat`
   - `PlayerStamina`
   - `PlayerAbility`
   - `PlayerInteraction`
   - `PlayerInput` (set **Behavior** to *Invoke Unity Events*, link actions to `InputManager` methods)
3. Add a **Camera** (child of scene, not player) with `PlayerCamera` component.
4. Assign the player transform to the camera's **Target** field.

---

## ⚙️ Inspector Configuration

### PlayerMovement
| Field | Default | Description |
|---|---|---|
| Move Speed | 5 | Walking speed (units/s) |
| Sprint Speed | 8 | Sprint speed (units/s) |
| Jump Force | 5 | Upward velocity on jump |
| Ground Check Radius | 0.25 | Sphere size for ground detection |

### PlayerCamera
| Field | Default | Description |
|---|---|---|
| Distance | 5 | Camera–player horizontal distance |
| Height | 2 | Vertical offset above player |
| Sensitivity | 3 | Mouse / stick sensitivity |
| Min / Max Pitch | -30° / 60° | Vertical look clamp |

### PlayerStamina
| Field | Default | Description |
|---|---|---|
| Max Stamina | 100 | Total stamina pool |
| Regen Rate | 15/s | Stamina recovered per second |
| Regen Delay | 1.5s | Seconds before regen starts |

---

## 🧬 Mutation System

Apply mutations via `PlayerController.ApplyMutation(IMutation mutation)`.

### Implementing a Mutation
```csharp
using MutantMinion.Player;

public class WolfDNAMutation : IMutation
{
    public void Apply(PlayerController controller)
    {
        controller.Movement.SetMoveSpeed(8f);      // Wolf → faster movement
        controller.Movement.SetSprintSpeed(12f);   // Wolf → faster sprint
    }
}
```

```csharp
public class FrogDNAMutation : IMutation
{
    public void Apply(PlayerController controller)
    {
        controller.Movement.SetJumpForce(9f);      // Frog → higher jump
    }
}
```

### Implementing an Ability
```csharp
using MutantMinion.Player;

public class SpiderPoisonAbility : IAbility
{
    public string Name        => "Spider Poison";
    public float  Cooldown    => 8f;
    public float  StaminaCost => 20f;

    public void Activate(GameObject owner)
    {
        // Apply poison AOE around player, etc.
    }
}

// Equip it:
controller.Ability.SetAbility(new SpiderPoisonAbility());
```

### Mutation Hooks at a Glance
| DNA | Target Method | Effect |
|---|---|---|
| Wolf | `Movement.SetMoveSpeed` / `SetSprintSpeed` | Speed boost |
| Frog | `Movement.SetJumpForce` | Higher jump |
| Troll | `Stamina.SetRegenRate` / `SetMaxStamina` | Regen boost |
| Spider | `Combat.OnAttackHit` event + `IAbility` | Poison on hit |
| Bat | `Interaction.detectionRadius` (via subclass) | Extended perception |

---

## 🗺️ Interactable Objects

Make any world object interactable:

```csharp
using MutantMinion.Player;
using UnityEngine;

public class LootChest : MonoBehaviour, IInteractable
{
    public string InteractPrompt => "Press F to loot";

    public void Interact(GameObject interactor)
    {
        Debug.Log("Player looted the chest!");
        // Add items to inventory here
        gameObject.SetActive(false);
    }
}
```

Tag the object with `Interactable` and ensure it has a Collider (can be a Trigger).

---

## 🔜 Planned Features

- [ ] Block / Parry mechanic (Right Mouse)
- [ ] Enemy AI with patrol and aggro
- [ ] Inventory and loot system
- [ ] Extraction point trigger
- [ ] Laboratory hub scene
- [ ] Minion creation system
- [ ] Multiplayer (PvPvE)
- [ ] Visual mutation morphing

---

## 📝 Notes

- Code is heavily commented for learning and team onboarding.
- All `Debug.Log` calls can be stripped for release with a custom logger.
- The `IMutation` and `IAbility` interfaces make the system extensible without modifying existing files.

---

*Built with ❤️ for the Mutant Minion Game MVP*
