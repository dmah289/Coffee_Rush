using System;
using Coffee_Rush.Board;
using Coffee_Rush.Level;
using Framework.ObjectPooling;
using UnityEngine;

namespace Coffee_Rush
{
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
        public float cellSize;
        public float borderSize;
    }
    
    public class BoardLayoutGenerator : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        
        [Header("References")]
        [SerializeField] private LevelLoader levelLoader;
        
        [Header("Board Manager")]
        [SerializeField] private BoardConfig boardConfig;
        private Tile[,] tiles;

        // In bound and active cells are valid
        private bool IsCellValid(int row, int col)
        {
            return (row >= 0 && row < levelLoader.currLevelData.height && col >= 0 && col < levelLoader.currLevelData.width)
                && levelLoader.currLevelData.GetCellData(row, col).isActive;
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
             selfTransform = transform;
         }
         
        public void SetupBoard()
        {
            tiles = new Tile[levelLoader.currLevelData.height,levelLoader.currLevelData.width];
            
            int halfWidth = levelLoader.currLevelData.width / 2;
            int halfHeight = levelLoader.currLevelData.height / 2;
            
            bool isEvenWidth = (levelLoader.currLevelData.width & 1) == 0;
            bool isEvenHeight = (levelLoader.currLevelData.height & 1) == 0;
            
            float posX, posY;
            
            for (int i = 0; i < levelLoader.currLevelData.height; i++)
            {
                for (int j = 0; j < levelLoader.currLevelData.width; j++)
                {
                    if (levelLoader.currLevelData.GetCellData(i, j).isActive)
                    {
                        Tile tile = ObjectPooler.GetFromPool<Tile>(PoolingType.Cell, selfTransform);
                    
                        if (isEvenWidth) posX = (j - halfWidth + 0.5f) * boardConfig.cellSize;
                        else posX = (j - halfWidth) * boardConfig.cellSize;

                        if (isEvenHeight) posY = (i - halfHeight + 0.5f) * boardConfig.cellSize;
                        else posY = (i - halfHeight) * boardConfig.cellSize;
                    
                        tile.SelfTransform.position = new Vector3(posX, posY, 0f);
#if UNITY_EDITOR
                        tile.name = $"Cell_{i}_{j}";
#endif
                        tiles[i,j] = tile;

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
            Vector3 verticalLocalScale = new Vector3(boardConfig.cellSize * (levelLoader.currLevelData.height - 1), 1f, 1f);
            
            Transform rightBorder = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            rightBorder.position = new Vector3(boardConfig.cellSize * levelLoader.currLevelData.width / 2 + boardConfig.borderSize / 2, 0, 0);
            rightBorder.eulerAngles = verticalEulerAngles;
            rightBorder.localScale = verticalLocalScale;
#if UNITY_EDITOR
            rightBorder.name = "right_border";
#endif
            
            Transform leftBorder = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            leftBorder.position = new Vector3(-boardConfig.cellSize * levelLoader.currLevelData.width / 2 - boardConfig.borderSize / 2, 0, 0);
            leftBorder.eulerAngles = verticalEulerAngles;
            leftBorder.localScale = verticalLocalScale;
            
#if UNITY_EDITOR
            leftBorder.name = "left_border";
#endif
        }

        private void SetupHorizontalBorder()
        {
            Vector3 horizontalLocalScale = new Vector3(boardConfig.cellSize * (levelLoader.currLevelData.width - 1), 1f, 1f);
            
            Transform bottomBorder = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            bottomBorder.position = new Vector3(0, -boardConfig.cellSize * levelLoader.currLevelData.height / 2 - boardConfig.borderSize / 2, 0);
            bottomBorder.eulerAngles = Vector3.zero;
            bottomBorder.localScale = horizontalLocalScale;
#if UNITY_EDITOR
            bottomBorder.name = "bottom_border";
#endif
            
            Transform topBorder = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            topBorder.position = new Vector3(0, boardConfig.cellSize * levelLoader.currLevelData.height / 2 + boardConfig.borderSize / 2, 0);
            topBorder.eulerAngles = Vector3.zero;
            topBorder.localScale = horizontalLocalScale;
#if UNITY_EDITOR
            topBorder.name = "top_border";
#endif
        }
    } 
}