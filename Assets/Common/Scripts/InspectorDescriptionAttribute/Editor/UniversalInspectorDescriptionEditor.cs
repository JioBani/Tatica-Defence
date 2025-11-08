// UniversalInspectorDescriptionEditor.cs  (Assets/Editor 폴더 안)
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Common.Scripts.InspectorDescriptionAttributes
{
    [CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
    [CanEditMultipleObjects]
    public class UniversalInspectorDescriptionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // 대상 타입에서 우리 Attribute 찾기 (상속 포함)
            var type = target.GetType();
            var attr = type.GetCustomAttributes(typeof(InspectorDescriptionAttribute), true)
                .Cast<InspectorDescriptionAttribute>()
                .FirstOrDefault();

            if (attr != null)
            {
                // Unity MessageType 매핑
                MessageType unityType = MessageType.None;
                switch (attr.messageType)
                {
                    case InspectorMessageType.Info:    unityType = MessageType.Info; break;
                    case InspectorMessageType.Warning: unityType = MessageType.Warning; break;
                    case InspectorMessageType.Error:   unityType = MessageType.Error; break;
                    case InspectorMessageType.None:    unityType = MessageType.None; break;
                }

                EditorGUILayout.HelpBox(attr.text, unityType);
                EditorGUILayout.Space();
            }

            // 원래 인스펙터 그림
            DrawDefaultInspector();
        }
    }
}
#endif