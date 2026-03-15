// Constants.cs
// Centralised configuration values for Mutant Minion Game.
// Adjust these values in the Inspector on the relevant components,
// or change the defaults here to affect the whole project at once.

namespace MutantMinion.Utils
{
    public static class Constants
    {
        // ─── Movement ────────────────────────────────────────────────────────────
        public const float DEFAULT_MOVE_SPEED      = 5f;
        public const float DEFAULT_SPRINT_SPEED    = 8f;
        public const float DEFAULT_JUMP_FORCE      = 5f;
        public const float GRAVITY                 = -9.81f;
        public const float GROUND_CHECK_DISTANCE   = 0.2f;
        public const float MOVE_SMOOTH_TIME        = 0.1f;   // acceleration smoothing

        // ─── Camera ──────────────────────────────────────────────────────────────
        public const float DEFAULT_CAMERA_DISTANCE = 5f;
        public const float DEFAULT_CAMERA_HEIGHT   = 2f;
        public const float DEFAULT_MOUSE_SENSITIVITY = 3f;
        public const float CAMERA_SMOOTH_TIME      = 0.05f;
        public const float CAMERA_MIN_PITCH        = -30f;
        public const float CAMERA_MAX_PITCH        = 60f;

        // ─── Stamina ─────────────────────────────────────────────────────────────
        public const float DEFAULT_MAX_STAMINA         = 100f;
        public const float DEFAULT_STAMINA_REGEN_RATE  = 15f;  // per second
        public const float DEFAULT_STAMINA_REGEN_DELAY = 1.5f; // seconds before regen starts
        public const float SPRINT_STAMINA_COST         = 15f;  // per second
        public const float ABILITY_STAMINA_COST        = 25f;

        // ─── Combat ──────────────────────────────────────────────────────────────
        public const float LIGHT_ATTACK_DAMAGE    = 10f;
        public const float LIGHT_ATTACK_RANGE     = 2f;
        public const float LIGHT_ATTACK_COOLDOWN  = 0.5f;

        // ─── Interaction ─────────────────────────────────────────────────────────
        public const float INTERACTION_RADIUS = 2.5f;

        // ─── Ability ─────────────────────────────────────────────────────────────
        public const float DEFAULT_ABILITY_COOLDOWN = 5f;

        // ─── Layers ──────────────────────────────────────────────────────────────
        public const string GROUND_LAYER      = "Ground";
        public const string ENEMY_LAYER       = "Enemy";
        public const string INTERACTABLE_LAYER = "Interactable";

        // ─── Tags ────────────────────────────────────────────────────────────────
        public const string PLAYER_TAG       = "Player";
        public const string ENEMY_TAG        = "Enemy";
        public const string INTERACTABLE_TAG = "Interactable";
    }
}
