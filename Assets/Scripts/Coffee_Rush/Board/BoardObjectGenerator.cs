using System.Collections;
using Coffee_Rush.Block;
using Coffee_Rush.Level;
using Framework.ObjectPooling;
using UnityEngine;

namespace Coffee_Rush.Board
{
    public class BoardObjectSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform blocksParent;
        [SerializeField] private Transform gatesParent;
        
        [SerializeField] private byte poolingTypeOffset = 5; // Offset for block types in PoolingType and eBlockType
        
        public IEnumerator SpawnObjects(LevelData levelData, Tile[,] tiles)
        {
            SpawnBlocks(levelData.blocksData, tiles);
            SpawnGates(levelData.gatesData, tiles);
            yield return null;
        }

        private void SpawnGates(GateData[] gatesData, Tile[,] tiles)
        {
            for (int i = 0; i < gatesData.Length; i++)
            {
                GateController gate = ObjectPooler.GetFromPool<GateController>(PoolingType.Gate, gatesParent);
                gate.Setup(tiles[gatesData[i].row, gatesData[i].col].transform.position,
                           gatesData[i].gateDir,
                           gatesData[i].itemColors);
            }
        }

        private void SpawnBlocks(BlockData[] blocksData, Tile[,] tiles)
        {
            for (int i = 0; i < blocksData.Length; i++)
            {
                BlockController block = ObjectPooler.GetFromPool<BlockController>((PoolingType)(blocksData[i].blockType + poolingTypeOffset), blocksParent);
                block.SetCheckPointToTargetTile(tiles[blocksData[i].row, blocksData[i].col].transform.position);
                block.ColorType = blocksData[i].blockColor;
            }
        }
    }
}