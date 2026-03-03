using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InstantiableTypeAttribute))]
public class InstantiableTypeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Базовая высота строки (для кнопки выпадающего списка)
        float height = EditorGUIUtility.singleLineHeight;

        // Если объект создан (не null), добавляем высоту всех его полей
        if (property.managedReferenceValue != null)
        {
            // true означает "включая потомков", это важно для раскрытия списка
            height += EditorGUI.GetPropertyHeight(property, true);
            height += EditorGUIUtility.standardVerticalSpacing; // Немного отступа
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 1. Рисуем прямоугольник для кнопки выбора типа (всегда одна строка)
        Rect dropdownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        // Получаем имя текущего типа или "None"
        string currentName = "None (Select Type)";
        if (property.managedReferenceValue != null)
        {
            // Красивое имя типа (KitchenSaveData вместо Assembly-CSharp.KitchenSaveData)
            currentName = property.managedReferenceValue.GetType().Name;
        }

        // Рисуем заголовок поля слева и кнопку справа
        // EditorGUI.PrefixLabel зарезервирует место под название переменной (_saveDataTemplate)
        Rect buttonRect = EditorGUI.PrefixLabel(dropdownRect, label);

        // 2. Рисуем кнопку
        if (GUI.Button(buttonRect, currentName, EditorStyles.popup))
        {
            DrawTypeSelectionMenu(property);
        }

        // 3. Если объект выбран, рисуем его свойства ниже
        if (property.managedReferenceValue != null)
        {
            // Сдвигаем позицию вниз на одну строку
            Rect contentRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, position.height - EditorGUIUtility.singleLineHeight);

            // Рисуем сам объект (true включает отрисовку детей)
            EditorGUI.PropertyField(contentRect, property, GUIContent.none, true);
        }

        EditorGUI.EndProperty();
    }

    private void DrawTypeSelectionMenu(SerializedProperty property)
    {
        GenericMenu menu = new GenericMenu();

        // Опция "None" для очистки
        menu.AddItem(new GUIContent("None"), property.managedReferenceValue == null, () =>
        {
            property.managedReferenceValue = null;
            property.serializedObject.ApplyModifiedProperties();
        });

        // Получаем тип поля (BaseSaveData) из fieldInfo, который предоставляет PropertyDrawer
        Type baseType = fieldInfo.FieldType;

        // Если это массив или список, fieldInfo даст тип коллекции, нам нужно получить тип элемента
        if (baseType.IsArray) baseType = baseType.GetElementType();
        else if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(List<>)) baseType = baseType.GetGenericArguments()[0];

        // Ищем всех наследников
        var types = TypeCache.GetTypesDerivedFrom(baseType)
            .Where(p => !p.IsAbstract && !p.IsInterface && p.IsSerializable) // Обязательно проверяем IsSerializable
            .OrderBy(p => p.Name);

        foreach (var type in types)
        {
            bool isSelected = property.managedReferenceValue != null && property.managedReferenceValue.GetType() == type;

            menu.AddItem(new GUIContent(type.Name), isSelected, () =>
            {
                try
                {
                    // Создаем новый экземпляр через конструктор по умолчанию
                    property.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Cannot create instance of {type.Name}. Make sure it has a parameterless constructor! Error: {e}");
                }
            });
        }

        menu.ShowAsContext();
    }
}