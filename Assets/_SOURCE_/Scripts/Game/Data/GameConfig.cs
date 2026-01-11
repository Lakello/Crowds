namespace Game.Data
{
    using CharacterSystem.Data;
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameConfig", menuName = "Data/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [field: SerializeField] public PrefabsHolder PrefabsHolder { get; private set; }
        [field: SerializeField] public CharacterDataHolder CharacterDataHolder { get; private set; }
        [field: SerializeField] public HealthBarDataHolder HealthBarDataHolder { get; private set; }
    }
}