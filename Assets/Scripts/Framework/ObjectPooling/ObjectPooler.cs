using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.ObjectPooling
{
    public static class ObjectPooler
    {
        private static int capacity = Enum.GetValues(typeof(PoolingType)).Length;

        private static List<Component>[] poolList = new List<Component>[capacity];

        private static readonly Transform poolParent = GameObject.FindGameObjectWithTag(KeySave.PoolParentName).transform;

        public static void SetUpPool<T>(PoolingType type, int poolSize, T itemPrefab) where T : Component
        {
            if (poolSize <= 1)
                throw new ArgumentException("Pool size must be greater than 1.");
            
            poolList[(byte)type] = new List<Component>(poolSize);
            for (int i = 0; i < poolSize; i++)
            {
                T instance = GameObject.Instantiate(itemPrefab);
                ReturnToPool(type, instance);
            }
        }

        public static T GetFromPool<T>(PoolingType type, Transform parent = null) where T : Component
        {
            if (poolList[(byte)type].Count > 1)
            {
                T instance = (T)poolList[(byte)type][poolList[(byte)type].Count - 1];
                poolList[(byte)type].RemoveAt(poolList[(byte)type].Count - 1);
                instance.transform.SetParent(parent);
                instance.gameObject.SetActive(true);
                return instance;
            }
            
            T newInstance = GameObject.Instantiate((T)poolList[(byte)type][0]);
            newInstance.gameObject.SetActive(true);
            return newInstance;
        }

        private static void ReturnToPool<T>(PoolingType type, T instance) where T : Component
        {
            PreprocessData(instance);
            poolList[(byte)type].Add(instance);
        }

        private static void PreprocessData<T>(T instance) where T : Component
        {
            instance.transform.SetParent(poolParent);
            instance.gameObject.SetActive(false);
        }
    }
}