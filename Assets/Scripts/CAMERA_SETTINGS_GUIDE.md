# 🎮 Camera Sensitivity & Settings System Guide

## ✅ What's Changed:

1. **Camera Sensitivity Reduced:** Default changed from 2.0 to 1.0 (less sensitive)
2. **UI-Ready Controls:** Added methods to change sensitivity from UI
3. **Settings Manager:** Created GameSettings system for save/load
4. **Example UI:** Created SettingsMenuUI template

---

## 🎯 Current Camera Settings:

### Default Values:
- **Mouse Sensitivity:** 1.0 (reduced from 2.0)
- **Gamepad Sensitivity:** 150 degrees/second
- **Sensitivity Range:** 0.1 to 5.0

### Adjustable in Inspector:
Select **PlayerCamera** GameObject → PlayerCamera component:
- Mouse Sensitivity (0.1 - 5.0)
- Min/Max vertical angles
- Rotation smooth time

---

## 🔧 Quick Test (Without UI):

### Method 1: Adjust in Inspector (Editor Only)
1. Select PlayerCamera in Hierarchy
2. Find "Mouse Sensitivity" in Inspector
3. Change value (try 0.5 for slower, 2.0 for faster)
4. Changes apply immediately in Play mode

### Method 2: Adjust via Code
```csharp
// In any script:
PlayerCamera camera = FindObjectOfType<PlayerCamera>();

// Set specific value
camera.SetMouseSensitivity(0.5f); // Slow
camera.SetMouseSensitivity(1.0f); // Normal
camera.SetMouseSensitivity(2.0f); // Fast

// Or use normalized value (0-1) for UI sliders
camera.SetNormalizedSensitivity(0.5f); // 50% between min and max
```

---

## 🎨 Setting Up UI Settings Menu (Future):

### Step 1: Create Settings GameObject
1. Create Empty GameObject in scene
2. Name it "GameSettings"
3. Add GameSettings script
4. This persists between scenes (DontDestroyOnLoad)

### Step 2: Create UI Panel
```
Canvas
└─ SettingsPanel
   ├─ Title (Text)
   ├─ SensitivitySlider (Slider)
   │  └─ Label (Text: "Mouse Sensitivity")
   ├─ SensitivityValue (TextMeshProUGUI)
   ├─ ApplyButton (Button)
   ├─ ResetButton (Button)
   └─ CloseButton (Button)
```

### Step 3: Setup Slider
1. Select SensitivitySlider
2. Set Min Value: 0
3. Set Max Value: 1 (normalized)
4. Set Whole Numbers: OFF
5. Set Value: 0.5 (default middle)

### Step 4: Connect UI
1. Add SettingsMenuUI script to SettingsPanel
2. Assign in Inspector:
   - Mouse Sensitivity Slider → SensitivitySlider
   - Sensitivity Value Text → SensitivityValue
   - Apply Button → ApplyButton
   - Reset Button → ResetButton
   - Close Button → CloseButton

### Step 5: Open Menu
```csharp
// From pause menu or settings button:
SettingsMenuUI settingsMenu = FindObjectOfType<SettingsMenuUI>();
settingsMenu.OpenMenu();
```

---

## 💾 Save/Load System:

### How It Works:
Settings are saved to **PlayerPrefs** (persistent across game sessions):
- Mouse Sensitivity → "MouseSensitivity"
- Master Volume → "MasterVolume" (ready for future)
- Music Volume → "MusicVolume" (ready for future)
- SFX Volume → "SFXVolume" (ready for future)

### Manual Save/Load:
```csharp
GameSettings settings = GameSettings.Instance;

// Save current settings
settings.SaveSettings();

// Load saved settings
settings.LoadSettings();

// Reset to defaults
settings.ResetToDefaults();
```

### Auto-Save:
Settings are automatically:
- Loaded on game start
- Saved when "Apply" button is clicked
- Reset when "Reset" button is clicked

---

## 🎮 Testing Camera Sensitivity:

### Recommended Values:
- **0.5** - Very slow (good for precision aiming)
- **1.0** - Normal (new default) ✓
- **1.5** - Faster
- **2.0** - Fast (old default)
- **3.0+** - Very fast

### Test In-Game:
1. Press Play
2. Move mouse around
3. If too fast: Lower sensitivity
4. If too slow: Raise sensitivity

---

## 📋 Quick Reference:

### Camera Properties Available:
```csharp
PlayerCamera camera = FindObjectOfType<PlayerCamera>();

// Get/Set sensitivity
float current = camera.GetMouseSensitivity();
camera.SetMouseSensitivity(1.5f);

// Get/Set normalized (for UI sliders 0-1)
float normalized = camera.GetNormalizedSensitivity();
camera.SetNormalizedSensitivity(0.7f);

// Get min/max range
Vector2 range = camera.GetSensitivityRange(); // (0.1, 5.0)
```

### Settings Manager:
```csharp
GameSettings settings = GameSettings.Instance;

// Camera
settings.SetMouseSensitivity(1.5f);
float sens = settings.MouseSensitivity;

// Audio (Future)
settings.SetMasterVolume(0.8f);
settings.SetMusicVolume(0.5f);
settings.SetSFXVolume(1.0f);

// Save/Load
settings.SaveSettings();
settings.LoadSettings();
settings.ResetToDefaults();
```

---

## 🔮 Future Features Ready:

The system is ready for:
- ✅ Mouse sensitivity control
- ✅ Save/load settings
- ✅ UI integration
- ⏳ Master volume control
- ⏳ Music volume control
- ⏳ SFX volume control
- ⏳ Graphics quality settings
- ⏳ Keybinding remapping
- ⏳ Accessibility options

---

## 🎯 Current Status:

**✅ Working Now:**
- Camera sensitivity reduced to 1.0
- Can be changed in Inspector
- Can be changed via code
- Settings manager ready
- Save/load system ready
- Example UI script ready

**⏭️ Next Steps (When You Want UI):**
1. Create UI Canvas and settings panel
2. Add sliders and buttons
3. Connect SettingsMenuUI script
4. Test and tweak

---

## 📁 New Files Created:

1. **GameSettings.cs** (`Assets/Scripts/Settings/`)
   - Singleton settings manager
   - Save/load to PlayerPrefs
   - Apply settings to game

2. **SettingsMenuUI.cs** (`Assets/Scripts/UI/`)
   - Example UI controller
   - Connects sliders to settings
   - Ready to use with your UI

3. **CAMERA_SETTINGS_GUIDE.md** (This file)
   - Complete guide for camera settings

---

## 🐛 Troubleshooting:

**Camera still too sensitive:**
- Lower Mouse Sensitivity value (try 0.5 or 0.7)
- Check if gamepad input is interfering

**Settings not saving:**
- Ensure GameSettings GameObject exists in scene
- Check Console for save confirmation
- PlayerPrefs location: Registry (Windows) or plist (Mac)

**UI slider not working:**
- Verify slider range is 0-1
- Check SettingsMenuUI script assignments
- Ensure PlayerCamera exists in scene

---

**Camera sensitivity is now fixed and ready for UI integration!** 🎉

Default is now 1.0 instead of 2.0, and you can easily add a settings menu later using the provided scripts.
