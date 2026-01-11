namespace ECS.Components.Game
{
    public struct EventHandlerComponent
    {
        public bool IsActive;
        public IEventHandler Handler;
    }
}