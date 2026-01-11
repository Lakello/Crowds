namespace Game.CharacterSystem
{
    using Data;
    using Game.Data;
    using UnityEngine;

    public class Character : MonoBehaviour
    {
        [field: SerializeField] public UnitType UnitType { get; private set; }
    }
}