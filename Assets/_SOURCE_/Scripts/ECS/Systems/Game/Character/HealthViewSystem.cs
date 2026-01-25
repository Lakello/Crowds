namespace ECS.Systems.Game
{
    using Aspects;
    using Components.Game;
    using global::Game.Data;
    using global::Game.UI;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Leopotam.EcsProto.Unity;
    using UnityEngine;

    public class HealthViewSystem : IProtoRunSystem
    {
        [DIUnity("Camera")] private readonly Camera _camera;
        [DIUnity("HealthBars")] private readonly BatchedHealthBarsGraphic _graphic;

        [DI] private GameAspect _gameAspect;

        [DI] private HealthBarDataHolder _dataHolder;

        [DI] private ProtoIt _healthIt = new ProtoIt(It.Inc<CharacterComponent, HealthComponent>());

        public void Run()
        {
            if (!_camera || !_graphic)
            {
                return;
            }

            BuildBars();

            _graphic.SetDirty();
        }

        void BuildBars()
        {
            _graphic.Bars.Clear();

            Vector3 camPos = _camera.transform.position;
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_camera);

            foreach (ProtoEntity entity in _healthIt)
            {
                ref CharacterComponent characterComponent = ref _gameAspect.CharacterPool.Get(entity);
                ref HealthComponent healthComponent = ref _gameAspect.HealthPool.Get(entity);

                Vector3 worldPosition = characterComponent.Character.transform.position + Vector3.up * healthComponent.HeightWorldOffset;

                float dist = Vector3.Distance(camPos, worldPosition);
                if (dist > _dataHolder.MaxDistance)
                {
                    continue;
                }

                Bounds bounds = new Bounds(worldPosition, Vector3.one * 0.5f);
                if (GeometryUtility.TestPlanesAABB(planes, bounds) == false)
                {
                    continue;
                }

                float hp = Mathf.Clamp01(healthComponent.NormalizedHealth);
                if (hp <= 0f)
                {
                    continue;
                }

                _graphic.Bars.Add(new BatchedHealthBarsGraphic.Bar
                {
                    WorldPosition = worldPosition,
                    NormalizedHealth = hp,
                    Size = _dataHolder.BarSize,
                    OffsetY = _dataHolder.OffsetY,
                    BackgroundColor = _dataHolder.VertexTint,
                    FillColor = _dataHolder.VertexTint
                });
            }
        }
    }
}