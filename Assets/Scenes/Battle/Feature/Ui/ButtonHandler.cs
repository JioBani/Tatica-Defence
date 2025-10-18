using DG.Tweening;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds.Ui
{
    public class ButtonHandler : MonoBehaviour
    {
        public void OnClickShowEnemySideButton()
        {
            Camera.main.transform.DOMoveY(6, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetUpdate(UpdateType.Late, isIndependentUpdate: false);
        }
    }
}

