using System;
using Coffee_Rush.Level;
using Framework.ObjectPooling;
using UnityEngine;

namespace Coffee_Rush
{
    public enum eBlockType : byte
    {
        None = 0,
        Block1 = 1,
        Block2 = 2,
    }
    
    public enum CellPosType : byte
    {
        Inner = 0,
        TopLeftCorner = 1,
        TopRightCorner = 2,
        BottomLeftCorner = 3,
        BottomRightCorner = 4,
    }

    [Serializable]
    public struct BoardConfig
    {
        public int maxWidth;
        public int maxHeight;
        public float cellSize;
        public float borderSize;
    }
    
    public class BoardController : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        
        [Header("Other Components")]
        [SerializeField] private LevelLoader levelLoader;
        
        [Header("Board Manager")]
        private Transform[][] cells;
        
        [Header("Board Configuration")]
        [SerializeField] private BoardConfig boardConfig;

        // Out of bounds or inactive cells are not valid
        private bool IsCellValid(int row, int col)
        {
            return (row >= 0 && row < boardConfig.maxHeight && col >= 0 && col < boardConfig.maxWidth)
                && levelLoader.currLevelData.cellsData[(row * boardConfig.maxWidth) + col].isActive;
        }

        private CellPosType GetCellPosType(int row, int col)
        {
            if (!IsCellValid(row, col) && IsCellValid(row, col-1) && IsCellValid(row-1, col) 
                || IsCellValid(row, col) && !IsCellValid(row, col-1) && !IsCellValid(row-1, col))
                return CellPosType.BottomLeftCorner;
            if (!IsCellValid(row, col) && IsCellValid(row, col+1) && IsCellValid(row-1, col)
                || IsCellValid(row, col) && !IsCellValid(row, col+1) && !IsCellValid(row-1, col))
                return CellPosType.BottomRightCorner;
            if (!IsCellValid(row, col) && IsCellValid(row, col+1) && IsCellValid(row+1, col)
                || IsCellValid(row, col) && !IsCellValid(row, col+1) && !IsCellValid(row+1, col))
                return CellPosType.TopRightCorner;
            if (!IsCellValid(row, col) && IsCellValid(row, col-1) && IsCellValid(row+1, col)
                || IsCellValid(row, col) && !IsCellValid(row, col+1) && !IsCellValid(row+1, col))
                return CellPosType.TopLeftCorner;

            return CellPosType.Inner;
        }

        private void Awake()
        {
            cells = new Transform[boardConfig.maxHeight][];
            selfTransform = transform;
        }

        private void Start()
        {
            SetupBoard();
        }

        private void SetupBoard()
        {
            int halfWidth = boardConfig.maxWidth / 2;
            int halfHeight = boardConfig.maxHeight / 2;
            
            bool isEvenWidth = (boardConfig.maxWidth & 1) == 0;
            bool isEvenHeight = (boardConfig.maxHeight & 1) == 0;
            
            float posX, posY;
            
            for (int i = 0; i < boardConfig.maxHeight; i++)
            {
                cells[i] = new Transform[boardConfig.maxWidth];
                for (int j = 0; j < boardConfig.maxWidth; j++)
                {
                    Transform cell = ObjectPooler.GetFromPool<Transform>(PoolingType.Cell, selfTransform);
                    
                    if (isEvenWidth) posX = (j - halfWidth + 0.5f) * boardConfig.cellSize;
                    else posX = (j - halfWidth) * boardConfig.cellSize;

                    if (isEvenHeight) posY = (i - halfHeight + 0.5f) * boardConfig.cellSize;
                    else posY = (i - halfHeight) * boardConfig.cellSize;
                    
                    cell.position = new Vector3(posX, posY, 0f);
#if UNITY_EDITOR
                    cell.name = $"Cell_{i}_{j}";
#endif
                    cells[i][j] = cell;

                    CellPosType type = GetCellPosType(i, j);
                    switch (type)
                    {
                        case CellPosType.Inner:
                            break;
                        case CellPosType.BottomLeftCorner:
                            SetupBottomLeftCorner(posX, posY);
                            break;
                        case CellPosType.BottomRightCorner:
                            SetupBottomRightCorner(posX, posY);
                            break;
                        case CellPosType.TopRightCorner:
                            SetupTopRightCorner(posX, posY);
                            break;
                        case CellPosType.TopLeftCorner:
                            SetupTopLeftCorner(posX, posY);
                            break;
                    }
                }
            }
            
            SetupVerticalBorder();
            SetupHorizontalBorder();
        }

