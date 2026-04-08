using Scenes.Battle.Feature.Synergy;
using UnityEngine;

namespace Scenes.Battle.Feature.Ui.SynergyInfo
{
    /// <summary>
    /// 시너지 정보 UI의 루트 컴포넌트. SynergyListPanel과 SynergyDetailPanel을 조율한다.
    /// </summary>
    public class SynergyInfoPanel : MonoBehaviour
    {
        /// <summary>시너지 목록 패널.</summary>
        [SerializeField] private SynergyListPanel listPanel;

        /// <summary>시너지 상세 패널.</summary>
        [SerializeField] private SynergyDetailPanel detailPanel;

        private void OnEnable()
        {
            listPanel.OnIndicatorClicked += HandleIndicatorClicked;
        }

        private void OnDisable()
        {
            listPanel.OnIndicatorClicked -= HandleIndicatorClicked;
        }

        /// <summary>인디케이터 클릭을 수신하여 상세 패널을 토글한다.</summary>
        private void HandleIndicatorClicked(SynergyActivation activation)
        {
            // 같은 시너지를 다시 클릭하면 패널을 닫는다 (CD-2)
            if (detailPanel.gameObject.activeSelf && detailPanel.CurrentActivation == activation)
            {
                detailPanel.Hide();
            }
            else
            {
                // 새 시너지이거나 패널이 닫혀있으면 해당 시너지로 열기 (CD-1, CD-3)
                detailPanel.Show(activation);
            }
        }
    }
}
