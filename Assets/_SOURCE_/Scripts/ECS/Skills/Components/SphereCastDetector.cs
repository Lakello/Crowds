namespace ECS.Skills
{
	using Leopotam.EcsProto.QoL;
	using Unity.Collections;
	using UnityEngine;

	public struct SphereCastDetector
	{
		private bool _isInitialized;
		
		public SphereCastZone Base;

		public NativeList<SphereCastZone> InsideZones;

		public void Init()
		{
			if (_isInitialized)
			{
				return;
			}
			
			Base.Init();
			
			InsideZones = new NativeList<SphereCastZone>(Allocator.Persistent);
			
			_isInitialized = true;
		}
	}

	public struct SphereCastZone
	{
		private bool _isInitialized;
		
		public Vector3 Position;
		public float Radius;
		public NativeList<ProtoPackedEntity> Entities;

		public void Init()
		{
			if (_isInitialized)
			{
				return;
			}
			
			Entities = new NativeList<ProtoPackedEntity>(Allocator.Persistent);
			
			_isInitialized = true;
		}
	}
}

namespace ECS.Authoring.Helpers
{
	using Game.CharacterSystem;
	using Leopotam.EcsProto;
	using Skills;

	public static partial class CreateComponentHelper
	{
		public static ref SphereCastDetector CreateSphereCastDetector(
			this ProtoEntity entity,
			ProtoPool<SphereCastDetector> pool, 
			Character character)
		{
			ref SphereCastDetector characterComponent = ref pool.Add(entity);
            
			return ref characterComponent;
		}
	}
}