using System;
using UnityEngine;

namespace Coffee_Rush.Board
{
    public class Tile : MonoBehaviour
    {
        public Transform SelfTransform { get; private set; }

        private void Awake()
        {
            SelfTransform = transform;
        }
    }
}