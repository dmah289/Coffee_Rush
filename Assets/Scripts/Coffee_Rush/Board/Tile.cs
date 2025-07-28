using System;
using UnityEngine;

namespace Coffee_Rush.Board
{
    public class Tile : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private GateController[] gates = new GateController[4];
        
        public Transform SelfTransform { get; private set; }

        public bool HasGate
        {
            get
            {
                for (int i = 0; i < gates.Length; i++)
                {
                    if (gates[i] != null)
                        return true;
                }

                return false;
            }
        }

        private void Awake()
        {
            SelfTransform = transform;
        }
        
        public void SetGate(GateController gate)
        {
            gates[(int)gate.GateDir - 1] = gate;
        }
    }
}