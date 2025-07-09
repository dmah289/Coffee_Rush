using System;
using UnityEngine;

namespace Coffee_Rush.Board
{
    public class BoardController : MonoBehaviour
    {
        [Header("Self References")]
        [SerializeField] private BoardLayoutGenerator layoutGenerator;

        public void EnterLevel()
        {
            layoutGenerator.SetupBoard();
        }
    }
}