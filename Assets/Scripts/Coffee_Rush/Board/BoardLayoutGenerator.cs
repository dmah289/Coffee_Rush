using System;
using System.Collections;
using Coffee_Rush.Board;
using Coffee_Rush.Level;
using Framework.DesignPattern;
using Framework.ObjectPooling;
using Unity.Mathematics;
using UnityEngine;

namespace Coffee_Rush
{
    
    public class BoardLayoutGenerator : MonoSingleton<BoardLayoutGenerator>
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        
        [Header("References")]
        [SerializeField] private LevelLoader levelLoader;
        
        [Header("Board Manager")]
        public Tile[,] tiles;


        protected override void Awake()
        {
            base.Awake();
            
            selfTransform = transform;
        }

        private bool IsInBound(int row, int col, int width, int height)
        {
            return row >= 0 && row < height && col >= 0 && col < width;
        }
        
        private eCornerType FindCornerBorder(int row, int col, LevelData levelData)
        {
            eCornerType eCornerType = eCornerType.None;
            bool state = levelData.GetCellData(row, col).isActive;
            for (byte i = 0; i < 4; i++)
            {
                int newRow1 = row + BoardConfig.tileDirections[i].x;
                int newCol1 = col + BoardConfig.tileDirections[i].y;
                
                int newRow2 = row + BoardConfig.tileDirections[(i + 1) % 4].x;
                int newCol2 = col + BoardConfig.tileDirections[(i + 1) % 4].y;

                bool state1, state2;
                
                if(!IsInBound(newRow1, newCol1, levelData.width, levelData.height))
                    state1 = false;
                else state1 = levelData.GetCellData(newRow1, newCol1).isActive;
                
                if(!IsInBound(newRow2, newCol2, levelData.width, levelData.height))
                    state2 = false;
                else state2 = levelData.GetCellData(newRow2, newCol2).isActive;

                if (state1 != state2) continue;
                
                if(state != state1)
                    eCornerType = eCornerType | (eCornerType)(1 << i);
            }
            return eCornerType;
        }
         
        public IEnumerator SetupBoard(LevelData levelData)
        {
            tiles = new Tile[levelData.height,levelData.width];
            
            int halfWidth = levelData.width / 2;
            int halfHeight = levelData.height / 2;
            
            bool isEvenWidth = (levelData.width & 1) == 0;
            bool isEvenHeight = (levelData.height & 1) == 0;
            
            float posX, posY;
            bool state;
            
            for (int i = 0; i < levelData.height; i++)
            {
                for (int j = 0; j < levelData.width; j++)
                {
                    if (isEvenWidth) posX = (j - halfWidth + 0.5f) * BoardConfig.cellSize;
                    else posX = (j - halfWidth) * BoardConfig.cellSize;

                    if (isEvenHeight) posY = (i - halfHeight + 0.5f) * BoardConfig.cellSize;
                    else posY = (i - halfHeight) * BoardConfig.cellSize;

                    state = levelData.GetCellData(i, j).isActive;

                    eCornerType type = FindCornerBorder(i, j, levelData);
                    
                    if (state)
                    {
                        Tile tile = ObjectPooler.GetFromPool<Tile>(PoolingType.Tile, selfTransform);
                    
                        tile.SelfTransform.position = new Vector3(posX, posY, 0f);
#if UNITY_EDITOR
                        tile.name = $"Cell_{i}_{j}";
#endif
                        tiles[i,j] = tile;
                    }
                    
                    if(type.HasFlag(eCornerType.TopLeft)) SetupTopLeftCorner(posX, posY, state);
                    if(type.HasFlag(eCornerType.TopRight)) SetupTopRightCorner(posX, posY, state);
                    if(type.HasFlag(eCornerType.BottomRight)) SetupBottomRightCorner(posX, posY, state);
                    if(type.HasFlag(eCornerType.BottomLeft)) SetupBottomLeftCorner(posX, posY, state);
                }
            }
            
            SetupContinuousStraightBorders(levelData);

            yield return null;
        }

