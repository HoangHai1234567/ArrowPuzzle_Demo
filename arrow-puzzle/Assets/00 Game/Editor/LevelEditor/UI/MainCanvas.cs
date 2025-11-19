using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DVLib.Graphics;
using Newtonsoft.Json;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArrowsPuzzle.Editor
{
    public enum NodeProcessType
    {
        Head,
        Tail
    }
    public class MainCanvas : VisualElement
    {
        EditorLevelData _levelData;

        private EditorArrow _selectedArrow = null;

        private bool _controlKeyDown = false;


        private Rect _headRect = new Rect(0, 0, EditorConfig.GridSize, EditorConfig.GridSize);

        private Rect _tailRect = new Rect(0, 0, EditorConfig.GridSize, EditorConfig.GridSize);

        private bool _isMouseDown = false;

        private NodeProcessType _nodeProcessType = NodeProcessType.Head;

        private Texture _previewImage;

        public EditorLevelData LevelData => _levelData;

        public MainCanvas()
        {
            ResizeCanvas();

            style.backgroundColor = new StyleColor(new Color(0.15f,0.15f,0.15f));
            generateVisualContent+= GenerateVisualContent;

            this.focusable = true;
            this.Focus();
            this.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            this.RegisterCallback<MouseDownEvent>(OnMouseDown);
            this.RegisterCallback<MouseUpEvent>(OnMouseUp);
            this.RegisterCallback<KeyDownEvent>(OnKeyDown);
            this.RegisterCallback<KeyUpEvent>(OnKeyUp);

            EditorEventManager.OnGridSizeChanged += EditorEventManager_OnGridSizeChanged;
            EditorEventManager.OnLevelSizeChanged += EditorEventManager_OnLevelSizeChanged;
            EditorEventManager.OnPreviewImageOpened += EditorEventManager_OnPreviewImageOpened;
            EditorEventManager.OnPreviewImageOpacityChanged += EditorEventManager_OnPreviewImageOpacityChanged;

            EditorCoroutineUtility.StartCoroutine(_CreateEmptyLevel(),this);

            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;

        }

        ~MainCanvas()
        {
            EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
        }
        private void EditorApplication_playModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                if (!string.IsNullOrEmpty(_texturePath))
                {
                    LoadPreviewImage();
                }
            }
            MarkDirtyRepaint();
        }

        private void EditorEventManager_OnPreviewImageOpacityChanged(float opacity)
        {
            _previewImageColor.a = opacity;
            MarkDirtyRepaint();
        }

        private string _texturePath = String.Empty;
        private void EditorEventManager_OnPreviewImageOpened(string texturePath)
        {
            //_previewImage = texture;
            _texturePath = texturePath;
            LoadPreviewImage();

            MarkDirtyRepaint();
        }

        void LoadPreviewImage()
        {
            if (!File.Exists(_texturePath))
            {
                Debug.LogError("Find not found: " + _texturePath);
            }

            byte[] fileData = File.ReadAllBytes(_texturePath);

            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);

            _previewImage = tex;
        }

        void ResizeCanvas()
        {
            EditorConfig.Columns = EditorConfig.LevelWidth + EditorConfig.OffsetX*2;
            EditorConfig.Rows = EditorConfig.LevelHeight + EditorConfig.OffsetY * 2;

            int w = EditorConfig.GridSize * EditorConfig.Columns;
            int h = EditorConfig.GridSize * EditorConfig.Rows;

            EditorConfig.CanvasWidth = w;
            EditorConfig.CanvasHeight = h;

            style.width = EditorConfig.CanvasWidth;
            style.height = EditorConfig.CanvasHeight;

            _boundingRect = EditorConfig.GetBoundingRect();


            MarkDirtyRepaint();
        }

        private void EditorEventManager_OnLevelSizeChanged(int width, int height)
        {
            /*
            if (width >= _levelData.Width && height >= _levelData.Height)
            {
                EditorConfig.LevelWidth = width;
                EditorConfig.LevelHeight = height;

                EditorConfig.Rows = height + EditorConfig.OffsetY*2;
                EditorConfig.Columns = width + EditorConfig.OffsetX*2;
                _levelData.ChangeLevelSize(width, height);

                ChangeSize();
            }
            */
        }

        private void EditorEventManager_OnGridSizeChanged(float newValue, float oldValue)
        {
            EditorConfig.ArrowLineWidth = EditorConfig.GridSize * 5 / 25f;

            ChangeLevelSize();
        }


        void ChangeLevelSize()
        {
            ResizeCanvas();

            foreach (var pair in _levelData.Arrows)
            {
                pair.Value.ChangeGridSize();
            }
            if (_selectedArrow != null)
            {
                HighlightArrowHeadTail(_selectedArrow);
            }
        }
        #region event handlers



        #endregion

        #region Key Process

        private void OnKeyUp(KeyUpEvent evt)
        {
            _controlKeyDown = evt.ctrlKey;
            _shiftKeyDown = evt.shiftKey;

            if (_shiftKeyDown)
            {
                _nodeProcessType = NodeProcessType.Tail;
            }
            else
            {
                _nodeProcessType = NodeProcessType.Head;
            }

            _canCreateNewArrow = false;
        }

        private bool _shiftKeyDown = false;
        private bool _canCreateNewArrow = false;
        private void OnKeyDown(KeyDownEvent evt)
        {
            _controlKeyDown = evt.ctrlKey;
            _shiftKeyDown = evt.shiftKey;

            if (_shiftKeyDown)
            {
                _nodeProcessType = NodeProcessType.Tail;
            }
            else
            {
                _nodeProcessType = NodeProcessType.Head;
            }

            if (evt.keyCode == KeyCode.N)
            {
                _canCreateNewArrow = true;
            }
        }

        #endregion

        IEnumerator _CreateEmptyLevel()
        {
            yield return null;

            _levelData = new EditorLevelData();
            MarkDirtyRepaint();
        }

        EditorArrow CreateNewArrow(int id, Vector2Int node)
        {
            if (!_levelData.Arrows.ContainsKey(id))
            {
                var arrow = _levelData.CreateNewArrow(id, node);
                return arrow;
            }

            return null;
        }

        #region Mouse Process
        private void OnMouseUp(MouseUpEvent evt)
        {
            MarkDirtyRepaint();

            _isMouseDown = false;
        }
        void OnMouseMove(MouseMoveEvent evt)
        {
            MarkDirtyRepaint();

            if (_isMouseDown && _selectedArrow != null)
            {
                if (_currentTool == EditorToolType.SelectArrow)
                {
                    MouseMoveProcess(evt.localMousePosition);
                }
            }

        }

        private void MouseMoveProcess(Vector2 localPosition)
        {
            Vector2 localPos = localPosition;
            var node = EditorConfig.ConvertPositionToIndex(localPos);

            if (!IsValidNode(node))
            {
                _selectedArrow?.Normal();
                _selectedArrow = null;

                return;
            }

            if (_selectedArrow == null)
            {
                //select an arrow
                EditorArrow arrow;
                if (_levelData.CheckClickArrow(node.x, node.y, out arrow))
                {
                    _selectedArrow?.Normal();

                    _selectedArrow = arrow;
                    _selectedArrow.Highlight();

                    HighlightArrowHeadTail(arrow);
                }
                //else
            }
            else
            {
                EditorArrow arrow;
                if (_levelData.CheckClickArrow(node.x, node.y, out arrow))
                {
                    if (_selectedArrow != arrow)
                    {
                        _selectedArrow.Normal();

                        _selectedArrow = arrow;
                        _selectedArrow.Highlight();

                        HighlightArrowHeadTail(arrow);
                    }
                }
                else
                {
                    if (_nodeProcessType == NodeProcessType.Tail)
                    {
                        var tail = _selectedArrow.Tail;
                        var v = tail - node;

                        if (v.sqrMagnitude <= 1)
                        {
                            AddTailToArrow(_selectedArrow, node);
                        }
                    }
                    else //Head
                    {
                        var head = _selectedArrow.Head;
                        var v = head - node;

                        if (v.sqrMagnitude <= 1)
                        {
                            AddHeadToArrow(_selectedArrow, node);
                        }
                    }

                }
            }
        }


        private void OnMouseDown(MouseDownEvent evt)
        {
            MarkDirtyRepaint();

            //left mouse button
            if (evt.button == 0)
            {
                Vector2 localPos = evt.localMousePosition;
                var node = EditorConfig.ConvertPositionToIndex(localPos);

                if (_canCreateNewArrow)
                {
                    AddNewArrow(node);
                    return;
                }
                if (_currentTool == EditorToolType.AddArrow)
                {
                    AddNewArrow(node);
                }
                else if(_currentTool == EditorToolType.RemoveArrow)
                {
                    RemoveArrow(node);
                }
                else if(_currentTool == EditorToolType.SelectArrow)
                {
                    _isMouseDown = true;
                    EditArrow(node);
                }
            }
            else if (evt.button == 1)    //right mouse button
            {
                if (_controlKeyDown)
                {
                    RightMouseDownProcess();
                }
            }
        }

        void EditArrow(Vector2Int node)
        {
            if (!IsValidNode(node))
            {
                _selectedArrow?.Normal();
                _selectedArrow = null;

                return;
            }


            //if don't have an selected arrow
            if (_selectedArrow == null)
            {
                //select an arrow
                EditorArrow arrow;
                if (_levelData.CheckClickArrow(node.x, node.y, out arrow))
                {
                    _selectedArrow = arrow;
                    _selectedArrow.Highlight();

                    HighlightArrowHeadTail(arrow);
                }
            }
            else
            {
                EditorArrow arrow;
                if (_levelData.CheckClickArrow(node.x, node.y, out arrow))
                {
                    //select other arrow
                    if (_selectedArrow != arrow)
                    {
                        _selectedArrow.Normal();
                        _selectedArrow = arrow;
                        _selectedArrow.Highlight();

                        HighlightArrowHeadTail(arrow);
                    }
                }
                else
                {
                    //add new head to selected arrow
                    if (_nodeProcessType == NodeProcessType.Tail)
                    {
                        var tail = _selectedArrow.Tail;
                        var v = tail - node;

                        if (v.sqrMagnitude <= 1)
                        {
                            AddTailToArrow(_selectedArrow, node);
                        }
                    }
                    else
                    {
                        var head = _selectedArrow.Head;
                        var v = head - node;

                        if (v.sqrMagnitude <= 1)
                        {
                            AddHeadToArrow(_selectedArrow, node);
                        }
                    }
                }
            }
        }

        void AddNewArrow(Vector2Int node)
        {
            if (_levelData.IsEmptyNode(node))
            {
                int id = _levelData.GetMaxArrowId();
                CreateNewArrow(id, node);
            }
        }

        void RightMouseDownProcess()
        {
            if (_selectedArrow != null)
            {
                if (_nodeProcessType == NodeProcessType.Tail)
                {
                    _levelData.RemoveArrowTailNode(_selectedArrow);
                }
                else
                {
                    _levelData.RemoveArrowHeadNode(_selectedArrow);
                }
                HighlightArrowHeadTail(_selectedArrow);
            }
        }

        void RemoveArrow(Vector2Int node)
        {
            EditorArrow arrow;
            if (_levelData.CheckClickArrow(node.x, node.y, out arrow))
            {
                _levelData.RemoveArrow(arrow);
            }

            _selectedArrow = null;
        }
        #endregion

        private void AddHeadToArrow(EditorArrow arrow, Vector2Int node)
        {
            _levelData.AddHeadNodeToArrow(arrow, node);
            HighlightArrowHeadTail(arrow);
        }

        private void AddTailToArrow(EditorArrow arrow, Vector2Int node)
        {
            _levelData.AddTailNodeToArrow(arrow, node);
            HighlightArrowHeadTail(arrow);
        }

        private void HighlightArrowHeadTail(EditorArrow arrow)
        {
            var head = arrow.Head;
            var pos = EditorConfig.ConvertIndexToPosition(head.x, head.y + 1);
            _headRect.position = pos;
            _headRect.size = new Vector2(EditorConfig.GridSize, EditorConfig.GridSize);

            var tail = arrow.Tail;
            pos = EditorConfig.ConvertIndexToPosition(tail.x, tail.y + 1);
            _tailRect.position = pos;
            _tailRect.size = new Vector2(EditorConfig.GridSize, EditorConfig.GridSize);
        }

        private bool IsValidNode(Vector2Int node)
        {
            if (node.x >= 0 &&
                node.x < _levelData.Width &&
                node.y >=0 &&
                node.y < _levelData.Height)
            {
                return true;
            }
            return false;
        }


        //private int rows;
        //private int cols;

        private Rect _boundingRect;

        #region Draw


        private void GenerateVisualContent(MeshGenerationContext mc)
        {
            DrawPreviewImage(mc);
            DrawGrid(mc);
            DrawCoordinates(mc);

            
            DrawBounding(mc);

            DrawHeadTail(mc);
            _levelData?.Draw(mc);
        }


        void DrawGrid(MeshGenerationContext mc)
        {
            int w = EditorConfig.Columns * EditorConfig.GridSize;
            int h = EditorConfig.Rows * EditorConfig.GridSize;
            for (int row = 0; row <= EditorConfig.Rows; row++)
            {
                GraphicX.Line.Draw(mc, new Vector2(0, row * EditorConfig.GridSize),
                    new Vector2(w, row * EditorConfig.GridSize),
                    1, Color.gray);

            }
            for (int col = 0; col <= EditorConfig.Columns; col++)
            {
                GraphicX.Line.Draw(mc, new Vector2(col * EditorConfig.GridSize, 0),
                    new Vector2(col * EditorConfig.GridSize, h),
                    1, Color.gray);
            }
        }

        void DrawCoordinates(MeshGenerationContext mc)
        {
            for (int y = 0; y < EditorConfig.Rows; y += 5)
            {
                var pos = EditorConfig.ConvertIndexToPosition(0, y);
                pos.x -= 15;
                mc.DrawText(y.ToString(), pos, 10, Color.white);
            }

            for (int x = 5; x < EditorConfig.Columns; x += 5)
            {
                var pos = EditorConfig.ConvertIndexToPosition(x, 0);
                mc.DrawText(x.ToString(), pos, 10, Color.white);
            }


            GraphicX.Line.Draw(mc,
                EditorConfig.ConvertIndexToPosition(0, 0),
                EditorConfig.ConvertIndexToPosition(EditorConfig.Columns - EditorConfig.OffsetX, 0),
                1,
                Color.green);

            GraphicX.Line.Draw(mc,
                EditorConfig.ConvertIndexToPosition(0, 0),
                EditorConfig.ConvertIndexToPosition(0, EditorConfig.Rows - EditorConfig.OffsetY),
                1,
                Color.green);
        }

        void DrawBounding(MeshGenerationContext mc)
        {
            _boundingRect = EditorConfig.GetBoundingRect();
            GraphicX.Rectangle.Draw(mc, _boundingRect, 2, Color.red);
        }

        private Color _previewImageColor = Color.white;
        void DrawPreviewImage(MeshGenerationContext mc)
        {
            if (_previewImage!=null)
            {
                GraphicX.TextureElement.Draw(mc,_previewImage,_boundingRect, _previewImageColor);
            }
        }

        void DrawHeadTail(MeshGenerationContext mc)
        {
            //Head - Tail
            if (_selectedArrow != null)
            {
                GraphicX.Rectangle.DrawFill(mc, _headRect, Color.magenta, false, 1, Color.black);
                GraphicX.Rectangle.DrawFill(mc, _tailRect, Color.green, false, 1, Color.black);
            }
        }

        #endregion

        private EditorToolType _currentTool = EditorToolType.SelectArrow;
        public void ChangeToolType(EditorToolType toolType)
        {
            _currentTool = toolType;
        }

        public void CreateNewLevel()
        {
            _levelData.Create(EditorConfig.LevelWidth, EditorConfig.LevelHeight);


            _headRect.position = new Vector2(-1000, -1000);
            _tailRect.position = new Vector2(-1000, -1000);

            ResizeCanvas();
            EditorEventManager.OnNewLevelCreatedEvent(_levelData);
        }

        public void OpenLevel(LevelData levelData)
        {
            _headRect.position = new Vector2(-1000, -1000);
            _tailRect.position = new Vector2(-1000, -1000);
            
            EditorConfig.LevelWidth = levelData.Width;
            EditorConfig.LevelHeight = levelData.Height;
            ResizeCanvas();
            _levelData.Create(levelData.Width, levelData.Height);
            foreach (var pair in levelData.Arrows)
            {
                var firstNode = pair.Value[0];

                var arrow = _levelData.CreateNewArrow(pair.Key, firstNode);
                for (int i = 1; i < pair.Value.Length; i++)
                {
                    _levelData.AddHeadNodeToArrow(arrow, pair.Value[i]);
                }
            }

            

        }

        public void ChangeGirdSize(int newSize, int oldSize)
        {
            EditorConfig.ArrowLineWidth *= 1.0f*newSize/oldSize;
            foreach (var pair in _levelData.Arrows)
            {
                pair.Value.ChangeGridSize(newSize,oldSize);
            }

            MarkDirtyRepaint();
        }
        public string GetTextLevel()
        {
            LevelData levelData = new LevelData();

            levelData.Width = _levelData.Width;
            levelData.Height = _levelData.Height;

            levelData.Arrows = new Dictionary<int, Vector2Int[]>();
            foreach (var pair in _levelData.Arrows)
            {
                var editorArrow = pair.Value;
                Vector2Int[] arrow = editorArrow.Nodes.ToArray();
                levelData.Arrows.Add(pair.Key,arrow);
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new IgnorePropertiesResolver(new[] { "magnitude", "sqrMagnitude" })
            };

            var text = JsonConvert.SerializeObject(levelData,settings);
            return text;
        }
    }
}
