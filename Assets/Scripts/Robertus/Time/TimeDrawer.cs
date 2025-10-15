#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Ini buat mengatur penampilan/logic untuk custom inspector SerializedTime.
/// Sebenarnya gapapa ga usah baca ini, soalnya ga berhubungan dengan logic game. Cuma editor doang.
/// </summary>
[CustomPropertyDrawer(typeof(SerializedTime))]
public class TimeDrawer : PropertyDrawer
{
    //https://docs.unity3d.com/6000.2/Documentation/ScriptReference/PropertyDrawer.html
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var hourRect = new Rect(position.x, position.y, 30, position.height);
        var divisorRect = new Rect(position.x + 30f, position.y, 10, position.height);
        var minuteRect = new Rect(position.x + 40, position.y, 30, position.height);
        //Jadi nanti tampilannya seperti ini
        //HOUR : MINUTE
        //Di mana Divisor itu pembatasnya, a.k.a titik dua a.k.a ":"

        //Ini supaya ":" berada tepat di antara jam dan menit, dan berwarna putih.
        GUIStyle divisiorGuiStyle = new GUIStyle();
        divisiorGuiStyle.alignment = TextAnchor.MiddleCenter;
        divisiorGuiStyle.normal.textColor = Color.white;

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        //Actually tambahin custom inspectornya di inspector
        property.FindPropertyRelative("I_hour").intValue = DrawPaddedIntField(hourRect, property.FindPropertyRelative("I_hour").intValue, 0, 23);
        property.FindPropertyRelative("I_minute").intValue = DrawPaddedIntField(minuteRect, property.FindPropertyRelative("I_minute").intValue, 0, 59);

        //EditorGUI.PropertyField(hourRect, property.FindPropertyRelative("I_hour"), GUIContent.none);
        EditorGUI.LabelField(divisorRect, ":", divisiorGuiStyle);
        //EditorGUI.PropertyField(minuteRect, property.FindPropertyRelative("I_minute"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    //ChatGPT
    //Buat nunjukin angka di inspector sebagai 01, 02, dst.
    //Also bakal batasin angkanya dari min ke max
    private int DrawPaddedIntField(Rect rect, int value, int min, int max)
    {
        value = Mathf.Clamp(value, min, max);
        string padded = value.ToString($"D2");
        string newText = EditorGUI.DelayedTextField(rect, GUIContent.none, padded);
        if (int.TryParse(newText, out int newValue))
            return newValue;
        return value;
    }
}
#endif