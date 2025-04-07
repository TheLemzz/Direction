using UnityEditor;
using UnityEngine;

/// <summary>
/// Attribute that hide property if choosen boolean value is false.
/// </summary>
public sealed class ConditionalBoolFieldAttribute : PropertyAttribute
{
    public string ConditionalSourceField;
    public ComparisonType Comparison;
    public DataType Data;

    public ConditionalBoolFieldAttribute(string conditionalSourceField, ComparisonType comparisonType = ComparisonType.Equal, DataType dataType = DataType.Int)
    {
        ConditionalSourceField = conditionalSourceField;
        Comparison = comparisonType;
        Data = dataType;
    }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ConditionalBoolFieldAttribute))]
public sealed class ConditionalBoolFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalBoolFieldAttribute cond = attribute as ConditionalBoolFieldAttribute;
        bool enabled = CheckForEqual(cond, property);
        if (enabled) EditorGUI.PropertyField(position, property, label, true);
    }

    private bool CheckForEqual(ConditionalBoolFieldAttribute cond, SerializedProperty property)
    {
        return GetConditionalFieldValue(cond.ConditionalSourceField, property.serializedObject);
    }

    private bool GetConditionalFieldValue(string fieldName, SerializedObject serializedObject)
    {
        SerializedProperty sourcePropertyValue = serializedObject.FindProperty(fieldName);
        if (sourcePropertyValue != null) return sourcePropertyValue.boolValue;

        Debug.LogError("ConditionalBoolFieldAttribute: Conditional source field not found");
        return false;
    }

    private bool CheckIntCondition(int value, int compareValue, ComparisonType comparisonType)
    {
        switch (comparisonType)
        {
            case ComparisonType.Equal:
                return value == compareValue;
            case ComparisonType.GreaterThan:
                return value > compareValue;
            case ComparisonType.LessThan:
                return value < compareValue;
            default:
                return false;
        }
    }
}
#endif

public enum ComparisonType
{
    Equal,
    GreaterThan,
    LessThan
}

public enum DataType
{
    Int,
    Double,
    Float
}
