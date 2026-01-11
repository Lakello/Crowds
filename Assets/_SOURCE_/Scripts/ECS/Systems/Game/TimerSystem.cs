namespace ECS.Systems.Game
{
    using Aspects;
    using Authoring;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Unity.Mathematics;
    using UnityEngine;

    public class TimerSystem : IProtoRunSystem
    {
        [DI] private GameAspect _gameAspect;

        [DI] private ProtoIt _timerIt = new ProtoIt(new[]
        {
            typeof(TimerComponent)
        });

        public void Run()
        {
            foreach (var e in _timerIt)
            {
                ref var timer = ref _gameAspect.TimerPool.Get(e);

                if (timer.RemainingTime > 0)
                {
                    timer.RemainingTime = math.max(0, timer.RemainingTime - Time.deltaTime);

                    continue;
                }

                if (timer.RemainingTime == 0)
                {
                    timer.RemainingTime = timer.Time;
                }
            }
        }
    }
}