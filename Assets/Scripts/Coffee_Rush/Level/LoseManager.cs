using UnityEngine;

namespace Coffee_Rush.Level
{
    [RequireComponent(typeof(LevelManager))]
    public class LoseManager : MonoBehaviour
    {
        public void FailLevel()
        {
            print("Level Failed");
        }
    }
}