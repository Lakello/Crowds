namespace Game.Data
{
    using CharacterSystem;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Prefabs Holder", menuName = "Data/Prefabs Holder")]
    public class PrefabsHolder : ScriptableObject
    {
        [BoxGroup("Player")]
        [field: SerializeField] public Character PlayerPrefab { get; private set; }
    }
}