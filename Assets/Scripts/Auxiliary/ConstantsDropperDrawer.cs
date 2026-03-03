#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConstantsDropperAttribute))]
public sealed class ConstantsDropperDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        ConstantsDropperAttribute constantsDropper = (ConstantsDropperAttribute)attribute;
        Type constantsType = constantsDropper.ConstantsType;

        List<FieldInfo> fields = constantsType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string)).ToList();

        List<string> options = fields.Select(f => f.GetValue(null).ToString()).ToList();
        options.Insert(0, "Select ID...");

        string currentValue = property.stringValue;
        int selectedIndex = options.IndexOf(currentValue);
        if (selectedIndex == -1) selectedIndex = 0;

        selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, options.ToArray());

        if (selectedIndex > 0) property.stringValue = options[selectedIndex];
    }
}
#endif