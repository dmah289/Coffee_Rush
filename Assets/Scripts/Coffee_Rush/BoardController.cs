using System;
using Framework.ObjectPooling;
using Unity.Mathematics;
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
        TopBorder = 5,
        BottomBorder = 6,
        LeftBorder = 7,
        RightBorder = 8,
    }
    
    public class BoardController : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        
        [Header("Board Components")]
        [SerializeField] private Transform[][] cells;
        
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
            
            if (row == height - 1) return CellPosType.TopBorder;
            if (row == 0) return CellPosType.BottomBorder;
            if (col == 0) return CellPosType.LeftBorder;
            if (col == width - 1) return CellPosType.RightBorder;

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
                    
                    cell.name = $"Cell_{i}_{j}";
                    
                    cells[i][j] = cell;

                    CellPosType type = GetCellPosType(i, j);
                    Transform border, corner;
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
            
            SetupRightBorder();
            SetupLeftBorder();
            SetupBottomBorder();
            SetupTopBorder();
        }

        private void SetupTopLeftCorner(float posX, float posY)
        {
            Transform corner;
            corner = ObjectPooler.GetFromPool<Transform>(PoolingType.OuterCorner, selfTransform);
            corner.position = new Vector3(posX - cellSize / 2, posY  + cellSize / 2, 0f);
            corner.eulerAngles = new Vector3(0, 0, 270f);
        }

        private void SetupTopRightCorner(float posX, float posY)
        {
            Transform corner;
            corner = ObjectPooler.GetFromPool<Transform>(PoolingType.OuterCorner, selfTransform);
            corner.position = new Vector3(posX + cellSize / 2, posY  + cellSize / 2, 0f);
            corner.eulerAngles = new Vector3(0, 0, 180f);
        }

        private void SetupBottomRightCorner(float posX, float posY)
        {
            Transform corner;
            corner= ObjectPooler.GetFromPool<Transform>(PoolingType.OuterCorner, selfTransform);
            corner.position = new Vector3(posX + cellSize / 2, posY - cellSize / 2, 0f);
            corner.eulerAngles = new Vector3(0, 0, 90f);
        }

        private void SetupBottomLeftCorner(float posX, float posY)
        {
            Transform corner;
            corner = ObjectPooler.GetFromPool<Transform>(PoolingType.OuterCorner, selfTransform);
            corner.position = new Vector3(posX - cellSize / 2, posY - cellSize / 2, 0f);
            corner.eulerAngles = Vector3.zero;
        }

        private void SetupRightBorder()
        {
            Transform border = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            border.position = new Vector3(cellSize * width / 2, -cellSize * width / 2, 0);
            border.eulerAngles = new float3(0, 0, 90);
            border.localScale = new Vector3(cellSize * (height - 1), 1f, 1f);
        }

        private void SetupLeftBorder()
        {
            Transform border = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            border.position = new Vector3(-cellSize * width / 2 - borderSize, -cellSize * width / 2, 0);
            border.eulerAngles = new float3(0, 0, 90);
            border.localScale = new Vector3(cellSize * (height - 1), 1f, 1f);
        }

        private void SetupBottomBorder()
        {
            Transform border = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            border.position = new Vector3((1-width) * cellSize / 2, -cellSize * height / 2 , 0);
            border.eulerAngles = Vector3.zero;
            border.localScale = new Vector3(cellSize * (width - 1), 1f, 1f);
        }

        private void SetupTopBorder()
        {
            Transform border = ObjectPooler.GetFromPool<Transform>(PoolingType.StraightBorder, selfTransform);
            border.position = new Vector3((1-width) * cellSize / 2, cellSize * height / 2 + borderSize, 0);
            border.eulerAngles = float3.zero;
            border.localScale = new Vector3(cellSize * (width - 1), 1f, 1f);
        }
    } 
}