using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplineDecorator))]
public class SplineDecoratorInspector : Editor
{
    private SplineDecorator decorator;
    private bool populated;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        decorator = target as SplineDecorator;
        
        if(GUILayout.Button("Create Decoration") && populated == false)
        {
            populated = true;
            Undo.RecordObject(decorator, "Create Decoration");
            decorator.CreateDecoration();
            Object[] objects = decorator.decorations;
            for (int i = 0; i < objects.Length; i++)
            {
                Undo.RegisterCreatedObjectUndo(objects[i], "Create Object");
            }
            EditorUtility.SetDirty(decorator);
        }

        if(GUILayout.Button("Delete All") && populated == true)
        {
            populated = false;
            Object[] objects = decorator.decorations;
            for(int i = 0; i < objects.Length; i++)
            {
                Undo.DestroyObjectImmediate(objects[i]);
            }
            decorator.DeleteDecorations();
            EditorUtility.SetDirty(decorator);
        }
    }
}
