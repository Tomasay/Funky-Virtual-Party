using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class Demo_RealtimeAssistant_Function_Character : MonoBehaviour
    {
        [SerializeField] private RectTransform characterTransform;
        [SerializeField] private Image characterFace;

        [SerializeField] private Color blue;
        [SerializeField] private Color green;
        [SerializeField] private Color red;

        private const float kMoveDistancePixel = 120f;
        private const float kMoveDuration = 1f;
        private Coroutine moveCoroutine;

        public enum CharacterColor
        {
            Blue,
            Green,
            Red
        }

        public void ChangeCharacterColor(CharacterColor color)
        {
            switch (color)
            {
                case CharacterColor.Blue:
                    MakeCharacterBlue();
                    break;
                case CharacterColor.Green:
                    MakeCharacterGreen();
                    break;
                case CharacterColor.Red:
                    MakeCharacterRed();
                    break;
            }
        }

        public void MakeCharacterBlue()
        {
            characterFace.color = blue;
        }

        public void MakeCharacterGreen()
        {
            characterFace.color = green;
        }

        public void MakeCharacterRed()
        {
            characterFace.color = red;
        }

        public void MoveCharacterToLeft()
        {
            Vector2 targetPos = characterTransform.anchoredPosition + new Vector2(-kMoveDistancePixel, 0f);
            StartMoveAnimation(targetPos);
        }

        public void MoveCharacterToRight()
        {
            Vector2 targetPos = characterTransform.anchoredPosition + new Vector2(kMoveDistancePixel, 0f);
            StartMoveAnimation(targetPos);
        }

        private void StartMoveAnimation(Vector2 target)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            moveCoroutine = StartCoroutine(MoveSmooth(characterTransform.anchoredPosition, target, kMoveDuration));
        }

        private IEnumerator MoveSmooth(Vector2 from, Vector2 to, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                characterTransform.anchoredPosition = Vector2.Lerp(from, to, EaseOutQuad(t));
                yield return null;
            }

            characterTransform.anchoredPosition = to;
            moveCoroutine = null;
        }

        // 부드러운 가속-감속 이징
        private float EaseOutQuad(float t) => 1 - (1 - t) * (1 - t);
    }
}