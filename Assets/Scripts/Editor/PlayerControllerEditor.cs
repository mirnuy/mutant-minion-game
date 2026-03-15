using UnityEngine;
using UnityEditor;
using MutantMinion.Player;

/// <summary>
/// Custom Inspector for PlayerController with auto-assign button
/// </summary>
[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();
        
        PlayerController controller = (PlayerController)target;
        
        // Add some space
        EditorGUILayout.Space(10);
        
        // Big helpful button
        if (GUILayout.Button("🔧 Auto-Assign All Components", GUILayout.Height(30)))
        {
            Undo.RecordObject(controller, "Auto-Assign Components");
            
            // Use SerializedObject to assign private fields
            SerializedObject so = new SerializedObject(controller);
            
            so.FindProperty("movement").objectReferenceValue = controller.GetComponent<PlayerMovement>();
            so.FindProperty("stamina").objectReferenceValue = controller.GetComponent<PlayerStamina>();
            so.FindProperty("combat").objectReferenceValue = controller.GetComponent<PlayerCombat>();
            so.FindProperty("interaction").objectReferenceValue = controller.GetComponent<PlayerInteraction>();
            so.FindProperty("ability").objectReferenceValue = controller.GetComponent<PlayerAbility>();
            so.FindProperty("inputManager").objectReferenceValue = controller.GetComponent<InputManager>();
            
            so.ApplyModifiedProperties();
            
            Debug.Log("✅ PlayerController: All components auto-assigned!");
        }
        
        // Add helpful info box
        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox(
            "Click the button above to automatically assign all player components. " +
            "Components will also auto-assign at runtime if left empty.",
            MessageType.Info
        );
    }
}
