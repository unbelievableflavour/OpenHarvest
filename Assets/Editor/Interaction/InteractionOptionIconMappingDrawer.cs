using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InteractionOptionIconMapping))]
public class InteractionOptionIconMappingDrawer : PropertyDrawer
{
    private const string ActionTypeFieldName = "actionType";
    private const string IconFieldName = "icon";
    private const string CustomOptionLabel = "<Custom>";

    private static readonly string[] TypeOptions =
    {
        "NpcChatInteractionOptionAction",
        "NpcContractsInteractionOptionAction",
        "NpcFollowToggleInteractionOptionAction",
        "NpcGiftInteractionOptionAction",
        "NpcStoreInteractionOptionAction",
        "NpcQuestInteractableOptionAction",
        CustomOptionLabel
    };

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty actionTypeProperty = property.FindPropertyRelative(ActionTypeFieldName);
        SerializedProperty iconProperty = property.FindPropertyRelative(IconFieldName);
        if (actionTypeProperty == null || iconProperty == null)
        {
            EditorGUI.LabelField(position, label.text, "Invalid mapping property.");
            return;
        }

        Rect row1 = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect row2 = new Rect(
            position.x,
            row1.yMax + EditorGUIUtility.standardVerticalSpacing,
            position.width,
            EditorGUIUtility.singleLineHeight);

        int selectedIndex = ResolveSelectedIndex(actionTypeProperty.stringValue);
        int newIndex = EditorGUI.Popup(row1, "Action Type", selectedIndex, TypeOptions);

        if (newIndex >= 0 && newIndex < TypeOptions.Length)
        {
            string picked = TypeOptions[newIndex];
            if (!string.Equals(picked, CustomOptionLabel, StringComparison.Ordinal))
            {
                actionTypeProperty.stringValue = picked;
            }
        }

        if (IsCustomSelection(newIndex))
        {
            EditorGUI.PropertyField(row2, actionTypeProperty, new GUIContent("Custom Type"));
            return;
        }

        EditorGUI.PropertyField(row2, iconProperty);
    }

    private static bool IsCustomSelection(int selectedIndex)
    {
        if (selectedIndex < 0 || selectedIndex >= TypeOptions.Length)
        {
            return true;
        }

        return string.Equals(TypeOptions[selectedIndex], CustomOptionLabel, StringComparison.Ordinal);
    }

    private static int ResolveSelectedIndex(string currentValue)
    {
        if (!string.IsNullOrWhiteSpace(currentValue))
        {
            string trimmed = currentValue.Trim();
            for (int i = 0; i < TypeOptions.Length; i++)
            {
                if (string.Equals(TypeOptions[i], trimmed, StringComparison.Ordinal))
                {
                    return i;
                }
            }
        }

        return TypeOptions.Length - 1;
    }
}
