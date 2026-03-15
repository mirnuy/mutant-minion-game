using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MutantMinion.Settings;

namespace MutantMinion.UI
{
    /// <summary>
    /// Example settings menu UI controller.
    /// Connect this to your settings menu UI panel.
    /// </summary>
    public class SettingsMenuUI : MonoBehaviour
    {
        [Header("Camera Settings UI")]
        [SerializeField] private Slider mouseSensitivitySlider;
        [SerializeField] private TextMeshProUGUI sensitivityValueText;
        
        [Header("Audio Settings UI (Future)")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        
        [Header("Buttons")]
        [SerializeField] private Button applyButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button closeButton;
        
        private void Start()
        {
            // Load current settings
            LoadCurrentSettings();
            
            // Setup button listeners
            if (applyButton != null)
                applyButton.onClick.AddListener(OnApplyClicked);
            
            if (resetButton != null)
                resetButton.onClick.AddListener(OnResetClicked);
            
            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseClicked);
            
            // Setup slider listeners
            if (mouseSensitivitySlider != null)
            {
                mouseSensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            }
            
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        
        /// <summary>
        /// Load current settings into UI
        /// </summary>
        private void LoadCurrentSettings()
        {
            GameSettings settings = GameSettings.Instance;
            
            // Set slider values
            if (mouseSensitivitySlider != null)
            {
                // Get normalized sensitivity (0-1) for slider
                Player.PlayerCamera camera = FindObjectOfType<Player.PlayerCamera>();
                if (camera != null)
                {
                    mouseSensitivitySlider.value = camera.GetNormalizedSensitivity();
                    UpdateSensitivityText(camera.GetMouseSensitivity());
                }
            }
            
            if (masterVolumeSlider != null)
                masterVolumeSlider.value = settings.MasterVolume;
            
            if (musicVolumeSlider != null)
                musicVolumeSlider.value = settings.MusicVolume;
            
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.value = settings.SFXVolume;
        }
        
        /// <summary>
        /// Called when sensitivity slider changes
        /// </summary>
        private void OnSensitivityChanged(float normalizedValue)
        {
            Player.PlayerCamera camera = FindObjectOfType<Player.PlayerCamera>();
            if (camera != null)
            {
                camera.SetNormalizedSensitivity(normalizedValue);
                UpdateSensitivityText(camera.GetMouseSensitivity());
            }
        }
        
        /// <summary>
        /// Update sensitivity display text
        /// </summary>
        private void UpdateSensitivityText(float sensitivity)
        {
            if (sensitivityValueText != null)
            {
                sensitivityValueText.text = $"{sensitivity:F2}";
            }
        }
        
        private void OnMasterVolumeChanged(float value)
        {
            GameSettings.Instance.SetMasterVolume(value);
        }
        
        private void OnMusicVolumeChanged(float value)
        {
            GameSettings.Instance.SetMusicVolume(value);
        }
        
        private void OnSFXVolumeChanged(float value)
        {
            GameSettings.Instance.SetSFXVolume(value);
        }
        
        /// <summary>
        /// Apply and save settings
        /// </summary>
        private void OnApplyClicked()
        {
            // Get current camera sensitivity and save it
            Player.PlayerCamera camera = FindObjectOfType<Player.PlayerCamera>();
            if (camera != null)
            {
                GameSettings.Instance.SetMouseSensitivity(camera.GetMouseSensitivity());
            }
            
            GameSettings.Instance.SaveSettings();
            
            Debug.Log("Settings applied and saved!");
        }
        
        /// <summary>
        /// Reset all settings to defaults
        /// </summary>
        private void OnResetClicked()
        {
            GameSettings.Instance.ResetToDefaults();
            LoadCurrentSettings();
        }
        
        /// <summary>
        /// Close settings menu
        /// </summary>
        private void OnCloseClicked()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Open settings menu
        /// </summary>
        public void OpenMenu()
        {
            gameObject.SetActive(true);
            LoadCurrentSettings();
        }
    }
}