        private void SetupContinuousStraightBorders(LevelData levelData)
        {
            for (int row = 0; row <= levelData.height; row++)
            {
                int startCol = -1;
                bool lastBelowState = false;

                for (int col = 0; col <= levelData.width; col++)
                {
                    (bool needBorder, bool belowState) res = NeedHorizontalBorder(row, col, levelData);

                    if (res.needBorder)
                    {
                        if(startCol == -1) startCol = col;
                        else lastBelowState = res.belowState;
                    }
                    else if (startCol != -1 && (!res.needBorder || col == levelData.width))
                    {
                        CreateHorizontalBorder(row, startCol, col - 1, levelData, lastBelowState);
                        startCol = -1;
                    }
                }
            }

            for (int col = 0; col <= levelData.width; col++)
            {
                int startRow = -1;
                bool lastLeftState = false;

                for (int row = 0; row <= levelData.height; row++)
                {
                    (bool needBorder, bool leftState) res = NeedVerticalBorder(row, col, levelData);
                    if (res.needBorder && startRow == -1)
                    {
                        startRow = row;
                        lastLeftState = res.leftState;
                    }
                    else if (startRow != -1 && (!res.needBorder || row == levelData.height))
                    {
                        CreateVerticalBorder(col, startRow, row - 1, levelData, lastLeftState);
                        startRow = -1;
                    }
                }
            }
        }

        private void CreateVerticalBorder(int col, int startRow, int endRow, LevelData levelData, bool lastLeftState)
        {
            int halfWidth = levelData.width / 2;
            int halfHeight = levelData.height / 2;
            
            bool isEvenWidth = (levelData.width & 1) == 0;
            bool isEvenHeight = (levelData.height & 1) == 0;

            float x, startY;

            if (isEvenWidth) x = (col - halfWidth + 0.5f) * BoardConfig.cellSize - BoardConfig.cellSize / 2;
            else x = (col - halfWidth) * BoardConfig.cellSize - BoardConfig.cellSize / 2;
            
            if(isEvenHeight) startY = (startRow - halfHeight + 0.5f) * BoardConfig.cellSize;
            else startY = (startRow - halfHeight) * BoardConfig.cellSize;
            
            float length = (endRow - startRow) * BoardConfig.cellSize;
            if (length == 0) return;
            float cenrterY = startY + length / 2;
            
            Transform border = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            border.position = new Vector3(x, cenrterY, 0f);
            border.eulerAngles = lastLeftState ? new Vector3(0, 0, -90) : new Vector3(0, 0, 90);
            border.localScale = new Vector3(length, 1f, 1f);
            
#if UNITY_EDITOR
            border.name = $"VerticalBorder_{col}_[{startRow}]_[{endRow}]";
#endif

        }

        private void CreateHorizontalBorder(int row, int startCol, int endCol, LevelData levelData, bool belowState)
        {
            int halfWidth = levelData.width / 2;
            int halfHeight = levelData.height / 2;
            
            bool isEvenWidth = (levelData.width & 1) == 0;
            bool isEvenHeight = (levelData.height & 1) == 0;

            float startX, y;
            
            if(isEvenWidth) startX = (startCol - halfWidth + 0.5f) * BoardConfig.cellSize;
            else startX = (startCol - halfWidth) * BoardConfig.cellSize;
            
            if(isEvenHeight) y = (row - halfHeight + 0.5f) * BoardConfig.cellSize - BoardConfig.cellSize / 2;
            else y = (row - halfHeight) * BoardConfig.cellSize - BoardConfig.cellSize / 2;
            
            float lengthX = (endCol - startCol) * BoardConfig.cellSize;

            if (lengthX == 0) return;
            
            float centerX = startX + lengthX / 2;

            Transform border = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            border.position = new Vector3(centerX, y, 0f);
            border.eulerAngles = belowState ? Vector3.zero : new Vector3(0, 0, 180);
            border.localScale = new Vector3(lengthX, 1f, 1f);
            
#if UNITY_EDITOR
            border.name = $"HorizontalBorder_{row}_[{startCol}]_[{endCol}]";
#endif
        }

