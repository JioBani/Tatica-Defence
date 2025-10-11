#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Common.Scripts.InspectorHint.Editor
{
    [CustomPropertyDrawer(typeof(InspectorHintAttribute))]
    public class InspectorHintDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attr = (InspectorHintAttribute)attribute;

            // 전체 높이: children 포함
            float full = EditorGUI.GetPropertyHeight(property, includeChildren: true);

            if (attr.Placement == InspectorHintPlacement.Below)
            {
                // HelpBox 높이 더해주기
                float viewWidth = EditorGUIUtility.currentViewWidth;
                float contentWidth = Mathf.Max(100f, viewWidth - 40f);
                float help = EditorStyles.helpBox.CalcHeight(new GUIContent(attr.Text), contentWidth);
                return full + 4f + help;
            }

            // Right 배치는 높이 추가 없음(헤더 높이에 정렬해서 그릴 뿐)
            return full;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (InspectorHintAttribute)attribute;

            if (attr.Placement == InspectorHintPlacement.Right)
            {
                float spacing = 6f;
                float rightWidth = Mathf.Max(80f, attr.RightWidth);

                // 헤더 1줄 높이(접혔든 펼쳤든 상관없이 foldout 라인 높이)
                float headerHeight = EditorGUI.GetPropertyHeight(property, includeChildren: false);

                // 필드(전체) 영역은 오른쪽 여백만 줄인다
                var fieldRect = new Rect(position.x, position.y, position.width - rightWidth - spacing, position.height);
                // 힌트는 헤더 라인 한 줄 높이에만 그린다(내려가지 않도록)
                var hintRect  = new Rect(fieldRect.xMax + spacing, position.y, rightWidth, headerHeight);

                // 먼저 전체 필드를 그린다(접힘/펼침·자식 포함)
                EditorGUI.PropertyField(fieldRect, property, label, includeChildren: true);

                // 그 다음 힌트를 헤더 라인에 맞춰 오버레이
                var style = new GUIStyle(EditorStyles.label) { wordWrap = true, alignment = TextAnchor.MiddleLeft };
                EditorGUI.LabelField(hintRect, new GUIContent(attr.Text), style);
            }
            else // Below
            {
                float fieldHeight = EditorGUI.GetPropertyHeight(property, includeChildren: true);
                var fieldRect = new Rect(position.x, position.y, position.width, fieldHeight);
                EditorGUI.PropertyField(fieldRect, property, label, includeChildren: true);

                var helpRect = new Rect(position.x, fieldRect.yMax + 4f, position.width, position.height - fieldHeight - 4f);
                EditorGUI.HelpBox(helpRect, attr.Text, MessageType.None);
            }
        }
    }
}
#endif
