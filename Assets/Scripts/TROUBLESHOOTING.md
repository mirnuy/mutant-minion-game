# 🔧 Runtime Error Fixes

## InvalidOperationException: Input System Conflict

### Error Message:
```
InvalidOperationException: You are trying to read Input using the UnityEngine.Input class, 
but you have switched active Input handling to Input System package in Player Settings.
```

### Cause:
Mixing old Input System (`Input.GetKeyDown`) with New Input System.

### ✅ Solution:

**This is now fixed!** The character controller uses only the New Input System.

If you see this error in custom scripts:
1. **Don't use:** `Input.GetKeyDown()`, `Input.GetAxis()`, etc.
2. **Instead use:** Unity's New Input System or the PlayerController's input events

---

## NullReferenceException in PlayerMovement

### Error Message:
```
NullReferenceException: Object reference not set to an instance of an object
MutantMinion.Player.PlayerMovement.GetMovementDirection ()
```

### Cause:
The camera is not tagged as "MainCamera" or doesn't exist in the scene.

### ✅ Solution:

**Option 1: Tag Your Camera (Recommended)**
1. Select your camera GameObject in Hierarchy
2. In Inspector, find the "Tag" dropdown (top of Inspector)
3. Select **"MainCamera"**
4. Press Play - Error should be fixed!

**Option 2: Ensure Camera Exists**
1. Check Hierarchy for a Camera object
2. If missing, create one: GameObject → Camera
3. Tag it as "MainCamera"
4. Add the PlayerCamera script to it
5. Assign the Player GameObject to the Target field

### How the Fix Works:
The updated PlayerMovement script now:
- ✅ Automatically finds the camera at Start()
- ✅ Tries to find camera again if it becomes null
- ✅ Provides a fallback to world-space movement if no camera found
- ✅ Shows helpful error messages in console

---

## InputManager Errors (55+ errors)

### Error Message:
```
CS0246: The type or namespace name 'PlayerInputActions' could not be found
```

### Cause:
The Unity Input Actions C# class hasn't been generated yet.

### ✅ Solution:

1. **Open Unity Editor**
2. **Find** `Assets/PlayerInputActions.inputactions` in Project window
3. **Click** on it to select
4. **In Inspector**, click **"Generate C# Class"**
5. **Set**:
   - C# Class File: `Assets/Scripts/Player/PlayerInputActions.cs`
   - C# Class Name: `PlayerInputActions`
   - C# Class Namespace: `MutantMinion.Player`
6. **Click "Apply"**
7. Unity will generate the file and all errors will disappear!

**Alternative:**
- Right-click `PlayerInputActions.inputactions` → Generate C# Class
- Or double-click it → In Input Actions window → Generate C# Class button

---

## Jump Not Working

### Symptoms:
- Press Space but player doesn't jump
- Player stays on ground
- No errors in console

### ✅ **FIXED!** This was a bug in the initial code

The velocity (jump/gravity) wasn't being applied to the CharacterController. This is now fixed!

### If Jump Still Doesn't Work:

**Step 1: Enable Debug Mode**
1. Select **Player** GameObject
2. Find **PlayerMovement** component
3. Check **"Show Jump Debug"** checkbox
4. Press Play and press Space
5. Check Console

**Step 2: Check Console Messages**

You'll see one of these:

**✓ "Jump executed! New velocity.y: X"**
- Jump is working! ✅
- If player still doesn't move up, check Step 3

**✗ "Jump failed - Not grounded!"**
- Ground detection issue (see below)

**Nothing appears**
- Input not reaching the script
- Regenerate Input Actions C# class (see INPUT_SYSTEM_FIX.md)

**Step 3: Verify Ground Detection**

**Issue: Floor not on Ground layer**
1. Select **Floor** GameObject
2. Set Layer to **"Ground"** (Layer 6)
3. Ensure floor has a Collider component

**Issue: Ground Mask not set**
1. Select **Player** GameObject  
2. **PlayerMovement** component
3. **Ground Mask** → Check "Ground" layer

**Issue: Ground layer doesn't exist**
1. Edit → Project Settings → Tags and Layers
2. Set Layer 6 to "Ground"
3. Apply to floor

**Step 4: Visual Debug in Scene View**

1. Press Play
2. Select Player in Hierarchy
3. Look in Scene View (not Game view)
4. You should see a **sphere at player's feet**:
   - **Green sphere** = Grounded ✅ Jump should work
   - **Red sphere** = Not grounded ❌ Jump won't work

If sphere is red when standing on floor:
- Floor not on Ground layer
- Ground Mask not including Ground layer
- GroundCheck position wrong

---

## Player Falls Through Floor

### Symptoms:
- Player drops infinitely downward
- Ground check shows red in Scene view

### ✅ Solutions:

**Fix 1: Set Floor Layer**
1. Select your floor GameObject
2. Set Layer to "Ground" (Layer 6)
3. Ensure floor has a Collider component

**Fix 2: Configure Ground Mask**
1. Select Player GameObject
2. Find PlayerMovement component
3. Set Ground Mask to include "Ground" layer
4. Check that GroundCheck object exists as child

**Fix 3: Check CharacterController**
1. Ensure CharacterController component is on Player
2. Set Skin Width to 0.08 (not 0)
3. Verify Height and Radius match your player size

