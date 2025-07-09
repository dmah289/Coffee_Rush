#if UNITY_EDITOR
using System;
using Coffee_Rush.Level;
using Coffee_Rush.LevelEditor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace LevelEditor.Scripts.RightSide
{
    public class FooterRightEditor : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private HeaderLeftEditor headerLeftEditor;
        
        public void OnExportLevelClicked()
        {
            if (!headerLeftEditor.IsLevelDataInitialized)
                throw new Exception("Level data has not been initialized yet. Please create a board first.");
            
            SaveAllData();
            SaveAsset();
        }
        
        private void SaveAllData()
        {
            for (int i = 0; i < headerLeftEditor.currLevelData.height; i++)
            {
                for (int j = 0; j < headerLeftEditor.currLevelData.width; j++)
                {
                    CellData cellData = new CellData
                    {
                        isActive = headerLeftEditor.cellsEdit[i,j].gameObject.activeSelf
                    };
                    headerLeftEditor.currLevelData.SetCellData(i, j, cellData);
                }
            }
        }
        
        private void SaveAsset()
        {
            string path = $"Assets/LevelData/{headerLeftEditor.currLevelData.levelIndex}.asset";
            EditorUtility.SetDirty(headerLeftEditor.currLevelData);
            AssetDatabase.CreateAsset(headerLeftEditor.currLevelData, path);
            AssetDatabase.SaveAssets();
            
            // Retrieve the main Addressable Asset Settings config object
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            string groupName = "LevelData";
            AddressableAssetGroup group = settings.FindGroup(groupName);
            
            // Get the unique ID used to track the asset
            string guid = AssetDatabase.AssetPathToGUID(path);
            
            // Create an entry for the asset by GUID and add it to the group 
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
            
            // Set the runtime address
            entry.address = $"Level {headerLeftEditor.currLevelData.levelIndex}";
            
            // Mark the settings as needing to be saved
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryAdded, entry, true);
        }
    }
}
#endif