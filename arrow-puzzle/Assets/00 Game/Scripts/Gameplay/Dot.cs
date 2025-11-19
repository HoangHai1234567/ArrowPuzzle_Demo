using DG.Tweening;
using UnityEngine;

namespace ArrowsPuzzle
{
    public class Dot : MonoBehaviour
    {
        public bool Won { get; private set; }
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Win()
        {
            if (Won)
            {
                return;
            }

            transform.DOScale(Vector3.one*3, 0.15f)
                .OnComplete(() =>
                {
                    transform.DOScale(Vector3.zero, 0.075f)
                        .SetDelay(0.1f);
                });
            Won = true;
        }
    }
}
