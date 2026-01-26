namespace ECS.Skills
{
	using System.Collections.Generic;
	using Components.Game;
	using Leopotam.EcsProto;
	using Leopotam.EcsProto.QoL;
	using UnityEngine;

	public sealed class NeighborGridXZ
	{
		private readonly float _cellSize;
		private readonly Dictionary<int, List<ProtoPackedEntity>> _cells;

		public NeighborGridXZ(float cellSize, int capacity = 1024)
		{
			_cellSize = Mathf.Max(0.0001f, cellSize);
			_cells = new Dictionary<int, List<ProtoPackedEntity>>(capacity);
		}

		public void Clear()
		{
			foreach (var kv in _cells)
			{
				kv.Value.Clear();
			}
		}

		public void Add(in ProtoPackedEntity e, in Vector3 pos)
		{
			GetCell(pos, out int cx, out int cz);
			int h = Hash(cx, cz);

			if (!_cells.TryGetValue(h, out var list))
			{
				list = new List<ProtoPackedEntity>(8);
				_cells.Add(h, list);
			}
			list.Add(e);
		}

		public bool FindNearestInRadius(
			ProtoWorld world,
			in ProtoPackedEntity self,
			in Vector3 selfPos,
			float radius,
			ProtoPool<CharacterComponent> characterPool,
			out ProtoPackedEntity nearest)
		{
			nearest = default;

			float r2 = radius * radius;
			float bestD2 = r2;
			bool found = false;

			GetCell(selfPos, out int cx, out int cz);

			int range = Mathf.CeilToInt(radius / _cellSize);

			for (int dz = -range; dz <= range; dz++)
			for (int dx = -range; dx <= range; dx++)
			{
				int h = Hash(cx + dx, cz + dz);

				if (!_cells.TryGetValue(h, out var list) || list.Count == 0)
					continue;

				for (int i = 0; i < list.Count; i++)
				{
					var other = list[i];
					if (other == self) continue;

					if (!other.TryUnpack(world, out ProtoEntity otherEntity))
						continue;

					Vector3 p = characterPool.Get(otherEntity).Position;

					float ddx = p.x - selfPos.x;
					float ddz = p.z - selfPos.z;
					float d2 = ddx * ddx + ddz * ddz;

					if (d2 < bestD2)
					{
						bestD2 = d2;
						nearest = other;
						found = true;
					}
				}
			}

			return found;
		}

		public int FindAllInRadius(
			ProtoWorld world,
			in ProtoPackedEntity self,
			in Vector3 selfPos,
			float radius,
			ProtoPool<CharacterComponent> characterPool,
			List<ProtoPackedEntity> results,
			bool clearResults = true)
		{
			if (clearResults) results.Clear();

			float r2 = radius * radius;

			GetCell(selfPos, out int cx, out int cz);

			int range = Mathf.CeilToInt(radius / _cellSize);

			for (int dz = -range; dz <= range; dz++)
			for (int dx = -range; dx <= range; dx++)
			{
				int h = Hash(cx + dx, cz + dz);

				if (!_cells.TryGetValue(h, out var list) || list.Count == 0)
					continue;

				for (int i = 0; i < list.Count; i++)
				{
					var other = list[i];
					if (other == self) continue;

					if (!other.TryUnpack(world, out ProtoEntity otherEntity))
						continue;

					Vector3 p = characterPool.Get(otherEntity).Position;

					float ddx = p.x - selfPos.x;
					float ddz = p.z - selfPos.z;
					float d2 = ddx * ddx + ddz * ddz;

					if (d2 <= r2)
						results.Add(other);
				}
			}

			return results.Count;
		}

		private static int Hash(int cx, int cz)
		{
			unchecked { return (cx * 73856093) ^ (cz * 19349663); }
		}

		private void GetCell(Vector3 pos, out int cx, out int cz)
		{
			cx = Mathf.FloorToInt(pos.x / _cellSize);
			cz = Mathf.FloorToInt(pos.z / _cellSize);
		}
	}
}