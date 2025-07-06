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
    
    public class BoardController : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        
        [Header("Board Manager")]
        private Transform[][] cells;
        
        [Header("Board Configuration")]
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private float cellSize;
        [SerializeField] private float borderSize;

        private CellPosType GetCellPosType(int row, int col)
        {
            if (row == 0 && col == 0)
                return CellPosType.BottomLeftCorner;
            if (row == 0 && col == width - 1)
                return CellPosType.BottomRightCorner;
            if (row == height - 1 && col == width - 1)
                return CellPosType.TopRightCorner;
            if (row == height - 1 && col == 0)
                return CellPosType.TopLeftCorner;

            return CellPosType.Inner;
        }
        
        

        private void Awake()
        {
            cells = new Transform[height][];
            selfTransform = transform;
        }

        private void Start()
        {
            SetupBoard();
        }

        private void SetupBoard()
        {
            int halfWidth = width / 2;
            int halfHeight = height / 2;
            
            bool isEvenWidth = (width & 1) == 0;
            bool isEvenHeight = (height & 1) == 0;
            
            float posX, posY;
            
            for (int i = 0; i < height; i++)
            {
                cells[i] = new Transform[width];
                for (int j = 0; j < width; j++)
                {
                    Transform cell = ObjectPooler.GetFromPool<Transform>(PoolingType.Cell, selfTransform);
                    
                    if (isEvenWidth) posX = (j - halfWidth + 0.5f) * cellSize;
                    else posX = (j - halfWidth) * cellSize;

                    if (isEvenHeight) posY = (i - halfHeight + 0.5f) * cellSize;
                    else posY = (i - halfHeight) * cellSize;
                    
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
            corner.position = new Vector3(posX - cellSize / 2, posY  + cellSize / 2, 0f);
            corner.eulerAngles = new Vector3(0, 0, 270f);
#if UNITY_EDITOR
            corner.name = "top_left_corner";
#endif
        }

        private void SetupTopRightCorner(float posX, float posY)
        {
            Transform corner = ObjectPooler.GetFromPool<Transform>(PoolingType.OuterCorner, selfTransform);
            corner.position = new Vector3(posX + cellSize / 2, posY  + cellSize / 2, 0f);
            corner.eulerAngles = new Vector3(0, 0, 180f);
#if UNITY_EDITOR
            corner.name = $"top_right_corner";
#endif
        }

        private void SetupBottomRightCorner(float posX, float posY)
        {
            Transform corner= ObjectPooler.GetFromPool<Transform>(PoolingType.OuterCorner, selfTransform);
            corner.position = new Vector3(posX + cellSize / 2, posY - cellSize / 2, 0f);
            corner.eulerAngles = new Vector3(0, 0, 90f);
#if UNITY_EDITOR
            corner.name = "bottom_right_corner";
#endif
        }

        private void SetupBottomLeftCorner(float posX, float posY)
        {
            Transform corner = ObjectPooler.GetFromPool<Transform>(PoolingType.OuterCorner, selfTransform);
            corner.position = new Vector3(posX - cellSize / 2, posY - cellSize / 2, 0f);
            corner.eulerAngles = Vector3.zero;
#if UNITY_EDITOR
            corner.name = "bottom_left_corner";
#endif
        }

        private void SetupVerticalBorder()
        {
            Vector3 verticalEulerAngles = new Vector3(0, 0, 90);
            Vector3 verticalLocalScale = new Vector3(cellSize * (height - 1), 1f, 1f);
            
            Transform rightBorder = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            rightBorder.position = new Vector3(cellSize * width / 2 + borderSize / 2, 0, 0);
            rightBorder.eulerAngles = verticalEulerAngles;
            rightBorder.localScale = verticalLocalScale;
#if UNITY_EDITOR
            rightBorder.name = "right_border";
#endif
            
            Transform leftBorder = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            leftBorder.position = new Vector3(-cellSize * width / 2 - borderSize / 2, 0, 0);
            leftBorder.eulerAngles = verticalEulerAngles;
            leftBorder.localScale = verticalLocalScale;
            
#if UNITY_EDITOR
            leftBorder.name = "left_border";
#endif
        }

        private void SetupHorizontalBorder()
        {
            Vector3 horizontalLocalScale = new Vector3(cellSize * (width - 1), 1f, 1f);
            
            Transform bottomBorder = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            bottomBorder.position = new Vector3(0, -cellSize * height / 2 - borderSize / 2, 0);
            bottomBorder.eulerAngles = Vector3.zero;
            bottomBorder.localScale = horizontalLocalScale;
#if UNITY_EDITOR
            bottomBorder.name = "bottom_border";
#endif
            
            Transform topBorder = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            topBorder.position = new Vector3(0, cellSize * height / 2 + borderSize / 2, 0);
            topBorder.eulerAngles = Vector3.zero;
            topBorder.localScale = horizontalLocalScale;
#if UNITY_EDITOR
            topBorder.name = "top_border";
#endif
        }
    } 
}