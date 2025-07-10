using System;
using Coffee_Rush.Level;
using UnityEngine;

namespace Coffee_Rush.Board
{
    public class BoardController : MonoBehaviour
    {
        [Header("Self References")]
        [SerializeField] private BoardLayoutGenerator layoutGenerator;

        public void EnterLevel(LevelData levelData)
        {
            layoutGenerator.SetupBoard(levelData);
        }
    }
}