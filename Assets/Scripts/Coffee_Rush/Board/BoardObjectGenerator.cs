using System;
using System.Collections;
using Coffee_Rush.Block;
using Coffee_Rush.Level;
using Coffee_Rush.Obstacles;
using Cysharp.Threading.Tasks;
using Framework.ObjectPooling;
using UnityEngine;

namespace Coffee_Rush.Board
{
    public class BoardObjectSpawner : MonoBehaviour
    {
        [Header("Manager")]
        [SerializeField] private BlockController[] blocks;
        [SerializeField] private GateController[] gates;
        [SerializeField] private KettleController[] kettles;
        [SerializeField] private BlockerController[] blockers;

        private int blockCount;

        public int BlockCount
        {
            get => blockCount;
            set
            {
                blockCount = value;
                if (blockCount == 0) LevelManager.Instance.WinLevel();
            }
        }
        
        public async UniTask SpawnObjects(LevelData levelData, Tile[,] tiles)
        {
            SpawnBlocks(levelData.blocksData, tiles);
            SpawnGates(levelData.gatesData, tiles);
            SpawnKettles(levelData.kettlesData, tiles);
            SpawnBlockers(levelData.blockersData, tiles);
            await UniTask.DelayFrame(3);
        }

        private void SpawnBlockers(BlockerData[] blockersData, Tile[,] tiles)
        {
            if(blockersData.Length > 0) 
                blockers = new BlockerController[blockersData.Length];

            for (int i = 0; i < blockersData.Length; i++)
            {
                BlockerController blocker = ObjectPooler.GetFromPool<BlockerController>(
                    (PoolingType)(blockersData[i].blockerType + (byte)PoolingType.BlockerType00 - 1));
                
                blocker.Setup(tiles[blockersData[i].row, blockersData[i].col].transform.position, 
                    blockersData[i].movementDirection);
                
                blockers[i] = blocker;
            }
        }

        private void SpawnKettles(KettleData[] kettlesData, Tile[,] tiles)
        {
            if (kettlesData.Length == 0)
                return;
            
            kettles = new KettleController[kettlesData.Length];
            for (int i = 0; i < kettlesData.Length; i++)
            {
                KettleController kettle = ObjectPooler.GetFromPool<KettleController>(PoolingType.Kettle);
                kettle.Setup(tiles[kettlesData[i].row, kettlesData[i].col].transform.position, kettlesData[i].countdown);
                
                kettles[i] = kettle;
            }
        }

        private void SpawnGates(GateData[] gatesData, Tile[,] tiles)
        {
            gates = new GateController[gatesData.Length];
            for (int i = 0; i < gatesData.Length; i++)
            {
                GateController gate = ObjectPooler.GetFromPool<GateController>(PoolingType.Gate);
                gate.Setup(tiles[gatesData[i].row, gatesData[i].col].transform.position,
                           gatesData[i].gateDir,
                           gatesData[i].itemColors,
                           gatesData[i].compressedItemPath);
                
                gates[i] = gate;
            }
        }

        private void SpawnBlocks(BlockData[] blocksData, Tile[,] tiles)
        {
            BlockCount = blocksData.Length;
            blocks = new BlockController[blockCount];
            for (int i = 0; i < blocksData.Length; i++)
            {
                BlockController block = ObjectPooler.GetFromPool<BlockController>
                    ((PoolingType)(blocksData[i].blockType + (byte)PoolingType.BlockType00 - 1));
                
                block.SetupOnLevelStarted(tiles[blocksData[i].row, blocksData[i].col].transform.position,
                    blocksData[i]);
                
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
            
            for (int i = 0; i < kettles.Length; i++)
            {
                kettles[i].OnRevokenToPool();
            }
            
            for(int i = 0; i < blockers.Length; i++)
                blockers[i].OnRevoke();
        }
    }
}