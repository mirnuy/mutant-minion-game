// GameManager.cs
// Lightweight game-state manager for the MVP prototype.
// Tracks the current game phase and provides helper methods for
// starting / ending raids and returning to the lab hub.
//
// Designed to grow: add inventory management, session stats,
// and multiplayer session handling here as the game expands.

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MutantMinion.Managers
{
    public enum GameState
    {
        MainMenu,
        Laboratory,   // safe hub – player manages minions
        Raid,         // active extraction session
        Extracting,   // player reached extraction point
        GameOver      // minion died in raid
    }

    public class GameManager : MonoBehaviour
    {
        // ─── Singleton ───────────────────────────────────────────────────────────
        public static GameManager Instance { get; private set; }

        // ─── State ───────────────────────────────────────────────────────────────
        public GameState CurrentState { get; private set; } = GameState.Raid;

        // ─── Unity lifecycle ─────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Debug.Log($"[GameManager] Game started. State: {CurrentState}");
        }

        // ─── Public API ──────────────────────────────────────────────────────────

        /// <summary>Begin a raid session (load the raid scene).</summary>
        public void StartRaid(string raidSceneName = "MainPrototype")
        {
            SetState(GameState.Raid);
            SceneManager.LoadScene(raidSceneName);
        }

        /// <summary>Player reached an extraction point.</summary>
        public void BeginExtraction()
        {
            if (CurrentState != GameState.Raid) return;
            SetState(GameState.Extracting);
            Debug.Log("[GameManager] Extraction started! Get to the extraction zone...");
            // TODO: Start extraction timer, lock combat, etc.
        }

        /// <summary>Minion successfully extracted.</summary>
        public void CompleteExtraction()
        {
            SetState(GameState.Laboratory);
            Debug.Log("[GameManager] Extraction successful! Returning to lab.");
            // TODO: Save loot, load laboratory scene.
        }

        /// <summary>Minion died in raid – permanent loss.</summary>
        public void MinionDied()
        {
            SetState(GameState.GameOver);
            Debug.Log("[GameManager] Minion lost. Game Over.");
            // TODO: Show death screen, clear minion data.
        }

        // ─── Private ─────────────────────────────────────────────────────────────
        private void SetState(GameState newState)
        {
            Debug.Log($"[GameManager] State: {CurrentState} → {newState}");
            CurrentState = newState;
        }
    }
}
