# Mutant Minion Character Controller - Setup Guide

## 📦 System Overview

This character controller system is designed for Unity 6 URP with a modular architecture. All scripts are mutation-ready and follow clean separation of concerns.

## 🎮 Components

### Core Player Scripts (All in `Assets/Scripts/Player/`)
1. **PlayerController.cs** - Main orchestrator (attach to player GameObject)
2. **PlayerMovement.cs** - WASD movement, sprint, jump
3. **PlayerCamera.cs** - Third-person camera with mouse look
4. **PlayerCombat.cs** - Light attack system (left click)
5. **PlayerStamina.cs** - Stamina management
6. **PlayerInteraction.cs** - World object interaction (F key)
7. **PlayerAbility.cs** - Mutation ability system
8. **InputManager.cs** - New Input System integration

### Example Scripts (In `Assets/Scripts/Examples/`)
- **ExampleEnemy.cs** - Sample enemy with IDamageable interface
- **ExampleInteractable.cs** - Sample interactable object

## 🚀 Quick Setup

### Step 1: Install Unity's New Input System
1. Open Package Manager (Window > Package Manager)
2. Search for "Input System"
3. Install the package
4. When prompted, allow Unity to restart

### Step 2: Create Input Action Asset
1. Right-click in Project window
2. Create -> Input Actions
3. Name it "PlayerInputActions"
4. Double-click to open Input Actions editor
5. Create Action Map named "Player" with these actions:

   **Action Map: Player**

   **IMPORTANT:** For Vector2 actions with keyboard, use "Add 2D Vector Composite" (NOT "Add Binding")!

   Create these actions:

   a. **Move** (Action Type: Value, Control Type: Vector2)
      - Click **+** next to Move → **"Add 2D Vector Composite"**
        - Up: `<Keyboard>/w`
        - Down: `<Keyboard>/s`
        - Left: `<Keyboard>/a`
        - Right: `<Keyboard>/d`
      - Click **+** next to Move → **"Add Binding"** → `<Gamepad>/leftStick`

   b. **Look** (Action Type: Value, Control Type: Vector2)
      - Click **+** → **"Add Binding"** → `<Mouse>/delta`
      - Click **+** → **"Add Binding"** → `<Gamepad>/rightStick`

   c. **Jump** (Action Type: Button)
      - Click **+** → **"Add Binding"** → `<Keyboard>/space`
      - Click **+** → **"Add Binding"** → `<Gamepad>/buttonSouth`

   d. **Sprint** (Action Type: Button)
      - Click **+** → **"Add Binding"** → `<Keyboard>/leftShift`
      - Click **+** → **"Add Binding"** → `<Gamepad>/leftStickPress`

   e. **Attack** (Action Type: Button)
      - Click **+** → **"Add Binding"** → `<Mouse>/leftButton`
      - Click **+** → **"Add Binding"** → `<Gamepad>/buttonWest`

   f. **Block** (Action Type: Button)
      - Click **+** → **"Add Binding"** → `<Mouse>/rightButton`
      - Click **+** → **"Add Binding"** → `<Gamepad>/buttonEast`

   g. **Interact** (Action Type: Button)
      - Click **+** → **"Add Binding"** → `<Keyboard>/f`
      - Click **+** → **"Add Binding"** → `<Gamepad>/buttonNorth`

   h. **Ability** (Action Type: Button)
      - Click **+** → **"Add Binding"** → `<Keyboard>/q`
      - Click **+** → **"Add Binding"** → `<Gamepad>/leftShoulder`

6. In Inspector, click "Generate C# Class"
   - Class Name: PlayerInputActions
   - Namespace: MutantMinion.Player (optional)
   - Output Path: Assets/Scripts/Player
7. Replace `Assets/Scripts/Player/PlayerInputActions.cs` with the generated class

### Step 3: Create Player GameObject
1. Create Empty GameObject, name it "Player"
2. Add Tag "Player" to it
3. Add these components:
   - Character Controller
   - PlayerController (will auto-add required components)
   - Capsule Collider (for combat detection)

4. Configure Character Controller:
   - Height: 2
   - Radius: 0.5
   - Center: (0, 1, 0)

### Step 4: Create Camera
1. Create Empty GameObject, name it "PlayerCamera"
2. Add Camera component
3. Add PlayerCamera script
4. Assign Player GameObject to "Target" field
5. Configure camera layers for URP

### Step 5: Configure Layers
Create these layers in Tags & Layers:
- **Ground** (Layer 6)
- **Enemy** (Layer 7)
- **Interactable** (Layer 8)

### Step 6: Set Up Player Components
In Player GameObject, configure:

**PlayerMovement:**
- Walk Speed: 5
- Sprint Speed: 8
- Jump Height: 2
- Ground Mask: Select "Ground" layer
- GroundCheck will auto-create at the correct position (bottom of CharacterController)

**PlayerCombat:**
- Light Attack Damage: 10
- Attack Range: 2
- Enemy Layer: Select "Enemy" layer
- Create child GameObject "AttackPoint" at position (0, 1, 1)

**PlayerInteraction:**
- Interaction Range: 2
- Interactable Mask: Select "Interactable" layer

**PlayerStamina:**
- Max Stamina: 100
- Sprint Drain Rate: 10
- Regen Rate: 15

### Step 7: Create Test Environment
1. Create Floor:
   - Cube scaled to (20, 0.5, 20)
   - Position at (0, 0, 0)
   - Layer: Ground

2. Create Test Enemy:
   - Cube GameObject
   - Add ExampleEnemy script
   - Layer: Enemy
   - Add Capsule Collider

3. Create Test Interactable:
   - Cube GameObject
   - Add ExampleInteractable script
   - Layer: Interactable
   - Add Box Collider

