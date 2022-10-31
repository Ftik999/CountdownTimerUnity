using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Timer.Scripts
{
    public class TweenController
    {
        private ICoroutineRunner _coroutineRunner;

        public TweenController(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public Coroutine MovementObject(RectTransform[] target, Vector2 endPosition, float moveTime,
            float moveInterval, Action onComplete)
        {
            return _coroutineRunner.StartCoroutine(MoveObjects(target, endPosition, moveTime, moveInterval,
                onComplete));
        }

        public Coroutine MovementObject(RectTransform target, Vector2 endPosition, float moveTime,
            Action onComplete)
        {
            return _coroutineRunner.StartCoroutine(MoveObject(target, endPosition, moveTime, onComplete));
        }

        public Coroutine ChangeColor(Image targetImage, Color finalColor, float animationTime)
        {
            return _coroutineRunner.StartCoroutine(AnimatedChangeColor(targetImage, finalColor, animationTime));
        }

        public Coroutine ShakeObject(Transform target, float temp_shake_intensity)
        {
            return _coroutineRunner.StartCoroutine(AnimatedShakeObject(target, temp_shake_intensity));
        }

        public void StopCoroutine(Coroutine coroutine)
        {
            _coroutineRunner.StopCoroutine(coroutine);
        }

        private IEnumerator AnimatedShakeObject(Transform target, float temp_shake_intensity)
        {
            Quaternion originRotation = target.rotation;
            float shake_decay = 0.002f;

            while (temp_shake_intensity > 0)
            {
                target.rotation = new Quaternion(
                    originRotation.x + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
                    originRotation.y + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
                    originRotation.z + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
                    originRotation.w + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f);
                temp_shake_intensity -= shake_decay;
                yield return null;
            }
        }

        private IEnumerator AnimatedChangeColor(Image targetImage, Color finalColor, float animationTime)
        {
            var startColor = targetImage.color;

            float interpolationRatio = 0;

            while (targetImage.color != finalColor)
            {
                targetImage.color = Color.Lerp(startColor, finalColor, interpolationRatio / animationTime);
                interpolationRatio += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator MoveObjects(RectTransform[] target, Vector2 endPosition, float moveTime, float moveInterval,
            Action onComplete)
        {
            for (int i = 0; i < target.Length; i++)
            {
                Action onCompleteMove = null;

                if (i != 0)
                {
                    endPosition = new Vector2(endPosition.x, target[i].anchoredPosition.y);
                }

                if (i == target.Length - 1)
                {
                    onCompleteMove = onComplete;
                }

                _coroutineRunner.StartCoroutine(MoveObject(target[i], endPosition, moveTime, onCompleteMove));
                yield return new WaitForSeconds(moveInterval);
            }
        }

        private IEnumerator MoveObject(RectTransform target, Vector2 endPosition, float moveTime,
            Action onComplete = null)
        {
            Vector2 startPosition = target.anchoredPosition;
            float interpolationRatio = 0;

            while (Vector2.Distance(target.anchoredPosition, endPosition) > 0.1f)
            {
                target.anchoredPosition = Vector2.Lerp(startPosition, endPosition, interpolationRatio / moveTime);
                interpolationRatio += Time.deltaTime;
                yield return null;
            }

            onComplete?.Invoke();
        }
    }
}