        private (bool needBorder, bool belowState) NeedHorizontalBorder(int row, int col, LevelData levelData)
        {
            bool currState = row < levelData.height && IsInBound(row, col, levelData.width, levelData.height) && levelData.GetCellData(row, col).isActive;
            bool belowState = row > 0 && IsInBound(row - 1, col, levelData.width, levelData.height) && levelData.GetCellData(row - 1, col).isActive;

            return (currState != belowState, belowState);
        }

        private (bool needBorder, bool leftState) NeedVerticalBorder(int row, int col, LevelData levelData)
        {
            bool currState = col < levelData.width && IsInBound(row, col, levelData.width, levelData.height) && levelData.GetCellData(row, col).isActive;
            bool leftState = col > 0 && IsInBound(row, col-1, levelData.width, levelData.height) && levelData.GetCellData(row, col-1).isActive;

            return (currState != leftState, leftState);
        }
        
        private void SetupTopLeftCorner(float posX, float posY, bool state)
        {
            Transform corner = ObjectPooler.GetFromPool<Transform>(state ? PoolingType.OuterCorner : PoolingType.InnerCorner, selfTransform);
            corner.position = new Vector3(posX - BoardConfig.cellSize / 2, posY  + BoardConfig.cellSize / 2, 0f);
            corner.eulerAngles = new Vector3(0, 0, 270f);
#if UNITY_EDITOR
            corner.name = "top_left_corner";
#endif
        }

        private void SetupTopRightCorner(float posX, float posY, bool state)
        {
            Transform corner = ObjectPooler.GetFromPool<Transform>(state ? PoolingType.OuterCorner : PoolingType.InnerCorner, selfTransform);
            corner.position = new Vector3(posX + BoardConfig.cellSize / 2, posY  + BoardConfig.cellSize / 2, 0f);
            corner.eulerAngles = new Vector3(0, 0, 180f);
#if UNITY_EDITOR
            corner.name = $"top_right_corner";
#endif
        }

        private void SetupBottomRightCorner(float posX, float posY, bool state)
        {
            Transform corner = ObjectPooler.GetFromPool<Transform>(state ? PoolingType.OuterCorner : PoolingType.InnerCorner, selfTransform);
            corner.position = new Vector3(posX + BoardConfig.cellSize / 2, posY - BoardConfig.cellSize / 2, 0f);
            corner.eulerAngles = new Vector3(0, 0, 90f);
#if UNITY_EDITOR
            corner.name = "bottom_right_corner";
#endif
        }

        private void SetupBottomLeftCorner(float posX, float posY, bool state)
        {
            Transform corner = ObjectPooler.GetFromPool<Transform>(state ? PoolingType.OuterCorner : PoolingType.InnerCorner, selfTransform);
            corner.position = new Vector3(posX - BoardConfig.cellSize / 2, posY - BoardConfig.cellSize / 2, 0f);
            corner.eulerAngles = Vector3.zero;
#if UNITY_EDITOR
            corner.name = "bottom_left_corner";
#endif
        }

        public Vector3 GetCoordPos(Vector3 worldPos)
        {
            int halfWidth = levelLoader.currLevelData.width / 2;
            int halfHeight = levelLoader.currLevelData.height / 2;
    
            bool isEvenWidth = (levelLoader.currLevelData.width & 1) == 0;
            bool isEvenHeight = (levelLoader.currLevelData.height & 1) == 0;
    
            float column, row;
    
            if (isEvenWidth) column = (worldPos.x / BoardConfig.cellSize) + halfWidth - 0.5f;
            else column = (worldPos.x / BoardConfig.cellSize) + halfWidth;
    
            if (isEvenHeight) row = (worldPos.y / BoardConfig.cellSize) + halfHeight - 0.5f;
            else row = (worldPos.y / BoardConfig.cellSize) + halfHeight;
            
            int roundedColumn = Mathf.RoundToInt(column);
            int roundedRow = Mathf.RoundToInt(row);
            
            return tiles[roundedRow, roundedColumn].SelfTransform.position;
        }
    } 
}