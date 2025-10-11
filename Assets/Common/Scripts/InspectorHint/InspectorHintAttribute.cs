using UnityEngine;

public enum InspectorHintPlacement
{
    Right,   // 필드 오른쪽에 라벨로 표시
    Below    // 필드 아래에 HelpBox로 표시
}

public class InspectorHintAttribute : PropertyAttribute
{
    public readonly string Text;
    public readonly InspectorHintPlacement Placement;
    public readonly float RightWidth;

    /// <param name="text">항상 보여줄 설명</param>
    /// <param name="placement">Right: 옆, Below: 아래</param>
    /// <param name="rightWidth">Right 배치 시 설명 영역 폭</param>
    public InspectorHintAttribute(string text, InspectorHintPlacement placement = InspectorHintPlacement.Right, float rightWidth = 200f)
    {
        Text = text;
        Placement = placement;
        RightWidth = rightWidth;
    }
}