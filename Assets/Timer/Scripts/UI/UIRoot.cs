using UnityEngine;
using UnityEngine.UI;

namespace Timer.Scripts.UI
{
    public class UIRoot : MonoBehaviour
    {
        private const float PartOfHeight = 0.25f;

        [SerializeField] private RectTransform _uiRootTransform;

        private Vector2 _startPositionToFirstTimer;
        public RectTransform UIRootTransform => _uiRootTransform;
        public Vector2 StartPositionToFirstTimer => _startPositionToFirstTimer;

        public void Init()
        {
            _uiRootTransform.GetComponent<RectMask2D>().padding = new Vector4(0, PartOfHeight * GetHeight(), 0, 0);
            float yPositionOnPartOfHeight = GetHeight() / 2 - PartOfHeight * GetHeight();
            _startPositionToFirstTimer = new Vector2(-GetWidth() / 2, yPositionOnPartOfHeight);
        }

        public float GetHeight() => _uiRootTransform.rect.height;

        private float GetWidth() => _uiRootTransform.rect.width;
    }
}