using System;
using System.Collections;
using UnityEngine;

namespace Timer.Scripts.UI
{
    public class LoadingCurtain : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _curtain;

        public void Hide(Action onComplete) =>
            StartCoroutine(FadeIn(onComplete));

        private IEnumerator FadeIn(Action onComplete)
        {
            while (_curtain.alpha > 0)
            {
                _curtain.alpha -= 0.03f;
                yield return new WaitForSeconds(0.03f);
            }

            onComplete?.Invoke();
            gameObject.SetActive(false);
        }
    }
}