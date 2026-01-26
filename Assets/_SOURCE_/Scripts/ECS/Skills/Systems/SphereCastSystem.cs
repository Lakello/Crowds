namespace ECS.Skills.Systems
{
	using Aspects;
	using Components.Game;
	using Leopotam.EcsProto;
	using Leopotam.EcsProto.QoL;

	public class SphereCastSystem : IProtoRunSystem
	{
		[DI] private readonly GameAspect _gameAspect;

		[DI] private readonly NeighborGridXZ _grid;

		[DI] private readonly ProtoIt _castIt = new ProtoIt(It.Inc<CharacterComponent, SphereCastDetector>());
		
		public void Run()
		{
			foreach (var castEntity in _castIt)
			{
				ref var character = ref _gameAspect.CharacterPool.Get(castEntity);
				ref var detector = ref _gameAspect.SphereCastPool.Get(castEntity);
				
				detector.Init();
				
				
			}
		}
	}
}