## 🧬 Creating Mutation Abilities

### Create Wolf Ability (Speed Boost)
1. Right-click in Project
2. Create -> Mutant Minion -> Ability Data
3. Name it "WolfAbility"
4. Configure:
   - Ability Name: "Wolf Speed"
   - Mutation Type: Wolf
   - Stamina Cost: 20
   - Duration: 5
   - Speed Multiplier: 1.5
   - Damage Multiplier: 1.3

### Create Frog Ability (Jump Boost)
1. Create -> Mutant Minion -> Ability Data
2. Name it "FrogAbility"
3. Configure:
   - Ability Name: "Frog Leap"
   - Mutation Type: Frog
   - Stamina Cost: 15
   - Duration: 10
   - Jump Multiplier: 2

### Equip Ability via Script
```csharp
PlayerController player = FindObjectOfType<PlayerController>();
AbilityData wolfAbility = // Load your ability asset
player.Ability.EquipAbility(wolfAbility);
```

## 🎯 Controls

### Keyboard & Mouse (Default)
- **WASD** - Move
- **Mouse** - Look around
- **Space** - Jump
- **Left Shift (Hold)** - Sprint
- **Left Click** - Light Attack
- **Right Click** - Block (reserved)
- **F** - Interact
- **Q** - Use Ability

### Gamepad
- **Left Stick** - Move
- **Right Stick** - Look around
- **A Button** - Jump
- **Left Stick Click** - Sprint
- **X Button** - Light Attack
- **B Button** - Block (reserved)
- **Y Button** - Interact
- **LB** - Use Ability

## 🔧 Extending the System

### Add New Mutation
1. Add new enum in `PlayerAbility.cs`:
```csharp
public enum MutationType
{
    None,
    Wolf,
    Frog,
    Bear  // New mutation
}
```

2. Add handlers in `PlayerAbility.cs`:
```csharp
private void ApplyBearEffects()
{
    // Implement bear-specific effects
}

private void RemoveBearEffects()
{
    // Remove bear-specific effects
}
```

### Add Custom Interactable
Implement `IInteractable` interface:
```csharp
public class MyInteractable : MonoBehaviour, IInteractable
{
    public void Interact(GameObject player) { }
    public bool CanInteract(GameObject player) { return true; }
    public string GetInteractionPrompt() { return "Press F"; }
    public void OnInteractableEnter(GameObject player) { }
    public void OnInteractableExit(GameObject player) { }
}
```

### Add Custom Enemy
Implement `IDamageable` interface:
```csharp
public class MyEnemy : MonoBehaviour, IDamageable
{
    public void TakeDamage(float damage)
    {
        // Handle damage
    }
}
```

## 🎨 Ready for Animations

All scripts are structured to work with Unity's Animator:
- Movement script exposes `IsMoving`, `IsSprinting`, `IsGrounded`
- Combat script has animator trigger points
- Add Animator component and set triggers/parameters

Example Animator Parameters:
- `IsMoving` (Bool)
- `IsSprinting` (Bool)
- `IsGrounded` (Bool)
- `LightAttack` (Trigger)
- `Jump` (Trigger)

## 📝 Notes

- System uses Unity's **New Input System** (fully remappable)
- All scripts use **namespaces** for clean organization
- **Event-driven** architecture for UI/system integration
- **Mutation system** ready for Wolf, Frog, and custom abilities
- Uses **interfaces** for extensibility (IDamageable, IInteractable)
- **Gizmos** included for debugging in Scene view
- Follows Unity's **CharacterController** for movement (no Rigidbody needed)

## 🐛 Common Issues

**"NullReferenceException in PlayerMovement.GetMovementDirection()"**
- Your camera needs to be tagged as **"MainCamera"**
- Select your camera GameObject in the Hierarchy
- In Inspector, set Tag dropdown to "MainCamera"
- The script will now automatically find it
- If you still get errors, ensure you have a Camera in the scene

**"No Keyboard option in Vector2 binding dropdown!"**
- This is EXPECTED behavior in Unity's Input System
- For Vector2 actions, use **"Add 2D Vector Composite"** (NOT "Add Binding")
- Click the **+** button next to the action name
- Select "Add 2D Vector Composite"
- Then assign W/A/S/D to Up/Down/Left/Right sub-bindings
- See Step 2 above for detailed instructions

**Input not working:**
- Make sure New Input System is installed
- Check Input Action Asset is generated with C# class
- Verify Player has InputManager component
- Ensure Input Actions asset has "Auto-Save" enabled
- Check Console for Input System errors

**Player falling through floor:**
- Ensure floor has Ground layer
- Check GroundCheck position (should be at player's feet)
- Verify Ground Mask in PlayerMovement includes Ground layer
- Make sure floor has a collider

**Camera not following:**
- Assign Player to Camera's Target field
- Check camera is NOT a child of Player
- Verify camera script is enabled
- Check Console for missing reference errors

**Combat not hitting enemies:**
- Verify enemies have Enemy layer
- Check AttackPoint position (should be in front of player)
- Ensure enemies have colliders
- Check attack range and radius values

**Move/Look input not generating C# class correctly:**
- Delete the auto-generated class
- In Input Actions asset Inspector, click "Generate C# Class" again
- Make sure namespace matches: MutantMinion.Player
- Replace the placeholder PlayerInputActions.cs with generated file

## 🚀 Next Steps

1. Add Health System
2. Implement Block mechanic
3. Add Animation system
4. Create more mutations
5. Build mutation transformation system
6. Add UI for health/stamina/abilities
7. Implement combo system
8. Add sound effects

---

**Need help?** Check Unity console for debug logs (enabled by default)
