namespace ECS
{
	using System.Collections.Generic;
	using Game.CharacterSystem;
	using Leopotam.EcsProto.QoL;
	using UtilsModule.Singleton;

	public class CharacterLinker : Singleton<CharacterLinker>
	{
		private Dictionary<Character, ProtoPackedEntity> _characterToEntity = new Dictionary<Character, ProtoPackedEntity>();
		private Dictionary<ProtoPackedEntity, Character> _entityToCharacter = new Dictionary<ProtoPackedEntity, Character>();

		public void Register(Character character, ProtoPackedEntity entity)
		{
			_characterToEntity[character] = entity;
			_entityToCharacter[entity] = character;
		}

		public void Unregister(Character character, ProtoPackedEntity entity)
		{
			_characterToEntity.Remove(character);
			_entityToCharacter.Remove(entity);
		}

		public Character Get(ProtoPackedEntity entity)
		{
			return _entityToCharacter.GetValueOrDefault(entity, null);
		}
		
		public ProtoPackedEntity Get(Character character)
		{
			return _characterToEntity.GetValueOrDefault(character);
		}
	}

	public static class CharacterLinkerExtensions
	{
		public static ProtoPackedEntity GetEntity(this Character character)
		{
			return CharacterLinker.Instance.Get(character);
		}
		
		public static Character GetCharacter(this ProtoPackedEntity entity)
		{
			return CharacterLinker.Instance.Get(entity);
		}
		
		public static void Register(this ProtoPackedEntity entity, Character character)
		{
			CharacterLinker.Instance.Register(character, entity);
		}
		
		public static void Register(this Character character, ProtoPackedEntity entity)
		{
			CharacterLinker.Instance.Register(character, entity);
		}
		
		public static void Unregister(this ProtoPackedEntity entity, Character character)
		{
			CharacterLinker.Instance.Unregister(character, entity);
		}
		
		public static void Unregister(this Character character, ProtoPackedEntity entity)
		{
			CharacterLinker.Instance.Unregister(character, entity);
		}
	}
}