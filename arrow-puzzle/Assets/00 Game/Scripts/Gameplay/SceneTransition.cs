using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArrowsPuzzle
{
    public class SceneTransition : MonoBehaviour
    {
        public static SceneTransition Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _leftCloud;
        [SerializeField] private RectTransform _rightCloud;

        [Header("Timing")]
        [SerializeField] private float _moveInDuration = 0.5f;
        [SerializeField] private float _holdDuration = 0.5f;
        [SerializeField] private float _moveOutDuration = 0.5f;

        private Vector2 _leftOffPos;
        private Vector2 _rightOffPos;
        private Vector2 _centerLeftPos;
        private Vector2 _centerRightPos;

        private bool _isPlaying;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (!_canvas) _canvas = GetComponentInChildren<Canvas>();

            _leftOffPos = _leftCloud.anchoredPosition;
            _rightOffPos = _rightCloud.anchoredPosition;

            // vị trí khi che màn: 2 đám mây/tán cây gặp nhau ở giữa
            _centerLeftPos = new Vector2(0f, _leftCloud.anchoredPosition.y);
            _centerRightPos = new Vector2(0f, _rightCloud.anchoredPosition.y);

            if (_canvas != null) _canvas.enabled = false;
        }

        public void LoadSceneWithTransition(string sceneName)
        {
            if (_isPlaying) return;
            StartCoroutine(DoTransition(sceneName));
        }

        private IEnumerator DoTransition(string sceneName)
        {
            _isPlaying = true;

            if (_canvas != null) _canvas.enabled = true;

            // reset vị trí off-screen
            _leftCloud.anchoredPosition = _leftOffPos;
            _rightCloud.anchoredPosition = _rightOffPos;

            // 1. Bay vào che màn
            var seqIn = DOTween.Sequence();
            seqIn.Join(_leftCloud
                .DOAnchorPos(_centerLeftPos, _moveInDuration)
                .SetEase(Ease.OutQuad));
            seqIn.Join(_rightCloud
                .DOAnchorPos(_centerRightPos, _moveInDuration)
                .SetEase(Ease.OutQuad));

            yield return seqIn.WaitForCompletion();

            // 2. Giữ che 1 lúc
            yield return new WaitForSeconds(_holdDuration);

            // 3. Load scene mới
            var async = SceneManager.LoadSceneAsync(sceneName);
            yield return async;
            yield return null; // đợi 1 frame cho UI/camera scene mới ổn

            // 4. Bay ra khỏi màn
            var seqOut = DOTween.Sequence();
            seqOut.Join(_leftCloud
                .DOAnchorPos(_leftOffPos, _moveOutDuration)
                .SetEase(Ease.InQuad));
            seqOut.Join(_rightCloud
                .DOAnchorPos(_rightOffPos, _moveOutDuration)
                .SetEase(Ease.InQuad));

            yield return seqOut.WaitForCompletion();

            if (_canvas != null) _canvas.enabled = false;

            _isPlaying = false;
        }
    }
}
