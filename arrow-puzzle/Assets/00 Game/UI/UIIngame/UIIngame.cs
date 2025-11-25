using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ArrowsPuzzle
{
    public class UIIngame : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textLevel;

        [SerializeField] private Button _buttonRestart;

        [SerializeField] private Button _buttonHome;

        [SerializeField] private Button _buttonHint;

        [SerializeField] private GameObject _heartIconPrefab;

        [SerializeField] private Transform _heartContainer;


        private int _maxLives = 3;
        void Start()
        {
            EventManager.GamePlay.OnLevelLoaded += GamePlay_OnLevelLoaded;
            EventManager.GamePlay.OnLostLives += GamePlay_OnLostLives;

            _buttonRestart.onClick.AddListener(EventManager.GamePlay.OnGameReplayEvent);

            _buttonHome.onClick.AddListener(OnHomeButtonClicked);

            _buttonHint.onClick.AddListener(() =>
            {
                EventManager.GamePlay.OnNeedHintEvent();
            });
        }

        private void OnHomeButtonClicked()
        {
            // Kill tween trong level hiện tại trước khi chuyển cảnh
            DOTween.KillAll();

            // Dùng transition nếu có, không thì load thẳng
            if (SceneTransition.Instance != null)
            {
                SceneTransition.Instance.LoadSceneWithTransition(Values.Game.HomeSceneName);
            }
            else
            {
                SceneManager.LoadScene(Values.Game.HomeSceneName);
            }
        }

        private void GamePlay_OnLevelLoaded(Level level)
        {
            _textLevel.text = $"Level {level.Name}";

            CreateHearts();
        }


        private void CreateHearts()
        {
            StartCoroutine(_Create());
            IEnumerator _Create()
            {
                yield return null;
                for (int i = 0; i < _maxLives; i++)
                {
                    var heartIcon = Instantiate(_heartIconPrefab, _heartContainer);
                    heartIcon.transform.localScale = Vector3.zero;
                }

                yield return null;

                foreach (Transform child in _heartContainer)
                {
                    child.DOScale(Vector3.one, 0.5f)
                        .SetEase(Ease.OutBack);
                    yield return new WaitForSeconds(0.2f);
                }

            }

        }
        private void GamePlay_OnLostLives(int lives, int maxLives)
        {
            if (lives < _heartContainer.childCount)
            {
                var heart = _heartContainer.GetChild(lives);
                heart.DOScale(Vector3.zero, 0.3f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() =>
                    {
                        heart.gameObject.SetActive(false);
                    });
            }
        }
    }
}
