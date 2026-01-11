namespace Game.Data
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Health Bar Data Holder", menuName = "Data/Health Bar Data Holder")]
    public class HealthBarDataHolder : ScriptableObject
    {
        [BoxGroup("Bar Settings")]
        [field: SerializeField] public Vector2 BarSize { get; private set; } = new Vector2(60, 8);
        [field: SerializeField] public float OffsetY { get; private set; } = 0f;
        [field: SerializeField] public Color VertexTint { get; private set; } = Color.white;

        [Header("Culling")]
        [field: SerializeField] public float MaxDistance { get; private set; } = 60f;
    }
}