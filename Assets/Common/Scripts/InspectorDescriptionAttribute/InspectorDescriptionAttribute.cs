// InspectorDescriptionAttribute.cs

using UnityEngine;

namespace Common.Scripts.InspectorDescriptionAttributes
{
    public enum InspectorMessageType { None, Info, Warning, Error }

    /// <summary>
    /// 컴포넌트 인스펙터 상단에 고정 설명 박스를 띄웁니다.
    /// 사용: [InspectorDescription("이 컴포넌트는 ...", InspectorMessageType.Info)]
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class InspectorDescriptionAttribute : PropertyAttribute
    {
        public readonly string text;
        public readonly InspectorMessageType messageType;

        public InspectorDescriptionAttribute(string text, InspectorMessageType messageType = InspectorMessageType.Info)
        {
            this.text = text;
            this.messageType = messageType;
        }
    }
}