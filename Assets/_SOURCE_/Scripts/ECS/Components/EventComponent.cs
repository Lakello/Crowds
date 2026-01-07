namespace _SOURCE_.Scripts.ECS.Components
{
    using Data;

    public struct EventComponent
    {
        public bool IsActive;
        public IEventHandler Handler;
    }
}