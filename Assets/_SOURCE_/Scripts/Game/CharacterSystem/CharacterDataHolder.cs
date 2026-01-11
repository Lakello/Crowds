namespace Game.CharacterSystem.Data
{
    using System;
    using System.Collections.Generic;
    using Game.Data;
    using Sirenix.OdinInspector;
    using Sirenix.Serialization;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Move Data", menuName = "Data/Move Data")]
    public class CharacterDataHolder : SerializedScriptableObject
    {
        [OdinSerialize] private Dictionary<UnitType, CharacterData> _data;

        public CharacterData GetData(UnitType unitType)
        {
            return _data[unitType];
        }
    }

    [Serializable]
    public struct CharacterData
    {
        public MovementData MovementData;
        public HealthData HealthData;
    }

    [Serializable]
    public struct MovementData
    {
        public float MoveSpeed;
        public float RotateSpeed;
    }
    
    [Serializable]
    public struct HealthData
    {
        public float MaxHealth;
        public float HeightWorldOffset;
    }
}