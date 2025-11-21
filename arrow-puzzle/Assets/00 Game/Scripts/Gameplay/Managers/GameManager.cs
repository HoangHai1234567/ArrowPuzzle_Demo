using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace ArrowsPuzzle
{
    [RequireComponent(typeof(GamePlay))]
    public class GameManager : MonoBehaviour
    {
        #region Serialize properties

        [SerializeField, Required] private MapManager _mapManager;

        [SerializeField, Required] private GamePlay _gamePlay;

        [SerializeField, Required] private LevelManager _levelManager;

        #endregion

        #region Public properties

        public MapManager MapManager => _mapManager;

        public GamePlay GamePlay => _gamePlay;

        public int Lives { get; private set; } = MaxLives;

        public const int MaxLives = 3;

        public int Level { get; private set; }

        public bool IsPlaying { get; private set; }

        public bool IsGameOver { get; private set; }

        public bool IsGameCompleted { get; private set; }

        public bool EnableInteract { get; private set; }

        #endregion

        #region Unity messages

        private void Awake()
        {
#if !UNITY_EDITOR
            Application.targetFrameRate = 60;
#endif
            EventManager.ClearAllEventHandlers();

            EventManager.GamePlay.OnGameInit += GamePlay_OnGameInit;
            EventManager.GamePlay.OnGameOver += GameOver;
            EventManager.GamePlay.OnGameCompleted += GameCompleted;
            EventManager.GamePlay.OnGameReplay += GamePlay_OnGameReplay;
            EventManager.GamePlay.OnNextLevel += GamePlayOnOnNextLevel;
            EventManager.GamePlay.OnEnableInteraction += GamePlay_OnEnableInteraction;
            EventManager.GamePlay.OnGamePlay += GamePlay_OnGamePlay;
            EventManager.GamePlay.OnNeedHint += GamePlay_OnNeedHint;
            EventManager.GamePlay.OnHinted += GamePlay_OnHinted;
        }

        private void GamePlay_OnHinted(Arrow arrow)
        {
            if (arrow != null)
            {
                arrow.Hint();
            }
        }

        private void GamePlay_OnNeedHint()
        {
            _mapManager.FindHint((arrow) =>
            {
                EventManager.GamePlay.OnHintedEvent(arrow);
            });
        }

        private void GamePlay_OnGamePlay()
        {
            EnableInteract = true;
            IsPlaying = true;
        }

        private void GamePlay_OnEnableInteraction(bool enable)
        {
            if (enable)
            {
                StartCoroutine(_Delay());
                IEnumerator _Delay()
                {
                    yield return null;
                    EnableInteract = true;

                    _gamePlay.enabled = true;
                }
            }
            else
            {
                EnableInteract = false;
                _gamePlay.Clear();
                _gamePlay.enabled = false;
            }

        }

        private IEnumerator Start()
        {
            EnableInteract = false;
            _gamePlay = GetComponent<GamePlay>();
            _gamePlay.OnArrowHit += _gamePlay_OnArrowHit;
            _gamePlay.OnArrowCleared += _gamePlay_OnArrowCleared;
            Level = PlayerPrefs.GetInt(Values.GameDataKeys.Level, 1);
            EventManager.GamePlay.OnGameStartedEvent(this);

            yield return null;
            EventManager.GamePlay.OnGameInitEvent(this);
        }

        #endregion

        #region Event handlers

        private void GamePlay_OnGameReplay()
        {
            // Reset băng đạn TRƯỚC khi load lại scene
            var shooter = FindObjectOfType<PlayerShoot>();
            if (shooter != null)
            {
                shooter.ResetMagazine();
            }

            LoadGameScene();
        }

        private void GamePlayOnOnNextLevel()
        {
            Level++;
            PlayerPrefs.SetInt(Values.GameDataKeys.Level, Level);
            PlayerPrefs.Save();

            // Reset băng đạn TRƯỚC khi sang level mới
            var shooter = FindObjectOfType<PlayerShoot>();
            if (shooter != null)
            {
                shooter.ResetMagazine();
            }

            //Reset TDGameManager
            var tdManager = FindObjectOfType<TDGameManager>();
            if (tdManager != null)
            {  
                tdManager.ResetTDGame();
            }

            LoadGameScene();
        }

        private void LoadGameScene()
        {
            DOTween.KillAll();
            SceneManager.LoadScene(Values.Game.GameSceneName);
        }

        private void GamePlay_OnGameInit(GameManager gameManager)
        {
            _levelManager.GetCurrentLevel((level =>
            {
                EventManager.GamePlay.OnLevelLoadedEvent(level);
                _mapManager.Create(level, (() =>
                {
                    EventManager.GamePlay.OnLevelCreatedEvent(this, level);
                    EventManager.GamePlay.OnGamePlayEvent();
                }));
            }));
        }

        private void _gamePlay_OnArrowCleared(Arrow arrow)
        {
            if (IsGameCompleted)
            {
                return;
            }

            if (IsGameOver)
            {
                return;
            }
            if (_mapManager.IsLevelCleared())
            {
                IsGameCompleted = true;
                EventManager.GamePlay.OnGameCompletedEvent();
            }
        }

        private void _gamePlay_OnArrowHit(Arrow arrow)
        {
            if (IsGameCompleted)
            {
                return;
            }

            if (IsGameOver)
            {
                return;
            }
            EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
            if (enemyManager != null)
            {
                enemyManager.TriggerRandomEnemyAttack();
            }
            if (Lives > 0)
            {
                Lives--;
                EventManager.GamePlay.OnLostLivesEvent(Lives, MaxLives);
                if (Lives == 0)
                {
                    IsGameOver = true;
                    EventManager.GamePlay.OnGameOverEvent();
                }
            }
        }

        public void GameCompleted()
        {
            _mapManager.ShowWinDots(0.5f, () =>
            {
                EventManager.UIEvents.OnRequestShowUILevelCompletedEvent(0, false);
            });
        }
        public void GameOver()
        {
            EventManager.UIEvents.OnRequestShowUIGameOverEvent(0.5f, false);
        }

        #endregion
    }
}
