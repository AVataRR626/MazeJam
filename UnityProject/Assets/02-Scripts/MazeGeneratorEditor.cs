using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MazeGenerator))]
class MazeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MazeGenerator myScript = (MazeGenerator)target;
        if (GUILayout.Button("Generate Maze"))
        {
            myScript.GenerateMaze();
        }
    }
}