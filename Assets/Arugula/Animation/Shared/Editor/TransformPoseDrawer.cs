#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Arugula.Animation;

[CustomPropertyDrawer(typeof(TransformPose))]
public class TransformPoseDrawer : PropertyDrawer
{
    static TransformPose copyBuffer;
    static bool hasCopied = false;

    static void Copy(object obj)
    {
        SerializedProperty property = obj as SerializedProperty;
        hasCopied = true;

        SerializedProperty position = property.FindPropertyRelative("position");
        SerializedProperty rotation = property.FindPropertyRelative("rotation");
        SerializedProperty scale = property.FindPropertyRelative("scale");

        copyBuffer.position = position.vector3Value;
        copyBuffer.rotation = rotation.quaternionValue;
        copyBuffer.scale = scale.vector3Value;
    }

    static void Paste(object obj)
    {
        SerializedProperty property = obj as SerializedProperty;

        SerializedProperty position = property.FindPropertyRelative("position");
        SerializedProperty rotation = property.FindPropertyRelative("rotation");
        SerializedProperty scale = property.FindPropertyRelative("scale");

        position.vector3Value = copyBuffer.position;
        rotation.quaternionValue = copyBuffer.rotation;
        scale.vector3Value = copyBuffer.scale;

        property.serializedObject.ApplyModifiedProperties();
    }

    static void ResetPose(object obj)
    {
        SerializedProperty property = obj as SerializedProperty;

        SerializedProperty position = property.FindPropertyRelative("position");
        SerializedProperty rotation = property.FindPropertyRelative("rotation");
        SerializedProperty scale = property.FindPropertyRelative("scale");

        position.vector3Value = Vector3.zero;
        rotation.quaternionValue = Quaternion.identity;
        scale.vector3Value = Vector3.zero;

        property.serializedObject.ApplyModifiedProperties();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, true);

        Rect contextRect = position;
        contextRect.height = EditorGUIUtility.singleLineHeight;
        Event ev = Event.current;
        if (ev.type == EventType.MouseUp && ev.button > 0)
        {
            if (contextRect.Contains(ev.mousePosition))
            {
                if (property.serializedObject.targetObject is Component)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Copy"), false, Copy, property);
                    if (hasCopied)
                        menu.AddItem(new GUIContent("Paste"), false, Paste, property);
                    else
                        menu.AddDisabledItem(new GUIContent("Paste"));
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Reset"), false, ResetPose, property);
                    menu.ShowAsContext();
                    ev.Use();
                }
            }
        }
        else if (ev.type == EventType.DragUpdated && ev.button == 0)
        {
            if (contextRect.Contains(ev.mousePosition))
            {
                if (DragAndDrop.objectReferences.Length == 1)
                {
                    Object obj = DragAndDrop.objectReferences[0];
                    Transform t = null;
                    if (obj is GameObject)
                        t = ((GameObject)obj).transform;
                    else if (obj is Component)
                        t = ((Component)obj).transform;

                    if (t != null)
                    {
                        DragAndDrop.AcceptDrag();
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        ev.Use();
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }
                }
            }
        }
        else if (ev.type == EventType.DragPerform && ev.button == 0)
        {
            if (contextRect.Contains(ev.mousePosition))
            {
                if (DragAndDrop.objectReferences.Length == 1)
                {
                    Object obj = DragAndDrop.objectReferences[0];
                    Transform t = null;
                    if (obj is GameObject)
                        t = ((GameObject)obj).transform;
                    else if (obj is Component)
                        t = ((Component)obj).transform;

                    if (t != null)
                    {
                        DragAndDrop.AcceptDrag();
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        ev.Use();

                        SerializedProperty pos = property.FindPropertyRelative("position");
                        SerializedProperty rotation = property.FindPropertyRelative("rotation");
                        SerializedProperty scale = property.FindPropertyRelative("scale");

                        pos.vector3Value = t.localPosition;
                        rotation.quaternionValue = t.localRotation;
                        scale.vector3Value = t.localScale;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }
                }
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property);
    }
}
#endif