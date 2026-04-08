using System.Text;
using Common.Data.Synergies;
using TMPro;
using UnityEngine;

namespace Scenes.Battle.Feature.Ui.SynergyInfo
{
    /// <summary>
    /// 상세 패널 내 티어 1행. 필요 카운트와 효과 수치를 표시한다.
    /// </summary>
    public class SynergyTierRow : MonoBehaviour
    {
        /// <summary>필요 카운트 텍스트.</summary>
        [SerializeField] private TMP_Text thresholdText;

        /// <summary>효과 수치 텍스트.</summary>
        [SerializeField] private TMP_Text effectText;

        /// <summary>활성/비활성 시각적 구분용.</summary>
        [SerializeField] private CanvasGroup canvasGroup;

        /// <summary>티어 데이터를 바인딩하고 활성 상태에 따라 시각적으로 구분한다.</summary>
        public void Bind(SynergyTier tier, bool isActive)
        {
            thresholdText.text = tier.RequiredCount.ToString();
            effectText.text = FormatConstants(tier);

            if (isActive)
            {
                canvasGroup.alpha = 1f;
            }
            else
            {
                canvasGroup.alpha = 0.4f;
            }
        }

        /// <summary>티어의 Constants를 "key: value" 형태로 포매팅한다.</summary>
        private string FormatConstants(SynergyTier tier)
        {
            if (tier.Constants == null || tier.Constants.Count == 0)
            {
                return "";
            }

            StringBuilder stringBuilder = new();
            bool isFirst = true;

            foreach (string key in tier.Constants.Keys)
            {
                if (!isFirst)
                {
                    stringBuilder.Append(", ");
                }
                stringBuilder.Append(key);
                stringBuilder.Append(": ");
                stringBuilder.Append(tier.Constants[key]);
                isFirst = false;
            }

            return stringBuilder.ToString();
        }
    }
}
