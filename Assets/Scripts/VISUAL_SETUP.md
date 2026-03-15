# 🎮 Visual Setup Guide

## Unity Hierarchy Setup

```
Scene
├── Player (Tag: "Player")
│   ├── [Components]
│   │   ├── Character Controller
│   │   ├── PlayerController
│   │   ├── PlayerMovement
│   │   ├── PlayerCamera (handled separately)
│   │   ├── PlayerCombat
│   │   ├── PlayerStamina
│   │   ├── PlayerInteraction
│   │   ├── PlayerAbility
│   │   ├── InputManager
│   │   └── Capsule Collider
│   │
│   ├── GroundCheck (Empty GameObject)
│   │   └── Position: (0, 0, 0) local
│   │
│   ├── AttackPoint (Empty GameObject)
│   │   └── Position: (0, 1, 1) local
│   │
│   └── PlayerModel (Your capsule/model)
│       └── Position: (0, 0, 0) local
│
├── PlayerCamera (Separate GameObject)
│   ├── [Components]
│   │   ├── Camera
│   │   └── PlayerCamera Script
│   │
│   └── Target: Assign Player GameObject
│
├── Environment
│   ├── Floor (Cube)
│   │   ├── Scale: (20, 0.5, 20)
│   │   ├── Layer: Ground
│   │   └── Box Collider
│   │
│   └── Walls, Props, etc.
│       └── Layer: Ground
│
├── Enemies
│   └── TestEnemy (Cube)
│       ├── Layer: Enemy
│       ├── ExampleEnemy Script
│       ├── Capsule Collider
│       └── Renderer (Material)
│
└── Interactables
    └── TestChest (Cube)
        ├── Layer: Interactable
        ├── ExampleInteractable Script
        ├── Box Collider
        └── Renderer (Material)
```

---

## Component Configuration Diagram

### Player GameObject Inspector

```
┌─────────────────────────────────────┐
│ Player                              │
├─────────────────────────────────────┤
│ Tag: Player                         │
│ Layer: Default                      │
├─────────────────────────────────────┤
│ Transform                           │
│ Position: (0, 1, 0)                 │
│ Rotation: (0, 0, 0)                 │
│ Scale: (1, 1, 1)                    │
├─────────────────────────────────────┤
│ Character Controller                │
│ ├─ Height: 2                        │
│ ├─ Radius: 0.5                      │
│ ├─ Center: (0, 1, 0)                │
│ └─ Skin Width: 0.08                 │
├─────────────────────────────────────┤
│ PlayerController                    │
│ ├─ Show Debug Info: ✓               │
│ └─ (Auto-assigns components)        │
├─────────────────────────────────────┤
│ PlayerMovement                      │
│ ├─ Walk Speed: 5                    │
│ ├─ Sprint Speed: 8                  │
│ ├─ Rotation Speed: 10               │
│ ├─ Jump Height: 2                   │
│ ├─ Gravity: -15                     │
│ ├─ Ground Check: [Assign]           │
│ ├─ Ground Distance: 0.4             │
│ └─ Ground Mask: Ground (Layer 6)    │
├─────────────────────────────────────┤
│ PlayerCombat                        │
│ ├─ Light Attack Damage: 10          │
│ ├─ Light Attack Range: 2            │
│ ├─ Light Attack Cooldown: 0.5       │
│ ├─ Enemy Layer: Enemy (Layer 7)     │
│ ├─ Attack Point: [Assign]           │
│ └─ Attack Radius: 1                 │
├─────────────────────────────────────┤
│ PlayerStamina                       │
│ ├─ Max Stamina: 100                 │
│ ├─ Sprint Drain Rate: 10            │
│ └─ Stamina Regen Rate: 15           │
├─────────────────────────────────────┤
│ PlayerInteraction                   │
│ ├─ Interaction Range: 2             │
│ ├─ Interactable Mask: Interactable  │
│ ├─ Detection Radius: 0.5            │
│ └─ Show Debug Info: ✓               │
├─────────────────────────────────────┤
│ PlayerAbility                       │
│ ├─ Current Ability: None            │
│ └─ Ability Cooldown: 5              │
├─────────────────────────────────────┤
│ InputManager                        │
│ └─ Enable Input: ✓                  │
├─────────────────────────────────────┤
│ Capsule Collider                    │
│ ├─ Height: 2                        │
│ ├─ Radius: 0.5                      │
│ └─ Center: (0, 1, 0)                │
└─────────────────────────────────────┘
```

### PlayerCamera Inspector

