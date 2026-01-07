namespace _SOURCE_.Scripts.Game.Data
{
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using Sirenix.Serialization;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Move Data", menuName = "Data/Move Data")]
    public class MovementDataHolder : SerializedScriptableObject
    {
        [OdinSerialize] private Dictionary<MovementType, MovementData> _data;

        public MovementData GetData(MovementType movementType)
        {
            return _data[movementType];
        }
    }
}