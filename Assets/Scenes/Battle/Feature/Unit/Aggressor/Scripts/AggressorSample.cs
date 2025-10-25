using TMPro;
using UnityEngine;

namespace Scenes.Battle.Feature.Aggressors
{
    public class AggressorSample : MonoBehaviour
    {
        [SerializeField] private TextMeshPro countText;

        public void SetCount(int count)
        {
            countText.text = $"x {count}";
        }
    }
}
