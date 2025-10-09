using NUnit.Framework;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

namespace Scenes.Battle.Scripts.Ui
{
    public class SwichViewButton : MonoBehaviour
    {

        [SerializeField] private bool isEnemySideView;
        [SerializeField] private TextMeshProUGUI buttonText;

        public void OnClick()
        {
            isEnemySideView = !isEnemySideView;

            if (isEnemySideView)
            {
                Camera.main.transform.DOMoveY(6, 0.5f)
                    .SetEase(Ease.InOutSine)
                    .SetUpdate(UpdateType.Late, isIndependentUpdate: false);
                
                buttonText.text = "Enemy";
            }
            else
            {
                Camera.main.transform.DOMoveY(0, 0.5f)
                    .SetEase(Ease.InOutSine)
                    .SetUpdate(UpdateType.Late, isIndependentUpdate: false);

                buttonText.text = "Defender";
            }

        }
    }
}


