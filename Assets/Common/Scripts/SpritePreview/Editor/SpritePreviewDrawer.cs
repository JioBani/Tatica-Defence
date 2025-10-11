#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Common.Scripts.SpritePreview.Editor
{
    [CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
    public class SpritePreviewDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty p, GUIContent l)
        {
            float h = EditorGUIUtility.singleLineHeight;
            if (p.objectReferenceValue is Sprite) h += ((SpritePreviewAttribute)attribute).Height + 6f;
            return h;
        }

        public override void OnGUI(Rect pos, SerializedProperty p, GUIContent l)
        {
            var attr = (SpritePreviewAttribute)attribute;

            var field = new Rect(pos.x, pos.y, pos.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(field, p, l);

            if (p.objectReferenceValue is Sprite s)
            {
                var preview = new Rect(pos.x, field.yMax + 4f, pos.width, attr.Height);

                var tex = s.texture;
                var tr = s.textureRect;
                var uv = new Rect(tr.x/tex.width, tr.y/tex.height, tr.width/tex.width, tr.height/tex.height);

                float aspect = tr.width / tr.height;
                float w = preview.height * aspect, h = preview.height;
                if (w > preview.width) { w = preview.width; h = preview.width / aspect; }
                var draw = new Rect(preview.x + (preview.width - w)/2f, preview.y + (preview.height - h)/2f, w, h);

                GUI.DrawTextureWithTexCoords(draw, tex, uv, true);
                GUI.Box(draw, GUIContent.none); // 테두리(선택)
            }
        }
    }
}
#endif