using UnityEngine;

namespace Scenes.Battle.Feature.Units.HealthBars
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer outlineSprite;
        [SerializeField] private SpriteRenderer insideSprite;

        public void Display(float rate)
        {
            if (rate <= 0)
            {
                rate = 0;
            }
            
            float sizeX = outlineSprite.size.x * rate;
            insideSprite.size = new Vector2(sizeX, insideSprite.size.y);
        }
    }
}
