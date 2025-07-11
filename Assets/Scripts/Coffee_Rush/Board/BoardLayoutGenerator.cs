using System;
using Coffee_Rush.Board;
using Coffee_Rush.Level;
using Framework.ObjectPooling;
using Unity.Mathematics;
using UnityEngine;

namespace Coffee_Rush
{
    [Flags]
    public enum CornerType : byte
    {
        None = 0,
        TopLeft = 1 << 0,
        TopRight = 1 << 1,
        BottomRight = 1 << 2,
        BottomLeft = 1 << 3
    }

    [Serializable]
    public struct BoardConfig
    {
        public Vector2Int[] tileDirections;     // left, top, right, bottom
        public float cellSize;                  // 2
        public float borderSize;                // 0.3
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
        
        
        private void Awake()
        {
            selfTransform = transform;
        }

        private bool IsInBound(int row, int col, int width, int height)
        {
            return row >= 0 && row < height && col >= 0 && col < width;
        }
        
        private CornerType FindCornerBorder(int row, int col, LevelData levelData)
        {
            CornerType cornerType = CornerType.None;
            bool state = levelData.GetCellData(row, col).isActive;
            for (byte i = 0; i < 4; i++)
            {
                int newRow1 = row + boardConfig.tileDirections[i].x;
                int newCol1 = col + boardConfig.tileDirections[i].y;
                
                int newRow2 = row + boardConfig.tileDirections[(i + 1) % 4].x;
                int newCol2 = col + boardConfig.tileDirections[(i + 1) % 4].y;

                bool state1, state2;
                
                if(!IsInBound(newRow1, newCol1, levelData.width, levelData.height))
                    state1 = false;
                else state1 = levelData.GetCellData(newRow1, newCol1).isActive;
                
                if(!IsInBound(newRow2, newCol2, levelData.width, levelData.height))
                    state2 = false;
                else state2 = levelData.GetCellData(newRow2, newCol2).isActive;

                if (state1 != state2) continue;
                
                if(state != state1)
                    cornerType = cornerType | (CornerType)(1 << i);
            }
            return cornerType;
        }
         
        public void SetupBoard(LevelData levelData)
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
                    if (isEvenWidth) posX = (j - halfWidth + 0.5f) * boardConfig.cellSize;
                    else posX = (j - halfWidth) * boardConfig.cellSize;

                    if (isEvenHeight) posY = (i - halfHeight + 0.5f) * boardConfig.cellSize;
                    else posY = (i - halfHeight) * boardConfig.cellSize;

                    state = levelData.GetCellData(i, j).isActive;

                    CornerType type = FindCornerBorder(i, j, levelData);
                    // byte typeByte = (byte)type;
                    // string binaryRepresentation = Convert.ToString(typeByte, 2).PadLeft(8, '0');
                    // Debug.Log($"Cell ({i}, {j}) - Type: {type} - Binary: {binaryRepresentation}");
                    
                    if (state)
                    {
                        Tile tile = ObjectPooler.GetFromPool<Tile>(PoolingType.Cell, selfTransform);
                    
                        tile.SelfTransform.position = new Vector3(posX, posY, 0f);
#if UNITY_EDITOR
                        tile.name = $"Cell_{i}_{j}";
#endif
                        tiles[i,j] = tile;
                    }
                    
                    if(type.HasFlag(CornerType.TopLeft)) SetupTopLeftCorner(posX, posY, state);
                    if(type.HasFlag(CornerType.TopRight)) SetupTopRightCorner(posX, posY, state);
                    if(type.HasFlag(CornerType.BottomRight)) SetupBottomRightCorner(posX, posY, state);
                    if(type.HasFlag(CornerType.BottomLeft)) SetupBottomLeftCorner(posX, posY, state);
                }
            }
            
            SetupContinuousStraightBorders(levelData);
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
            Debug.Log($"{startRow} {endRow} {endRow}");
            
            int halfWidth = levelData.width / 2;
            int halfHeight = levelData.height / 2;
            
            bool isEvenWidth = (levelData.width & 1) == 0;
            bool isEvenHeight = (levelData.height & 1) == 0;

            float x, startY;

            if (isEvenWidth) x = (col - halfWidth + 0.5f) * boardConfig.cellSize - boardConfig.cellSize / 2;
            else x = (col - halfWidth) * boardConfig.cellSize - boardConfig.cellSize / 2;
            
            if(isEvenHeight) startY = (startRow - halfHeight + 0.5f) * boardConfig.cellSize;
            else startY = (startRow - halfHeight) * boardConfig.cellSize;
            
            float length = (endRow - startRow) * boardConfig.cellSize;
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
            Debug.Log($"{row} - {belowState}");
            int halfWidth = levelData.width / 2;
            int halfHeight = levelData.height / 2;
            
            bool isEvenWidth = (levelData.width & 1) == 0;
            bool isEvenHeight = (levelData.height & 1) == 0;

            float startX, y;
            
            if(isEvenWidth) startX = (startCol - halfWidth + 0.5f) * boardConfig.cellSize;
            else startX = (startCol - halfWidth) * boardConfig.cellSize;
            
            if(isEvenHeight) y = (row - halfHeight + 0.5f) * boardConfig.cellSize - boardConfig.cellSize / 2;
            else y = (row - halfHeight) * boardConfig.cellSize - boardConfig.cellSize / 2;
            
            float lengthX = (endCol - startCol) * boardConfig.cellSize;

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
            corner.position = new Vector3(posX - boardConfig.cellSize / 2, posY  + boardConfig.cellSize / 2, 0f);
            corner.eulerAngles = new Vector3(0, 0, 270f);
#if UNITY_EDITOR
            corner.name = "top_left_corner";
#endif
        }

        private void SetupTopRightCorner(float posX, float posY, bool state)
        {
            Transform corner = ObjectPooler.GetFromPool<Transform>(state ? PoolingType.OuterCorner : PoolingType.InnerCorner, selfTransform);
            corner.position = new Vector3(posX + boardConfig.cellSize / 2, posY  + boardConfig.cellSize / 2, 0f);
            corner.eulerAngles = new Vector3(0, 0, 180f);
#if UNITY_EDITOR
            corner.name = $"top_right_corner";
#endif
        }

        private void SetupBottomRightCorner(float posX, float posY, bool state)
        {
            Transform corner = ObjectPooler.GetFromPool<Transform>(state ? PoolingType.OuterCorner : PoolingType.InnerCorner, selfTransform);
            corner.position = new Vector3(posX + boardConfig.cellSize / 2, posY - boardConfig.cellSize / 2, 0f);
            corner.eulerAngles = new Vector3(0, 0, 90f);
#if UNITY_EDITOR
            corner.name = "bottom_right_corner";
#endif
        }

        private void SetupBottomLeftCorner(float posX, float posY, bool state)
        {
            Transform corner = ObjectPooler.GetFromPool<Transform>(state ? PoolingType.OuterCorner : PoolingType.InnerCorner, selfTransform);
            corner.position = new Vector3(posX - boardConfig.cellSize / 2, posY - boardConfig.cellSize / 2, 0f);
            corner.eulerAngles = Vector3.zero;
#if UNITY_EDITOR
            corner.name = "bottom_left_corner";
#endif
        }
    } 
}