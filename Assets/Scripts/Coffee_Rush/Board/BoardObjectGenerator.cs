using System.Collections;
using Coffee_Rush.Block;
using Coffee_Rush.Level;
using Coffee_Rush.Obstacles;
using Framework.ObjectPooling;
using UnityEngine;

namespace Coffee_Rush.Board
{
    public class BoardObjectSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform blocksParent;
        [SerializeField] private Transform gatesParent;
        
        [Header("Manager")]
        [SerializeField] private BlockController[] blocks;
        [SerializeField] private GateController[] gates;
        
        public IEnumerator SpawnObjects(LevelData levelData, Tile[,] tiles)
        {
            SpawnBlocks(levelData.blocksData, tiles);
            SpawnGates(levelData.gatesData, tiles);
            SpawnKettles(levelData.kettlesData, tiles);
            yield return WaitHelper.GetWaitForEndOfFrame();
        }

        private void SpawnKettles(KettleData[] kettlesData, Tile[,] tiles)
        {
            for (int i = 0; i < kettlesData.Length; i++)
            {
                KettleController kettle = ObjectPooler.GetFromPool<KettleController>(PoolingType.Kettle);
                kettle.Setup(tiles[kettlesData[i].row, kettlesData[i].col].transform.position, kettlesData[i].countdown);
            }
        }

        private void SpawnGates(GateData[] gatesData, Tile[,] tiles)
        {
            gates = new GateController[gatesData.Length];
            for (int i = 0; i < gatesData.Length; i++)
            {
                GateController gate = ObjectPooler.GetFromPool<GateController>(PoolingType.Gate, gatesParent);
                gate.Setup(tiles[gatesData[i].row, gatesData[i].col].transform.position,
                           gatesData[i].gateDir,
                           gatesData[i].itemColors,
                           gatesData[i].compressedItemPath);
                
                gates[i] = gate;
            }
        }

        private void SpawnBlocks(BlockData[] blocksData, Tile[,] tiles)
        {
            blocks = new BlockController[blocksData.Length];
            for (int i = 0; i < blocksData.Length; i++)
            {
                BlockController block = ObjectPooler.GetFromPool<BlockController>
                    ((PoolingType)(blocksData[i].blockType + (byte)PoolingType.BlockType00 - 1), blocksParent);
                block.SetCheckPointToTargetTile(tiles[blocksData[i].row, blocksData[i].col].transform.position);
                block.SetMovementDirection(blocksData[i].moveableDir);
                block.SetBlockObstacle(blocksData[i].countdownIce);
                block.ColorType = blocksData[i].blockColor;
                block.BlockType = blocksData[i].blockType;
                
                blocks[i] = block;
            }
        }


        public void RevokeObjects()
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].OnRevokenToPool();
            }
            
            for(int i = 0; i < gates.Length; i++)
            {
                gates[i].OnRevokenToPool();
            }
        }
    }
}