```
┌─────────────────────────────────────┐
│ PlayerCamera                        │
├─────────────────────────────────────┤
│ Transform                           │
│ Position: (0, 2, -5)                │
│ Rotation: (10, 0, 0)                │
├─────────────────────────────────────┤
│ Camera                              │
│ ├─ Clear Flags: Skybox              │
│ ├─ Culling Mask: Everything         │
│ ├─ Projection: Perspective          │
│ └─ Field of View: 60                │
├─────────────────────────────────────┤
│ PlayerCamera Script                 │
│ ├─ Target: [Player GameObject]      │
│ ├─ Target Offset: (0, 1.5, 0)       │
│ ├─ Distance: 5                      │
│ ├─ Min Distance: 2                  │
│ ├─ Max Distance: 10                 │
│ ├─ Mouse Sensitivity: 2             │
│ ├─ Min Vertical Angle: -40          │
│ ├─ Max Vertical Angle: 70           │
│ ├─ Rotation Smooth Time: 0.1        │
│ ├─ Check Collision: ✓               │
│ ├─ Collision Offset: 0.3            │
│ └─ Collision Mask: Everything       │
└─────────────────────────────────────┘
```

---

## Layer Setup

### Edit → Project Settings → Tags and Layers

```
Layers:
├─ 0: Default
├─ 1: TransparentFX
├─ 2: Ignore Raycast
├─ 3: (empty)
├─ 4: Water
├─ 5: UI
├─ 6: Ground          ← Create this
├─ 7: Enemy           ← Create this
└─ 8: Interactable    ← Create this
```

---

## Input Action Asset Structure

### PlayerInputActions.inputactions - DETAILED SETUP

**IMPORTANT:** For WASD keyboard controls on Vector2 actions, you MUST use "2D Vector Composite"!

### How to Create Move Action (Step-by-Step):

```
1. Create Action: "Move"
   - Action Type: Value
   - Control Type: Vector2

2. Add Keyboard Bindings:
   Click [+] next to "Move" 
   → Select "Add 2D Vector Composite" ← IMPORTANT!

   This creates:
   Move
   └─ 2D Vector [Composite]
       ├─ Up [Binding]     ← Click → Press W → <Keyboard>/w
       ├─ Down [Binding]   ← Click → Press S → <Keyboard>/s
       ├─ Left [Binding]   ← Click → Press A → <Keyboard>/a
       └─ Right [Binding]  ← Click → Press D → <Keyboard>/d

3. Add Gamepad Binding:
   Click [+] next to "Move"
   → Select "Add Binding" → Select <Gamepad>/leftStick
```

### Complete Action Map Structure:

```
Action Maps:
└─ Player
   │
   ├─ Move (Value: Vector2)
   │  ├─ [Composite] 2D Vector ← For Keyboard WASD
   │  │  ├─ Up: <Keyboard>/w
   │  │  ├─ Down: <Keyboard>/s
   │  │  ├─ Left: <Keyboard>/a
   │  │  └─ Right: <Keyboard>/d
   │  └─ [Binding] <Gamepad>/leftStick
   │
   ├─ Look (Value: Vector2)
   │  ├─ [Binding] <Mouse>/delta
   │  └─ [Binding] <Gamepad>/rightStick
   │
   ├─ Jump (Button)
   │  ├─ [Binding] <Keyboard>/space
   │  └─ [Binding] <Gamepad>/buttonSouth
   │
   ├─ Sprint (Button)
   │  ├─ [Binding] <Keyboard>/leftShift
   │  └─ [Binding] <Gamepad>/leftStickPress
   │
   ├─ Attack (Button)
   │  ├─ [Binding] <Mouse>/leftButton
   │  └─ [Binding] <Gamepad>/buttonWest
   │
   ├─ Block (Button)
   │  ├─ [Binding] <Mouse>/rightButton
   │  └─ [Binding] <Gamepad>/buttonEast
   │
   ├─ Interact (Button)
   │  ├─ [Binding] <Keyboard>/f
   │  └─ [Binding] <Gamepad>/buttonNorth
   │
   └─ Ability (Button)
      ├─ [Binding] <Keyboard>/q
      └─ [Binding] <Gamepad>/leftShoulder
```

### Visual: Input Actions Window Should Look Like This:

```
Actions                    Bindings
───────                    ────────
⊙ Move                     ▸ 2D Vector [Composite]
  <No Binding>               • Up: W [Keyboard]
                             • Down: S [Keyboard]
                             • Left: A [Keyboard]
                             • Right: D [Keyboard]
                           ▸ Left Stick [Gamepad]

⊙ Look                     ▸ Delta [Mouse]
  <No Binding>             ▸ Right Stick [Gamepad]

∙ Jump                     ▸ Space [Keyboard]
  Space [Keyboard]         ▸ Button South [Gamepad]
```

**Generate C# Class:**
- Open Input Actions asset
- Click "Generate C# Class" in Inspector
- Namespace: MutantMinion.Player
- Class Name: PlayerInputActions
- Output Path: Assets/Scripts/Player
- Replace the placeholder file

---

## Project Folder Structure

