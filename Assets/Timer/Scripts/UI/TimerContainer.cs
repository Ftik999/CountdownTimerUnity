using UnityEngine;
using UnityEngine.UI;

namespace Timer.Scripts.UI
{
    public class TimerContainer : MonoBehaviour
    {
        [SerializeField] private RectTransform _timerContainerTransform;
        [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;
        [SerializeField] private ScrollRect _scrollRect;

        public RectTransform TimerContainerTransform => _timerContainerTransform;

        public void Construct(Vector2 spawnPosition, float containerSize)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = spawnPosition;

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                rectTransform.anchoredPosition.y + containerSize);
        }

        public void SetScroll(float value) =>
            _scrollRect.verticalNormalizedPosition = value;

        public void SetVerticalLayoutSpacing(float spacing) =>
            _verticalLayoutGroup.spacing = spacing;
    }
}