using UnityEngine;
using UnityEditor;

public class DialogNode : MonoBehaviour
{
    public string dialogue;
    public DialogNode[] nextNodes;
}

[CustomEditor(typeof(DialogNode))]
public class DialogNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DialogNode dialogNode = (DialogNode)target;

        EditorGUILayout.LabelField("Dialogue:");
        dialogNode.dialogue = EditorGUILayout.TextArea(dialogNode.dialogue);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Next Nodes:");
        if (dialogNode.nextNodes != null)
        {
            for (int i = 0; i < dialogNode.nextNodes.Length; i++)
            {
                dialogNode.nextNodes[i] = (DialogNode)EditorGUILayout.ObjectField("Node " + i, dialogNode.nextNodes[i], typeof(DialogNode), true);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