        private void SetupTopLeftCorner(float posX, float posY)
        {
            Transform corner = ObjectPooler.GetFromPool<Transform>(PoolingType.OuterCorner, selfTransform);
            corner.position = new Vector3(posX - boardConfig.cellSize / 2, posY  + boardConfig.cellSize / 2, 0f);
            corner.eulerAngles = new Vector3(0, 0, 270f);
#if UNITY_EDITOR
            corner.name = "top_left_corner";
#endif
        }

        private void SetupTopRightCorner(float posX, float posY)
        {
            Transform corner = ObjectPooler.GetFromPool<Transform>(PoolingType.OuterCorner, selfTransform);
            corner.position = new Vector3(posX + boardConfig.cellSize / 2, posY  + boardConfig.cellSize / 2, 0f);
            corner.eulerAngles = new Vector3(0, 0, 180f);
#if UNITY_EDITOR
            corner.name = $"top_right_corner";
#endif
        }

        private void SetupBottomRightCorner(float posX, float posY)
        {
            Transform corner= ObjectPooler.GetFromPool<Transform>(PoolingType.OuterCorner, selfTransform);
            corner.position = new Vector3(posX + boardConfig.cellSize / 2, posY - boardConfig.cellSize / 2, 0f);
            corner.eulerAngles = new Vector3(0, 0, 90f);
#if UNITY_EDITOR
            corner.name = "bottom_right_corner";
#endif
        }

        private void SetupBottomLeftCorner(float posX, float posY)
        {
            Transform corner = ObjectPooler.GetFromPool<Transform>(PoolingType.OuterCorner, selfTransform);
            corner.position = new Vector3(posX - boardConfig.cellSize / 2, posY - boardConfig.cellSize / 2, 0f);
            corner.eulerAngles = Vector3.zero;
#if UNITY_EDITOR
            corner.name = "bottom_left_corner";
#endif
        }

        private void SetupVerticalBorder()
        {
            Vector3 verticalEulerAngles = new Vector3(0, 0, 90);
            Vector3 verticalLocalScale = new Vector3(boardConfig.cellSize * (boardConfig.maxHeight - 1), 1f, 1f);
            
            Transform rightBorder = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            rightBorder.position = new Vector3(boardConfig.cellSize * boardConfig.maxWidth / 2 + boardConfig.borderSize / 2, 0, 0);
            rightBorder.eulerAngles = verticalEulerAngles;
            rightBorder.localScale = verticalLocalScale;
#if UNITY_EDITOR
            rightBorder.name = "right_border";
#endif
            
            Transform leftBorder = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            leftBorder.position = new Vector3(-boardConfig.cellSize * boardConfig.maxWidth / 2 - boardConfig.borderSize / 2, 0, 0);
            leftBorder.eulerAngles = verticalEulerAngles;
            leftBorder.localScale = verticalLocalScale;
            
#if UNITY_EDITOR
            leftBorder.name = "left_border";
#endif
        }

        private void SetupHorizontalBorder()
        {
            Vector3 horizontalLocalScale = new Vector3(boardConfig.cellSize * (boardConfig.maxWidth - 1), 1f, 1f);
            
            Transform bottomBorder = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            bottomBorder.position = new Vector3(0, -boardConfig.cellSize * boardConfig.maxHeight / 2 - boardConfig.borderSize / 2, 0);
            bottomBorder.eulerAngles = Vector3.zero;
            bottomBorder.localScale = horizontalLocalScale;
#if UNITY_EDITOR
            bottomBorder.name = "bottom_border";
#endif
            
            Transform topBorder = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            topBorder.position = new Vector3(0, boardConfig.cellSize * boardConfig.maxHeight / 2 + boardConfig.borderSize / 2, 0);
            topBorder.eulerAngles = Vector3.zero;
            topBorder.localScale = horizontalLocalScale;
#if UNITY_EDITOR
            topBorder.name = "top_border";
#endif
        }
    } 
}