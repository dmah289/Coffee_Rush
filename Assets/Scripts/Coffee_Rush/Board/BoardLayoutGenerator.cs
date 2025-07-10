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
        BottomLeft = 1 << 3,
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
            
            Debug.Log(1 << 1);
        }

        private bool IsInBound(int row, int col, int width, int height)
        {
            return row >= 0 && row < height && col >= 0 && col < width;
        }
        
        // TODO : Refractor logic
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
                    byte typeByte = (byte)type;
                    string binaryRepresentation = Convert.ToString(typeByte, 2).PadLeft(8, '0');
                    Debug.Log($"Cell ({i}, {j}) - Type: {type} - Binary: {binaryRepresentation}");
                    
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
        }

        // TODO : Refractor to use a single method with parameters for position and rotation
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

//         private void SetupVerticalBorder()
//         {
//             Vector3 verticalEulerAngles = new Vector3(0, 0, 90);
//             Vector3 verticalLocalScale = new Vector3(boardConfig.cellSize * (levelData.height - 1), 1f, 1f);
//             
//             Transform rightBorder = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
//             rightBorder.position = new Vector3(boardConfig.cellSize * levelData.width / 2 + boardConfig.borderSize / 2, 0, 0);
//             rightBorder.eulerAngles = verticalEulerAngles;
//             rightBorder.localScale = verticalLocalScale;
// #if UNITY_EDITOR
//             rightBorder.name = "right_border";
// #endif
//             
//             Transform leftBorder = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
//             leftBorder.position = new Vector3(-boardConfig.cellSize * levelData.width / 2 - boardConfig.borderSize / 2, 0, 0);
//             leftBorder.eulerAngles = verticalEulerAngles;
//             leftBorder.localScale = verticalLocalScale;
//             
// #if UNITY_EDITOR
//             leftBorder.name = "left_border";
// #endif
//         }
//
//         private void SetupHorizontalBorder()
//         {
//             Vector3 horizontalLocalScale = new Vector3(boardConfig.cellSize * (levelData.width - 1), 1f, 1f);
//             
//             Transform bottomBorder = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
//             bottomBorder.position = new Vector3(0, -boardConfig.cellSize * levelData.height / 2 - boardConfig.borderSize / 2, 0);
//             bottomBorder.eulerAngles = Vector3.zero;
//             bottomBorder.localScale = horizontalLocalScale;
// #if UNITY_EDITOR
//             bottomBorder.name = "bottom_border";
// #endif
//             
//             Transform topBorder = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
//             topBorder.position = new Vector3(0, boardConfig.cellSize * levelData.height / 2 + boardConfig.borderSize / 2, 0);
//             topBorder.eulerAngles = Vector3.zero;
//             topBorder.localScale = horizontalLocalScale;
// #if UNITY_EDITOR
//             topBorder.name = "top_border";
// #endif
//         }
    } 
}