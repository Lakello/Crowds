namespace _SOURCE_.Scripts.ECS.Components
{
    using Game.Data;
    using UnityEngine;

    public struct MoveComponent
    {
        public Vector2 Input;
        public Transform Target;
        public MovementType MovementType;
    }
}