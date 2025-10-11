using UnityEngine;

namespace Common.Scripts.SpritePreview
{
    public class SpritePreviewAttribute : PropertyAttribute
    {
        public int Height;
        public SpritePreviewAttribute(int height = 140) { Height = height; }
    }
}
