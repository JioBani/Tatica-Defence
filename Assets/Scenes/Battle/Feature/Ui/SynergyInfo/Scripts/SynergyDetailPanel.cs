using Scenes.Battle.Feature.Synergy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Battle.Feature.Ui.SynergyInfo
{
    /// <summary>
    /// 시너지 상세 패널. 시너지 클릭 시 상세 정보를 표시한다.
    /// </summary>
    public class SynergyDetailPanel : MonoBehaviour
    {
        /// <summary>시너지 아이콘.</summary>
        [SerializeField] private Image detailIcon;

        /// <summary>시너지 이름.</summary>
        [SerializeField] private TMP_Text detailName;

        /// <summary>닫기 버튼.</summary>
        [SerializeField] private Button closeButton;

        /// <summary>현재 표시 중인 시너지. 토글 판정에 사용한다.</summary>
        private SynergyActivation _currentActivation;

        /// <summary>현재 표시 중인 시너지.</summary>
        public SynergyActivation CurrentActivation => _currentActivation;

        private void Awake()
        {
            closeButton.onClick.AddListener(Hide);
        }

        /// <summary>상세 패널을 열고 시너지 데이터를 바인딩한다.</summary>
        public void Show(SynergyActivation activation)
        {
            _currentActivation = activation;
            detailIcon.sprite = activation.Definition.Icon;
            detailName.text = activation.Definition.DisplayName;
            gameObject.SetActive(true);
        }

        /// <summary>상세 패널을 닫는다.</summary>
        public void Hide()
        {
            _currentActivation = null;
            gameObject.SetActive(false);
        }
    }
}
