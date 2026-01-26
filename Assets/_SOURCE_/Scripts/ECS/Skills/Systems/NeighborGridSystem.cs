namespace ECS.Skills.Systems
{
	using Aspects;
	using Components.Game;
	using Leopotam.EcsProto;
	using Leopotam.EcsProto.QoL;

	public class NeighborGridSystem : IProtoRunSystem
	{
		[DI] private readonly GameAspect _gameAspect;
		
		[DI] private readonly NeighborGridXZ _grid;

		[DI] private readonly ProtoIt _characterIt = new ProtoIt(It.Inc<CharacterComponent>());
		
		public void Run()
		{
			_grid.Clear();
			
			foreach (ProtoEntity entity in _characterIt)
			{
				ProtoPackedEntity packedEntity = _gameAspect.World().PackEntity(entity);
				ref CharacterComponent characterComponent = ref _gameAspect.CharacterPool.Get(entity);
				
				_grid.Add(packedEntity, characterComponent.Position);
			}
		}
	}
}