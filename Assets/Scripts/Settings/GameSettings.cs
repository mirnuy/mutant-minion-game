using UnityEngine;

namespace MutantMinion.Settings
{
    /// <summary>
    /// Manages game settings including camera sensitivity.
    /// Provides save/load functionality and easy integration with UI.
    /// </summary>
    public class GameSettings : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private float defaultMouseSensitivity = 1f;
        
        [Header("Audio Settings (Future)")]
        [SerializeField] private float masterVolume = 1f;
        [SerializeField] private float musicVolume = 0.7f;
        [SerializeField] private float sfxVolume = 1f;
        
        // Singleton instance
        private static GameSettings instance;
        public static GameSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameSettings>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GameSettings");
                        instance = go.AddComponent<GameSettings>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        // Current settings
        public float MouseSensitivity { get; private set; }
        public float MasterVolume { get; private set; }
        public float MusicVolume { get; private set; }
        public float SFXVolume { get; private set; }
        
        private void Awake()
        {
            // Ensure only one instance exists
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSettings();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Load settings from PlayerPrefs
        /// </summary>
        public void LoadSettings()
        {
            MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultMouseSensitivity);
            MasterVolume = PlayerPrefs.GetFloat("MasterVolume", masterVolume);
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume", musicVolume);
            SFXVolume = PlayerPrefs.GetFloat("SFXVolume", sfxVolume);
            
            ApplySettings();
        }
        
        /// <summary>
        /// Save settings to PlayerPrefs
        /// </summary>
        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("MouseSensitivity", MouseSensitivity);
            PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
            PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
            PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
            PlayerPrefs.Save();
            
            Debug.Log("Settings saved!");
        }
        
        /// <summary>
        /// Apply settings to the game (camera, audio, etc.)
        /// </summary>
        public void ApplySettings()
        {
            // Apply camera sensitivity
            Player.PlayerCamera playerCamera = FindObjectOfType<Player.PlayerCamera>();
            if (playerCamera != null)
            {
                playerCamera.SetMouseSensitivity(MouseSensitivity);
            }
            
            // Future: Apply audio settings
            // AudioListener.volume = MasterVolume;
        }
        
        /// <summary>
        /// Set mouse sensitivity and apply immediately
        /// </summary>
        public void SetMouseSensitivity(float sensitivity)
        {
            MouseSensitivity = sensitivity;
            
            // Apply to camera immediately
            Player.PlayerCamera playerCamera = FindObjectOfType<Player.PlayerCamera>();
            if (playerCamera != null)
            {
                playerCamera.SetMouseSensitivity(MouseSensitivity);
            }
        }
        
        /// <summary>
        /// Set master volume
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            MasterVolume = Mathf.Clamp01(volume);
            // Future: Apply to audio system
        }
        
        /// <summary>
        /// Set music volume
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            MusicVolume = Mathf.Clamp01(volume);
            // Future: Apply to music mixer
        }
        
        /// <summary>
        /// Set SFX volume
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            SFXVolume = Mathf.Clamp01(volume);
            // Future: Apply to SFX mixer
        }
        
        /// <summary>
        /// Reset all settings to default values
        /// </summary>
        public void ResetToDefaults()
        {
            MouseSensitivity = defaultMouseSensitivity;
            MasterVolume = 1f;
            MusicVolume = 0.7f;
            SFXVolume = 1f;
            
            SaveSettings();
            ApplySettings();
            
            Debug.Log("Settings reset to defaults!");
        }
    }
}
