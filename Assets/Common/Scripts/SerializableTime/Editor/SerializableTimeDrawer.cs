// Assets/Editor/SerailizableTimeDrawer.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Common.Scripts.SerializableTime.Editor
{
    /// <summary>
    /// SerializableTime을 인스펙터에서 한 줄 "[]h []m []s.fff"처럼 입력 (필드 뒤에 접미 라벨)
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableTime))]
    public class SerializableTimeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var pHours   = property.FindPropertyRelative("hours");
            var pMinutes = property.FindPropertyRelative("minutes");
            var pSeconds = property.FindPropertyRelative("seconds");

            float padding = 2f;
            float fieldW  = Mathf.Max(40f, (position.width - padding * 3) * 0.18f);
            float infoW   = position.width - (fieldW * 3 + padding * 3);

            var rH = new Rect(position.x,                position.y, fieldW, EditorGUIUtility.singleLineHeight);
            var rM = new Rect(rH.xMax + padding,         position.y, fieldW, EditorGUIUtility.singleLineHeight);
            var rS = new Rect(rM.xMax + padding,         position.y, fieldW, EditorGUIUtility.singleLineHeight);
            var rI = new Rect(rS.xMax + padding,         position.y, infoW,  EditorGUIUtility.singleLineHeight);

            // 변경: 입력칸 뒤에 접미 라벨 배치 -> []h []m []s
            DrawIntWithSuffix (rH, "h", pHours,   min: 0);
            DrawIntWithSuffix (rM, "m", pMinutes, min: 0);
            DrawFloatWithSuffix(rS, "s", pSeconds, min: 0f);

            int hours   = pHours.intValue;
            int minutes = pMinutes.intValue;
            float seconds = pSeconds.floatValue;

            Normalize(ref hours, ref minutes, ref seconds);

            if (hours   != pHours.intValue   ||
                minutes != pMinutes.intValue ||
                Mathf.Abs(seconds - pSeconds.floatValue) > 0.0001f)
            {
                pHours.intValue     = hours;
                pMinutes.intValue   = minutes;
                pSeconds.floatValue = seconds;
            }

            double totalSeconds = hours * 3600d + minutes * 60d + seconds;
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.TextField(rI, $"{totalSeconds:0.###} s  ({totalSeconds * 1000d:0} ms)");
            }

            EditorGUI.EndProperty();
        }

        // ====== 접미 라벨 버전 ======

        private static void DrawIntWithSuffix(Rect r, string suffix, SerializedProperty prop, int min = 0)
        {
            const float suffixW = 12f; // 접미 라벨 폭
            const float gap = 2f;

            var rf = new Rect(r.x, r.y, r.width - (suffixW + gap), r.height); // 숫자 입력칸
            var rl = new Rect(rf.xMax + gap, r.y, suffixW, r.height);         // 접미 라벨

            int v = EditorGUI.DelayedIntField(rf, GUIContent.none, prop.intValue);
            if (v < min) v = min;
            prop.intValue = v;

            EditorGUI.LabelField(rl, suffix, EditorStyles.miniLabel);
        }

        private static void DrawFloatWithSuffix(Rect r, string suffix, SerializedProperty prop, float min = 0f)
        {
            const float suffixW = 12f; // 접미 라벨 폭
            const float gap = 2f;

            var rf = new Rect(r.x, r.y, r.width - (suffixW + gap), r.height); // 숫자 입력칸
            var rl = new Rect(rf.xMax + gap, r.y, suffixW, r.height);         // 접미 라벨

            float v = EditorGUI.DelayedFloatField(rf, GUIContent.none, prop.floatValue);
            if (v < min) v = min;
            prop.floatValue = v;

            EditorGUI.LabelField(rl, suffix, EditorStyles.miniLabel);
        }

        /// <summary>
        /// 초→분, 분→시 승급 및 범위 보정 (음수 방지)
        /// </summary>
        private static void Normalize(ref int h, ref int m, ref float s)
        {
            if (h < 0) h = 0;
            if (m < 0) m = 0;
            if (s < 0f) s = 0f;

            if (s >= 60f)
            {
                int addM = Mathf.FloorToInt(s / 60f);
                s -= addM * 60f;
                m += addM;
            }
            if (m >= 60)
            {
                int addH = m / 60;
                m -= addH * 60;
                h += addH;
            }

            s = Mathf.Round(s * 1000f) / 1000f; // 밀리초 3자리
        }
    }
}
#endif
