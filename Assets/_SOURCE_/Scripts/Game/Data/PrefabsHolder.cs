namespace _SOURCE_.Scripts.Game.Data
{
    using CharacterSystem;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Prefabs Holder", menuName = "Data/Prefabs Holder")]
    public class PrefabsHolder : ScriptableObject
    {
        [field: SerializeField] public Character PlayerPrefab { get; private set; }
    }
}