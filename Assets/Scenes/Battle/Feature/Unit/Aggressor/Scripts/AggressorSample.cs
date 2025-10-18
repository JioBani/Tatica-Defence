using TMPro;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds.Unit.Aggressor
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
