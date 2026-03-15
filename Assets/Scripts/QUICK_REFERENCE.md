# 🎮 Quick Reference Card

## File Structure Created
```
Assets/Scripts/
├── PlayerController.cs          (Main orchestrator)
├── Player/
│   ├── PlayerMovement.cs        (WASD, Sprint, Jump)
│   ├── PlayerCamera.cs          (Third-person camera)
│   ├── PlayerCombat.cs          (Light attack)
│   ├── PlayerStamina.cs         (Stamina system)
│   ├── PlayerInteraction.cs     (World interaction)
│   ├── PlayerAbility.cs         (Mutation system)
│   ├── InputManager.cs          (Input routing)
│   └── PlayerInputActions.cs    (Input placeholder)
├── Examples/
│   ├── ExampleInteractable.cs   (Sample interactable)
│   └── ExampleEnemy.cs          (Sample enemy)
├── SETUP_GUIDE.md               (Full setup instructions)
└── ARCHITECTURE.md              (System architecture docs)
```

## Quick Setup Checklist

### 1. Install Input System Package
- [ ] Window → Package Manager → Input System → Install

### 2. Create Input Actions
- [ ] Right-click → Create → Input Actions → "PlayerInputActions"
- [ ] Add actions: Move, Look, Jump, Sprint, Attack, Block, Interact, Ability
- [ ] Generate C# Class (replace placeholder)

### 3. Build Player GameObject
- [ ] Create Empty "Player" with Tag "Player"
- [ ] Add Character Controller
- [ ] Add PlayerController (auto-adds all components)
- [ ] Create child "GroundCheck" at (0, 0, 0)
- [ ] Create child "AttackPoint" at (0, 1, 1)

### 4. Build Camera
- [ ] Create Empty "PlayerCamera"
- [ ] Add Camera + PlayerCamera script
- [ ] Assign Player to Target field

### 5. Create Layers
- [ ] Layer 6: Ground
- [ ] Layer 7: Enemy
- [ ] Layer 8: Interactable

### 6. Test Scene
- [ ] Floor cube (20×0.5×20) with Ground layer
- [ ] Test enemy cube with ExampleEnemy + Enemy layer
- [ ] Test interactable with ExampleInteractable + Interactable layer

## Default Controls
| Action | Keyboard | Gamepad |
|--------|----------|---------|
| Move | WASD | Left Stick |
| Look | Mouse | Right Stick |
| Jump | Space | A Button |
| Sprint | Left Shift | LS Click |
| Attack | Left Click | X Button |
| Block | Right Click | B Button |
| Interact | F | Y Button |
| Ability | Q | LB |

## Key Properties to Configure

### PlayerMovement
- Walk Speed: `5f`
- Sprint Speed: `8f`
- Jump Height: `2f`
- Ground Mask: `Ground Layer`

### PlayerCamera
- Distance: `5f`
- Mouse Sensitivity: `2f`
- Min/Max Vertical: `-40° / 70°`

### PlayerCombat
- Attack Damage: `10f`
- Attack Range: `2f`
- Enemy Layer: `Enemy Layer`

### PlayerStamina
- Max Stamina: `100f`
- Sprint Drain: `10f/sec`
- Regen Rate: `15f/sec`

### PlayerInteraction
- Range: `2f`
- Interactable Mask: `Interactable Layer`

## Creating Abilities

### Wolf Ability (Speed + Damage)
```
Right-click → Create → Mutant Minion → Ability Data
- Name: WolfAbility
- Type: Wolf
- Stamina: 20
- Duration: 5s
- Speed Multiplier: 1.5x
- Damage Multiplier: 1.3x
```

### Frog Ability (Jump Boost)
```
Right-click → Create → Mutant Minion → Ability Data
- Name: FrogAbility
- Type: Frog
- Stamina: 15
- Duration: 10s
- Jump Multiplier: 2x
```

## Code Examples

### Equip Ability
```csharp
PlayerController player = FindObjectOfType<PlayerController>();
player.Ability.EquipAbility(abilityData);
```

### Disable Controls (Cutscene)
```csharp
player.SetControlsEnabled(false);
```

### Listen to Stamina Changes
```csharp
player.Stamina.OnStaminaChanged += (current, max) => {
    Debug.Log($"Stamina: {current}/{max}");
};
```

### Create Custom Interactable
```csharp
public class Door : MonoBehaviour, IInteractable
{
    public void Interact(GameObject player) 
    { 
        // Open door logic
    }
    
    public bool CanInteract(GameObject player) 
    { 
        return !isOpen; 
    }
    
    public string GetInteractionPrompt() 
    { 
        return "Press F to open"; 
    }
    
    public void OnInteractableEnter(GameObject player) { }
    public void OnInteractableExit(GameObject player) { }
}
```

### Create Custom Enemy
```csharp
public class Boss : MonoBehaviour, IDamageable
{
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }
}
```

## Animation Integration (Future)

### Animator Parameters
- `IsMoving` (Bool)
- `IsSprinting` (Bool)
- `IsGrounded` (Bool)
- `Speed` (Float)
- `LightAttack` (Trigger)
- `Jump` (Trigger)

### Access in Code
```csharp
Animator anim = GetComponent<Animator>();
anim.SetBool("IsMoving", movement.IsMoving);
anim.SetBool("IsSprinting", movement.IsSprinting);
```

## Common Debug Commands

### Scene View Gizmos
- Select Player → Ground check sphere (green/red)
- Select Player → Attack range sphere (yellow/red)
- Select Player → Interaction range (yellow/green)

### Console Logs
- Enable in PlayerController: `showDebugInfo = true`
- Shows: Attacks, Interactions, Stamina, Abilities

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Input not working | Install Input System package, generate C# class |
| Falling through floor | Set floor to Ground layer, check Ground Mask |
| Camera not following | Assign Player to Camera Target field |
| Can't hit enemies | Set enemies to Enemy layer, check attack range |
| Can't interact | Set objects to Interactable layer, check range |

## Next Steps
1. ✅ System created and compiling
2. ⏭️ Create Input Action Asset (replace placeholder)
3. ⏭️ Set up scene with player, camera, floor
4. ⏭️ Create test enemy and interactable
5. ⏭️ Create ability ScriptableObjects
6. ⏭️ Add animations
7. ⏭️ Build UI system
8. ⏭️ Implement mutation transformation

---
📖 **Full Documentation:** See `SETUP_GUIDE.md` and `ARCHITECTURE.md`