```
Assets/
├─ Scenes/
│  └─ MainScene.unity
│
├─ Scripts/
│  ├─ PlayerController.cs
│  ├─ Player/
│  │  ├─ PlayerMovement.cs
│  │  ├─ PlayerCamera.cs
│  │  ├─ PlayerCombat.cs
│  │  ├─ PlayerStamina.cs
│  │  ├─ PlayerInteraction.cs
│  │  ├─ PlayerAbility.cs
│  │  ├─ InputManager.cs
│  │  └─ PlayerInputActions.cs
│  ├─ Examples/
│  │  ├─ ExampleEnemy.cs
│  │  └─ ExampleInteractable.cs
│  ├─ SETUP_GUIDE.md
│  ├─ ARCHITECTURE.md
│  ├─ QUICK_REFERENCE.md
│  └─ README.md
│
├─ Input/
│  └─ PlayerInputActions.inputactions
│
├─ Data/
│  └─ Abilities/
│     ├─ WolfAbility.asset
│     └─ FrogAbility.asset
│
└─ Materials/
   ├─ PlayerMaterial.mat
   ├─ EnemyMaterial.mat
   └─ InteractableMaterial.mat
```

---

## Scene Setup Checklist

### Step 1: Player Setup
- [ ] Create Empty GameObject "Player"
- [ ] Add Tag "Player"
- [ ] Add Character Controller
- [ ] Add PlayerController script
- [ ] Create child "GroundCheck" at (0, 0, 0)
- [ ] Create child "AttackPoint" at (0, 1, 1)
- [ ] Add visual capsule or model

### Step 2: Camera Setup
- [ ] Create GameObject "PlayerCamera"
- [ ] Add Camera component
- [ ] Add PlayerCamera script
- [ ] Drag Player to Target field
- [ ] Position at (0, 2, -5)

### Step 3: Environment
- [ ] Create Floor (Cube 20×0.5×20)
- [ ] Set Layer to Ground
- [ ] Add collider

### Step 4: Test Objects
- [ ] Create Enemy cube
- [ ] Add ExampleEnemy script
- [ ] Set Layer to Enemy
- [ ] Add collider
- [ ] Create Interactable cube
- [ ] Add ExampleInteractable script
- [ ] Set Layer to Interactable
- [ ] Add collider

### Step 5: Configure Layers
- [ ] Create Ground layer (6)
- [ ] Create Enemy layer (7)
- [ ] Create Interactable layer (8)
- [ ] Assign Ground Mask in PlayerMovement
- [ ] Assign Enemy Layer in PlayerCombat
- [ ] Assign Interactable Mask in PlayerInteraction

### Step 6: Input System
- [ ] Install Input System package
- [ ] Create Input Action Asset
- [ ] Set up actions as shown above
- [ ] Generate C# class
- [ ] Replace PlayerInputActions.cs

### Step 7: Test
- [ ] Press Play
- [ ] Test WASD movement
- [ ] Test mouse look
- [ ] Test sprint (Shift)
- [ ] Test jump (Space)
- [ ] Test attack (Left Click on enemy)
- [ ] Test interact (F near interactable)

---

## Visual Reference

### Player in Scene View
```
     ┌─────┐
     │  📷  │  PlayerCamera (follows)
     └──┬──┘
        │
     ┌──▼──┐
     │  👤  │  Player (Capsule)
     └──┬──┘
        │
    ────┴────  Ground
```

### Attack Detection
```
        ┌──────┐
        │Attack│  AttackPoint
        │ ○ ○  │  Detection Sphere
        └──┬───┘
           │
        ┌──▼──┐
        │  👤  │  Player
        └─────┘
```

### Ground Detection
```
        ┌─────┐
        │  👤  │  Player
        └──┬──┘
           │
        ┌──▼──┐
        │  ○   │  GroundCheck Sphere
        └──┬──┘
    ───────▼─────  Ground (Green if grounded)
```

### Interaction Detection
```
        ┌─────┐
        │  📦  │  Interactable
        └──┬──┘
           │
        ┌──▼──┐  Detection Sphere
        │  👤  │  Player
        └─────┘
         (F to interact)
```

---

## Color Coding

### Gizmo Colors in Scene View
- 🟢 **Green** - Grounded / In Range / Active
- 🔴 **Red** - Not Grounded / Out of Range
- 🟡 **Yellow** - Detection Range / Attack Range
- 🔵 **Blue** - Camera Ray / Interaction Direction

### Material Suggestions
- **Player:** Blue (Hero color)
- **Enemy:** Red (Danger)
- **Interactable:** Yellow (Attention)
- **Ground:** Gray/Brown (Neutral)

---

## Quick Test Scene Setup

### Minimal Scene (5 Minutes)
1. Create Floor (Cube, Layer: Ground)
2. Create Player (Add PlayerController)
3. Create Camera (Add PlayerCamera, assign Player)
4. Create Enemy (Cube, ExampleEnemy, Layer: Enemy)
5. Create Interactable (Cube, ExampleInteractable, Layer: Interactable)
6. Press Play!

### Expected Behavior
- Player moves with WASD
- Camera follows and looks with mouse
- Shift makes player sprint (stamina drains)
- Space makes player jump
- Left-click near enemy damages it
- F near interactable triggers interaction

---

**Visual guide complete!** 🎨
See SETUP_GUIDE.md for detailed text instructions.
