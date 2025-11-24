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
        private MapManager _mapManager; // NEW: để lấy bound puzzle

        // NEW: lưu vị trí camera lúc vừa play
        private Vector3 _startCameraPos;

        // NEW: cho phép kéo xuống sâu hơn một chút
        [SerializeField]
        private float _extraDownOffset = 2f;

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
            _mapManager = gameManager.MapManager;   // NEW: lưu lại MapManager
        }

        public float zoomSpeed = 5f;
        public float minZoom = 10f;
        public float maxZoom = 27f;

        [Header("Map Bounds")]
        public Vector2 mapMin = new Vector2(-15f, -15f);
        public Vector2 mapMax = new Vector2(15f, 15f);

        private bool _isDragging = false;
        private Vector3 _dragOrigin;
        private bool _canInteract = false;

        [SerializeField] private float _cameraMoveThreshold = 0.1f;

        void Update()
        {
            if (!_canInteract) return;
            if (_gameManager == null) return;
            if (!_gameManager.IsPlaying) return;
            if (_gameManager.IsGameOver || _gameManager.IsGameCompleted) return;

            bool overPuzzle = IsPointerOverPuzzleArea();  // NEW

            PanCamera(overPuzzle);

#if UNITY_EDITOR
            if (overPuzzle)
                ZoomCamera();          // chỉ zoom nếu chuột đang ở puzzle
#else
            if (overPuzzle)
                HandleTouchZoom();
#endif
            ClampCamera();

            if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
                EventManager.GamePlay.OnEnableInteractionEvent(true);
            }
        }

        // NEW: kiểm tra chuột có nằm trong khung puzzle không
        bool IsPointerOverPuzzleArea()
        {
            if (_camera == null || _mapManager == null)
                return true; // fallback: cho phép nếu thiếu reference

            if (_mapManager.boundTopLeft == null ||
                _mapManager.boundBottomRight == null)
                return true;

            Vector3 tl = _camera.WorldToScreenPoint(_mapManager.boundTopLeft.position);
            Vector3 br = _camera.WorldToScreenPoint(_mapManager.boundBottomRight.position);

            // Screen origin (0,0) ở bottom-left
            Rect puzzleRect = Rect.MinMaxRect(tl.x, br.y, br.x, tl.y);

            return puzzleRect.Contains(Input.mousePosition);
        }

        void PanCamera(bool canStartDrag)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // chỉ bắt đầu drag nếu click trong vùng puzzle
                if (!canStartDrag) return;

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

            // Clamp X như cũ
            if (mapWidth <= viewWidth)
                pos.x = (mapMin.x + mapMax.x) / 2f;
            else
                pos.x = Mathf.Clamp(pos.x, mapMin.x + horzExtent, mapMax.x - horzExtent);

            // Tính min / max Y theo map
            float minY, maxY;
            if (mapHeight <= viewHeight)
            {
                float centerY = (mapMin.y + mapMax.y) / 2f;
                minY = maxY = centerY;
            }
            else
            {
                // cho phép kéo xuống sâu hơn một chút: trừ thêm _extraDownOffset
                minY = mapMin.y + vertExtent - _extraDownOffset;
                maxY = mapMax.y - vertExtent;
            }

            // Clamp theo map
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            // NEW: không cho kéo cao hơn vị trí ban đầu
            if (pos.y > _startCameraPos.y)
                pos.y = _startCameraPos.y;

            _camera.transform.position = pos;
        }


        bool CanPan()
        {
            float vertExtent = _camera.orthographicSize;
            float horzExtent = vertExtent * _camera.aspect;

            float viewWidth = horzExtent * 2f;
            float viewHeight = vertExtent * 2f;

            float mapWidth = mapMax.x - mapMin.x;
            float mapHeight = mapMax.y - mapMin.y;

            return (mapWidth > viewWidth || mapHeight > viewHeight);
        }

        #endregion

        private void GamePlay_OnLevelLoaded(Level level)
        {
            // size cố định khi bắt đầu
            maxZoom = 17f;
            _camera.orthographicSize = 17f;
            minZoom = minZoom;

            // NEW: lưu vị trí ban đầu để giới hạn kéo lên
            _startCameraPos = _camera.transform.position;
        }
    }
}
