# 🎮 Input System Setup - Common Issue Solution

## ❌ PROBLEM: "I don't see Keyboard in the Vector2 binding dropdown!"

When you try to add a binding to a **Vector2** action (like Move or Look), you'll notice:

```
Usages dropdown shows:
  - Gamepad
  - Joystick
  - Mouse
  - Pen
  - Pointer
  - Sensor
  - Touchscreen
  - Other

❌ NO KEYBOARD OPTION!
```

## ✅ SOLUTION: Use "2D Vector Composite" Instead

### This is NOT a bug - it's how Unity's Input System works!

**Why?**
- A single keyboard key = 1 button (on/off)
- A Vector2 needs 2 values (X and Y)
- You need 4 keys (W/A/S/D) to make a Vector2
- Unity combines them using a **"Composite Binding"**

---

## 📋 Step-by-Step Solution for Move Action

### Step 1: Create the Move Action
```
In Input Actions window:
1. Click "+" to add new action
2. Name it "Move"
3. Action Type: Value
4. Control Type: Vector2
```

### Step 2: Add 2D Vector Composite (for WASD)

**DON'T click on the binding dropdown!**

Instead:
```
1. Find the Move action in the Actions list
2. Click the [+] button to the RIGHT of "Move" 
3. From the menu, select:
   "Add 2D Vector Composite"
```

You should now see:
```
Actions                  Bindings
───────                  ────────
⊙ Move                   ▸ 2D Vector [Composite]
                           • Up [Binding]
                           • Down [Binding]
                           • Left [Binding]
                           • Right [Binding]
```

### Step 3: Assign Keys to Each Direction

Now you can assign individual keys:

```
1. Click on "Up [Binding]"
   → Press W key
   → Shows: W [Keyboard]

2. Click on "Down [Binding]"
   → Press S key
   → Shows: S [Keyboard]

3. Click on "Left [Binding]"
   → Press A key
   → Shows: A [Keyboard]

4. Click on "Right [Binding]"
   → Press D key
   → Shows: D [Keyboard]
```

Final result:
```
⊙ Move
  ▸ 2D Vector [Composite]
    • Up: W [Keyboard]
    • Down: S [Keyboard]
    • Left: A [Keyboard]
    • Right: D [Keyboard]
```

### Step 4: Add Gamepad Binding (Optional)

For gamepad support:
```
1. Click [+] next to "Move" again
2. This time select "Add Binding"
3. In the Path dropdown, select:
   Gamepad → Left Stick
```

Now you have both:
```
⊙ Move
  ▸ 2D Vector [Composite]      ← Keyboard WASD
    • Up: W [Keyboard]
    • Down: S [Keyboard]
    • Left: A [Keyboard]
    • Right: D [Keyboard]
  ▸ Left Stick [Gamepad]        ← Gamepad stick
```

---

## 🎯 Complete Setup for All Actions

### Move (Vector2) - Needs Composite!
```
Click [+] → "Add 2D Vector Composite"
  Up: W
  Down: S
  Left: A
  Right: D
Click [+] → "Add Binding" → <Gamepad>/leftStick
```

### Look (Vector2) - Mouse is direct, gamepad needs binding
```
Click [+] → "Add Binding" → <Mouse>/delta
Click [+] → "Add Binding" → <Gamepad>/rightStick
```

### Jump (Button) - Simple binding
```
Click [+] → "Add Binding" → <Keyboard>/space
Click [+] → "Add Binding" → <Gamepad>/buttonSouth
```

### Sprint (Button) - Simple binding
```
Click [+] → "Add Binding" → <Keyboard>/leftShift
Click [+] → "Add Binding" → <Gamepad>/leftStickPress
```

### Attack (Button) - Simple binding
```
Click [+] → "Add Binding" → <Mouse>/leftButton
Click [+] → "Add Binding" → <Gamepad>/buttonWest
```

### Block (Button) - Simple binding
```
Click [+] → "Add Binding" → <Mouse>/rightButton
Click [+] → "Add Binding" → <Gamepad>/buttonEast
```

### Interact (Button) - Simple binding
```
Click [+] → "Add Binding" → <Keyboard>/f
Click [+] → "Add Binding" → <Gamepad>/buttonNorth
```

### Ability (Button) - Simple binding
```
Click [+] → "Add Binding" → <Keyboard>/q
Click [+] → "Add Binding" → <Gamepad>/leftShoulder
```

---

## 🔑 Key Takeaways

| Action Type | Keyboard Setup | Why |
|-------------|----------------|-----|
| **Button** | Add Binding → Select key | 1 key = 1 button ✅ |
| **Value (Float)** | Add Binding → Select key | 1 key = 1 value ✅ |
| **Value (Vector2)** | Add 2D Vector Composite | Need 4 keys for X+Y ⚠️ |
| **Value (Vector3)** | Add 3D Vector Composite | Need 6 keys for X+Y+Z ⚠️ |

---

## ✅ Verification

After setup, your Move action should look like this:

```
Input Actions Window:

Actions Panel:
├─ Player (Action Map)
   ├─ Move (Value, Vector2) ← Your action
   ├─ Look (Value, Vector2)
   ├─ Jump (Button)
   └─ ...

Bindings Panel (when Move is selected):
└─ Move
   ├─ 2D Vector [Composite]
   │  ├─ Up: W [Keyboard]
   │  ├─ Down: S [Keyboard]
   │  ├─ Left: A [Keyboard]
   │  └─ Right: D [Keyboard]
   └─ Left Stick [Gamepad]
```

---

## 🧪 Testing

After generating the C# class and setting up the player:

1. Press **Play** in Unity
2. Press **W** - Player should move forward
3. Press **A/D** - Player should move left/right
4. Press **S** - Player should move backward
5. Press **W+D** - Player should move diagonally

If movement doesn't work:
- Check Console for errors
- Verify Input Actions asset has "Auto-Save" enabled
- Make sure you generated the C# class
- Ensure PlayerController has InputManager component

---

## 📚 Why This Design?

Unity's Input System is designed to be **device-agnostic**:

- **Gamepad stick** naturally outputs Vector2 (X and Y analog values)
- **Keyboard** needs to **simulate** a stick by combining 4 digital buttons
- The **2D Vector Composite** is Unity's way of converting 4 buttons into 1 Vector2

This allows your game code to treat keyboard and gamepad identically - both provide the same Vector2 input!

---

## 🆘 Still Having Issues?

**"I created the composite but it's not working"**
- Make sure to click "Save Asset" in Input Actions window
- Generate C# Class after making changes
- Replace the PlayerInputActions.cs file with generated code
- Restart Unity if necessary

**"The generated class doesn't have Move.Player.Move"**
- Check namespace in generated class
- Verify Action Map is named "Player"
- Verify Action is named "Move"
- Try deleting and regenerating the class

**"Input Actions window is confusing"**
- This is a common Unity complaint - you're not alone!
- The key is: Vector2 = Composite, Button = Binding
- Once set up, you rarely need to touch it again

---

**Problem solved!** 🎉

Now you can set up WASD movement for your 3D character controller!
