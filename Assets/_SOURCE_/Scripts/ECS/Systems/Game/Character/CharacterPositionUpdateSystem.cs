namespace ECS.Systems.Game
{
	using Aspects;
	using Components.Game;
	using Leopotam.EcsProto;
	using Leopotam.EcsProto.QoL;

	public class CharacterPositionUpdateSystem : IProtoRunSystem
	{
		[DI] private readonly GameAspect _gameAspect;
		
		[DI] private readonly ProtoIt _characterIt = new ProtoIt(It.Inc<CharacterComponent>());
		
		public void Run()
		{
			var world = _gameAspect.World();
			
			foreach (var entity in _characterIt)
			{
				var packedEntity = world.PackEntity(entity);
				
				var linkCharacter = CharacterLinker.Instance.Get(packedEntity);

				if (linkCharacter == null)
				{
					continue;
				}

				ref var characterComponent = ref _gameAspect.CharacterPool.Get(entity);
				characterComponent.Position = linkCharacter.transform.position;
			}
		}
	}
}