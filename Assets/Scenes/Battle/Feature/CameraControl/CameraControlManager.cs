using DG.Tweening;
using Scenes.Battle.Feature.Rounds;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds.CameraControl
{
    public class CameraControlManager : MonoBehaviour
    {
        Camera _mainCamera;
        
        private void Awake()
        {
            _mainCamera = Camera.main;
            
            RoundManager.Instance
                .GetPhase(PhaseType.Combat)
                .phaseEvent
                .Add(PhaseEventType.Enter, (_,_) => SetCombatMode());
            
            RoundManager.Instance
                .GetPhase(PhaseType.Maintenance)
                .phaseEvent
                .Add(PhaseEventType.Enter, (_,_) => SetMaintenanceMode());
        }
        
        private void SetCombatMode()
        {
            _mainCamera.DOOrthoSize(6.5f, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetUpdate(UpdateType.Late, isIndependentUpdate: false);
            
            _mainCamera.transform.DOMoveY(3.3f, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetUpdate(UpdateType.Late, isIndependentUpdate: false);
        }

        private void SetMaintenanceMode()
        {
            _mainCamera.DOOrthoSize(5f, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetUpdate(UpdateType.Late, isIndependentUpdate: false);
            
            _mainCamera.transform.DOMoveY(0f, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetUpdate(UpdateType.Late, isIndependentUpdate: false);
        }

        public void ShowAggressorSide()
        {
            _mainCamera.transform.DOMoveY(6, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetUpdate(UpdateType.Late, isIndependentUpdate: false);
        }

        public void ShowDefenderSide()
        {
            _mainCamera.transform.DOMoveY(0, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetUpdate(UpdateType.Late, isIndependentUpdate: false);
        }
    }
}
