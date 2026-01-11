namespace Game.Data
{
    using System;
    using UnityEngine;
    using UtilsModule.Singleton;

    public class SpawnPoint : MonoBehaviour, ISingleAccess
    {
        public Type TargetType => typeof(SpawnPoint);
    }
}