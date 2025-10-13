using TMPro;
using UnityEngine;

namespace Scenes.Battle.Scripts.Unit.Aggressor
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