---

## Camera Not Following Player

### Symptoms:
- Camera stays in one position
- Camera doesn't rotate with mouse

### ✅ Solutions:

**Fix 1: Assign Target**
1. Select PlayerCamera GameObject
2. In PlayerCamera component, find "Target" field
3. Drag Player GameObject into this field

**Fix 2: Check Camera Setup**
1. Ensure camera is **NOT** a child of Player
2. Camera should be separate GameObject
3. Has PlayerCamera script attached
4. Script is enabled (checkbox checked)

**Fix 3: Lock Cursor**
- Press Play
- Click in Game view to lock cursor
- Mouse should now control camera

---

## Combat Not Hitting Enemies

### Symptoms:
- Click attack but enemy takes no damage
- Attack animation plays but nothing happens

### ✅ Solutions:

**Fix 1: Enemy Layer**
1. Select enemy GameObject
2. Set Layer to "Enemy" (Layer 7)
3. Enemy must have a Collider

**Fix 2: Attack Settings**
1. Select Player GameObject
2. Find PlayerCombat component
3. Set Enemy Layer to "Enemy"
4. Check AttackPoint exists as child at (0, 1, 1)

**Fix 3: IDamageable Interface**
```csharp
// Enemy script must implement:
public class MyEnemy : MonoBehaviour, IDamageable
{
    public void TakeDamage(float damage)
    {
        // Handle damage
    }
}
```

---

## No Input Response

### Symptoms:
- WASD doesn't move player
- Mouse doesn't rotate camera
- No buttons work

### ✅ Solutions:

**Fix 1: Generate Input Actions**
- See "InputManager Errors" section above
- Must generate C# class from .inputactions file

**Fix 2: Install Input System**
1. Window → Package Manager
2. Search "Input System"
3. Install
4. Restart Unity when prompted

**Fix 3: Check InputManager**
1. Select Player GameObject
2. Verify InputManager component exists
3. Enable Input checkbox is checked
4. No errors in Console

**Fix 4: Cursor Lock**
- Click in Game view to lock cursor
- This enables mouse input

---

## Stamina Not Regenerating

### Symptoms:
- Stamina drains but never refills
- Can't sprint after using it once

### ✅ Solutions:

**Check Settings:**
1. Select Player GameObject
2. Find PlayerStamina component
3. Verify:
   - Stamina Regen Rate > 0 (try 15)
   - Regen Delay After Use = 1 second
4. Watch Console for stamina change logs

**Debug:**
- Enable debug logging in PlayerController
- Should see "Stamina: X/100" messages

---

## Ability Not Activating

### Symptoms:
- Press Q but nothing happens
- No stamina cost deducted

### ✅ Solutions:

**Fix 1: Equip Ability**
```csharp
// Abilities must be equipped first:
PlayerController player = FindObjectOfType<PlayerController>();
AbilityData ability = // Load your ability ScriptableObject
player.Ability.EquipAbility(ability);
```

**Fix 2: Check Stamina**
- Ensure you have enough stamina for ability cost
- Check ability's stamina cost in ScriptableObject

**Fix 3: Cooldown**
- Ability may still be on cooldown
- Wait for cooldown to finish

---

## Common Unity Issues

### "The type or namespace name 'UnityEngine' could not be found"
- Close Unity
- Delete Library folder
- Reopen Unity (will regenerate)

### "Assembly has duplicate references"
- Window → Package Manager → Input System → Remove
- Reinstall Input System package

### Scripts not compiling after changes
- Edit → Preferences → External Tools
- Click "Regenerate project files"

---

## Quick Diagnostics Checklist

Before running the game, verify:

**Scene Setup:**
- [ ] Player GameObject exists with Tag "Player"
- [ ] Camera exists with Tag "MainCamera"
- [ ] Floor exists with Layer "Ground"
- [ ] PlayerCamera has Player assigned to Target

**Player Components:**
- [ ] Character Controller
- [ ] PlayerController
- [ ] PlayerMovement (Ground Mask set)
- [ ] PlayerCombat (Enemy Layer set)
- [ ] PlayerStamina
- [ ] PlayerInteraction (Interactable Mask set)
- [ ] PlayerAbility
- [ ] InputManager

**Input System:**
- [ ] Input System package installed
- [ ] PlayerInputActions.inputactions exists
- [ ] C# class generated in Assets/Scripts/Player/
- [ ] No compilation errors

**Layers:**
- [ ] Ground (Layer 6) created
- [ ] Enemy (Layer 7) created
- [ ] Interactable (Layer 8) created

---

## Still Having Issues?

1. **Check Console:** Look for error messages
2. **Enable Debug Logs:** Set `showDebugInfo = true` in PlayerController
3. **Verify Gizmos:** In Scene view, you should see:
   - Green/Red sphere at GroundCheck
   - Yellow sphere at AttackPoint
   - Yellow/Green interaction range
4. **Test Step-by-Step:** Test each system individually
5. **Rebuild:** Sometimes Unity needs Assets → Reimport All

---

**Most Common Issue:** Forgetting to tag the Camera as "MainCamera" ← Fixed!

**Second Most Common:** Not generating the Input Actions C# class ← Do this first!

**Third Most Common:** Floor not set to Ground layer ← Easy to forget!
