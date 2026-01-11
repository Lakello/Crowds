namespace UtilsModule.Other
{
    using System;
    using System.Collections.Generic;
    using Execute.Mode;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UtilsModule.Execute;
    using UtilsModule.Execute.Interfaces;
    using ZLinq;

    public abstract class ParentHolder<TKey> : MonoBehaviour, IExecuteHolder
        where TKey : Enum
    {
        [SerializeField]
        private Data[] _data;

        private static Dictionary<TKey, Transform> _parents;

        public ExecuteMethod Method => ExecuteMethod.Awake;
        public int Priority { get; set; }

        public static Transform GetParent(TKey type)
        {
            if (_parents == null)
            {
                throw new Exception($"ParentHolder {typeof(TKey).Name} not initialized");
            }

            if (_parents.TryGetValue(type, out Transform result))
            {
                return result;
            }

            throw new ArgumentException(type.ToString(), nameof(type));
        }

        public static bool TryGetParent(TKey type, out Transform parent)
        {
            parent = null;

            if (_parents == null)
            {
                return false;
            }

            if (_parents.TryGetValue(type, out Transform result))
            {
                parent = result;
                return true;
            }

            return false;
        }

        public Executor GetExecutor()
        {
            return new ExecutorSync(Init);
        }

        private void Init()
        {
            _parents = _data.AsValueEnumerable().ToDictionary(d => d.Type, d => d.Parent);
        }

        private void OnDestroy()
        {
            _parents = null;
        }

        [Serializable]
        private struct Data
        {
            [HorizontalGroup]
            [HideLabel]
            public TKey Type;
            [HorizontalGroup]
            [HideLabel]
            public Transform Parent;
        }
    }
}