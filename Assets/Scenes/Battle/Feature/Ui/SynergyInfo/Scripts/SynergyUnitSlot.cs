using Common.Data.Units.UnitLoadOuts;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Battle.Feature.Ui.SynergyInfo
{
    /// <summary>
    /// 상세 패널 내 소환수 아이콘 슬롯. 해당 시너지에 속하는 소환수를 표시한다.
    /// </summary>
    public class SynergyUnitSlot : MonoBehaviour
    {
        /// <summary>소환수 아이콘.</summary>
        [SerializeField] private Image unitIcon;

        /// <summary>활성/비활성 시각적 구분용.</summary>
        [SerializeField] private CanvasGroup canvasGroup;

        /// <summary>바인딩된 소환수 데이터.</summary>
        private UnitLoadOutData _unitLoadOut;

        /// <summary>바인딩된 소환수 데이터.</summary>
        public UnitLoadOutData UnitLoadOut => _unitLoadOut;

        /// <summary>소환수 데이터를 바인딩하고 아이콘을 설정한다.</summary>
        public void Bind(UnitLoadOutData unitLoadOut)
        {
            _unitLoadOut = unitLoadOut;
            unitIcon.sprite = unitLoadOut.Unit.Icon;
        }

        /// <summary>배치 상태에 따라 활성/비활성을 시각적으로 구분한다.</summary>
        public void SetDeployed(bool isDeployed)
        {
            if (isDeployed)
            {
                canvasGroup.alpha = 1f;
            }
            else
            {
                canvasGroup.alpha = 0.4f;
            }
        }
    }
}
