using UnityEngine;

namespace ArrowsPuzzle
{
    public class CameraManager : MonoBehaviour
    {
        #region Serialize properties

        [SerializeField]
        private float _minCameraSize = 22;

        #endregion

        #region private properties

        private Camera _camera;

        private GameManager _gameManager;

        #endregion

        #region Unity messages

        private void Start()
        {
            EventManager.GamePlay.OnGameStarted += GamePlay_OnGameStarted;
            EventManager.GamePlay.OnLevelLoaded += GamePlay_OnLevelLoaded;
            EventManager.GamePlay.OnGamePlay += GamePlay_OnGamePlay;

            EventManager.GamePlay.OnArrowHighlight += GamePlay_OnArrowHighlight;
            EventManager.GamePlay.OnArrowUnHighlight += GamePlay_OnArrowUnHighlight;

            _camera = Camera.main;
        }

        private void GamePlay_OnArrowUnHighlight(Arrow obj)
        {
            _canInteract = true;
            _isDragging = false;
        }

        private void GamePlay_OnArrowHighlight(Arrow obj)
        {
            _canInteract = false;
        }

        private void GamePlay_OnGamePlay()
        {
            _canInteract = true;
        }

        private void GamePlay_OnGameStarted(GameManager gameManager)
        {
            _gameManager = gameManager;
        }


        public float zoomSpeed = 5f;            
        public float minZoom = 2f;              
        public float maxZoom = 20f;

        [Header("Map Bounds")]
        public Vector2 mapMin = new Vector2(-10f, -10f);
        public Vector2 mapMax = new Vector2(10f, 10f);

        private bool _isDragging = false;

        private Vector3 _dragOrigin;

        private bool _canInteract = false;

        void Update()
        {
            if (!_canInteract)
            {
                return;
            }
            if (!_gameManager.IsPlaying)
            {
                return;
            }
            if (_gameManager.IsGameOver || _gameManager.IsGameCompleted)
            {
                return;
            }

            PanCamera();
#if UNITY_EDITOR
            ZoomCamera();
#else
            HandleTouchZoom();
#endif
            ClampCamera();
            if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
                EventManager.GamePlay.OnEnableInteractionEvent(true);
            }
        }

        [SerializeField] private float _cameraMoveThreshold = 0.1f;

        void PanCamera()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _dragOrigin = _camera.ScreenToWorldPoint(Input.mousePosition);
                _isDragging = true;
                
            }

            if (_isDragging)
            {
                Vector3 difference = _dragOrigin - _camera.ScreenToWorldPoint(Input.mousePosition);

                if (CanPan())
                {
                    _camera.transform.position += difference;
                }

                if (difference.sqrMagnitude > _cameraMoveThreshold * _cameraMoveThreshold)
                {
                    EventManager.GamePlay.OnEnableInteractionEvent(false);
                }
            }
        }

        void ZoomCamera()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scroll) > 0.0001f)
            {
                _camera.orthographicSize -= scroll * zoomSpeed;
                _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, minZoom, maxZoom);
            }
        }

#if !UNITY_EDITOR
        void HandleTouchZoom()
        {
            if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

                float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
                float currentMagnitude = (touch0.position - touch1.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                _camera.orthographicSize -= difference * zoomSpeed * Time.deltaTime;
                _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, minZoom, maxZoom);
            }
        }
#endif
        void ClampCamera()
        {
            float vertExtent = _camera.orthographicSize;
            float horzExtent = vertExtent * _camera.aspect;

            float viewWidth = horzExtent * 2f;
            float viewHeight = vertExtent * 2f;

            float mapWidth = mapMax.x - mapMin.x;
            float mapHeight = mapMax.y - mapMin.y;

            Vector3 pos = _camera.transform.position;

            
            if (mapWidth <= viewWidth)
                pos.x = (mapMin.x + mapMax.x) / 2f;
            else
                pos.x = Mathf.Clamp(pos.x, mapMin.x + horzExtent, mapMax.x - horzExtent);

            if (mapHeight <= viewHeight)
                pos.y = (mapMin.y + mapMax.y) / 2f;
            else
                pos.y = Mathf.Clamp(pos.y, mapMin.y + vertExtent, mapMax.y - vertExtent);

            _camera.transform.position = pos;
        }

        bool CanPan()
        {
            float vertExtent = _camera.orthographicSize;
            float horzExtent = vertExtent * -_camera.aspect;

            float viewWidth = horzExtent * 2f;
            float viewHeight = vertExtent * 2f;

            float mapWidth = mapMax.x - mapMin.x;
            float mapHeight = mapMax.y - mapMin.y;

            return (mapWidth > viewWidth || mapHeight > viewHeight);
        }
#endregion


        private void GamePlay_OnLevelLoaded(Level level)
        {
            float x = level.Data.Width/10.0f;
            float y = level.Data.Height/5.0f;
            mapMin = new Vector2(-level.Data.Width / 2.0f - x, -level.Data.Height / 2.0f - y);
            mapMax = new Vector2(level.Data.Width / 2.0f + x, level.Data.Height / 2.0f + y);

            float minSizeByWidth = level.Data.Width / (2f * _camera.aspect);
            float minSizeByHeight = level.Data.Height / 2f;

            maxZoom = Mathf.Min(minSizeByWidth, minSizeByHeight) + 12;
            minZoom = Mathf.Max(2f, maxZoom * 0.5f);

            _camera.orthographicSize = maxZoom;
        }


    }
}
