namespace ECS.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Game.CharacterSystem;
    using Game.Data;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using UnityEngine;

    public class CharacterPoolService : IProtoInitService
    {
        public Dictionary<UnitType, Queue<Character>> Pool { get; private set; } = new Dictionary<UnitType, Queue<Character>>();

        public void Init(IProtoSystems systems)
        {
            Debug.Log("CharacterPoolService::Init");
            var types = Enum.GetValues(typeof(UnitType)).Cast<UnitType>();

            foreach (var unitType in types)
            {
                Pool.Add(unitType, new Queue<Character>());
            }
        }
    }
}