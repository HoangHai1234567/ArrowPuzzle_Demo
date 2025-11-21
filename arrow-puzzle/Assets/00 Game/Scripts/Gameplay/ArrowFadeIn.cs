using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace ArrowsPuzzle
{
    public class ArrowFadeIn : MonoBehaviour
    {
        [Header("Fade settings")]
        [SerializeField] private float fadeDelay    = 0f; 
        [SerializeField] private float fadeDuration = 0.4f; 

        private Renderer[] renderers;

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        private void OnEnable()
        {
            PlayFadeIn();
        }

        public void PlayFadeIn()
        {
            if (renderers == null || renderers.Length == 0) return;

            foreach (var r in renderers)
            {
                if (r == null) continue;

                var mat = r.material;
                if (mat == null || !mat.HasProperty("_Color")) continue;

                Color c = mat.color;
                c.a = 0f;
                mat.color = c;

                mat.DOFade(1f, fadeDuration)
                .SetDelay(fadeDelay)
                .SetEase(Ease.OutQuad);
            }
        }
    }

}
