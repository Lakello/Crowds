namespace TargetModule
{
    using UnityEngine;

    public partial class TargetPoint
    {
        public interface ITargetPointMode
        {
            public TargetPointData Construct(TargetPointData data, TargetPoint targetPoint);

            public Vector3 GetTargetPosition(ref TargetPointData data);
            public bool CanNext(ref TargetPointData data);
            
            public bool CanSkip(ref TargetPointData data);
            
            public ITargetPointMode GetSelfMode();
        }
    }
}