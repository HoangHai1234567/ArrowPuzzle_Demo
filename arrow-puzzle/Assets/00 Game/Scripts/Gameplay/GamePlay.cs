using System;
using DG.Tweening;
using UnityEngine;

namespace ArrowsPuzzle
{
    public class GamePlay : MonoBehaviour
    {
        #region Serialize properties

        [SerializeField]
        private GameManager _gameManager;

        [SerializeField] 
        private float _holdArrowThreshold = 0.5f;

        #endregion

        #region Private properties

        private MapManager _mapManager;

        private Camera _camera;

        private Arrow _currentArrow = null;

        #endregion

        #region Public properties

        public GameManager GameManager => _gameManager;

        #endregion

        #region Events

        public event Action<Arrow> OnArrowCleared;

        public event Action<Arrow> OnArrowHit;

        #endregion

        #region Unity messages

        private void Start()
        {
            _mapManager = _gameManager.MapManager;
            _camera = Camera.main;
        }

        private float _holdTime = 0;

        private bool _isHolding = false;

        private void LateUpdate()
        {
            if (!_gameManager.IsPlaying)
            {
                return;
            }
            if (!_gameManager.EnableInteract)
            {
                return;
            }
            if (_gameManager.IsGameOver || _gameManager.IsGameCompleted)
            {
                return;
            }

            //first tap
            if (_currentArrow == null && Input.GetMouseButtonDown(0))
            {
                FirstTapHandler();
            }

            if (_currentArrow != null)
            {
                TapOrHoldHandler();
            }
        }

        private void FirstTapHandler()
        {
            var position = _camera.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            _holdTime = 0;
            _mapManager.CheckClickArrow(position, out _currentArrow);
        }

        private void TapOrHoldHandler()
        {
            _holdTime += Time.deltaTime;
            if (_holdTime > _holdArrowThreshold)
            {
                _isHolding = true;
                _currentArrow.Highlight();
                EventManager.GamePlay.OnArrowHighlightEvent(_currentArrow);
            }

            if (Input.GetMouseButtonUp(0))
            {
                //holding an arrow action
                if (_isHolding)
                {
                    _isHolding = false;
                    _currentArrow.UnHighlight();
                    EventManager.GamePlay.OnArrowUnHighlightEvent(_currentArrow);
                }
                else //quick tap and release an arrow action
                {
                    _currentArrow.Normal();
                    Go(_currentArrow);
                }
                _holdTime = 0;
                _currentArrow = null;
            }
        }

        #endregion

        private void Go(Arrow arrow)
        {
            var can = _mapManager.ArrowCanGo(arrow, out var hitId, out var hitNode);
            if (can)
            {
                _mapManager.RemoveArrow(arrow);
                _mapManager.ShowArrowDots(arrow);
                arrow.Normal();
                arrow.GoForward(ArrowGoForwardCallback);
            }
            else
            {
                var other = _mapManager.Arrows[hitId];
                ArrowHit(arrow, hitNode, other);
            }
        }

        private void ArrowGoForwardCallback(Arrow arrow)
        {
            arrow.gameObject.SetActive(false);
            OnArrowCleared?.Invoke(arrow);
            _currentArrow = null;
        }

        private void ArrowHit(Arrow arrow, Vector2Int hitNode, Arrow other)
        {
            arrow.Error();
            _mapManager.ShowArrowDots(arrow);
            arrow.GoForwardError(hitNode,other, () =>
            {
                _mapManager.HideArrowDots(arrow);
                float shakingStrength = 0.04f;
                _camera.transform.DOShakePosition(0.4f,
                    new Vector3(shakingStrength, shakingStrength));

                OnArrowHit?.Invoke(arrow);
                _currentArrow = null;
            });

        }

        public void Clear()
        {
            _currentArrow = null;
            _holdTime = 0;
            _isHolding = false;
        }
    }
}
