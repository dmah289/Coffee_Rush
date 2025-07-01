using System;
using UnityEngine;

namespace Framework.ObjectPooling
{
    public class PoolingManager : MonoBehaviour
    {
        [Header("Board Prefabs")]
        [SerializeField] private Transform cellPrefab;
        [SerializeField] private Transform outerCornerPrefab;
        [SerializeField] private Transform straightBorderPrefab;

        private void Awake()
        {
            // Initialize the board items
            ObjectPooler.SetUpPool(PoolingType.Cell, 50, cellPrefab);
            ObjectPooler.SetUpPool(PoolingType.OuterCorner, 5, outerCornerPrefab);
            ObjectPooler.SetUpPool(PoolingType.StraightBorder, 20, straightBorderPrefab);
            
        }
    }
}