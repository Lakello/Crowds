namespace Game.UI
{
    using Unity.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class BatchedHealthBarsGraphic : MaskableGraphic
    {
        [SerializeField] private Camera _worldCamera;
        [SerializeField] private Canvas _canvas;

        public NativeList<Bar> Bars { get; private set; } = new NativeList<Bar>(Allocator.Persistent);

        public struct Bar
        {
            public Vector3 WorldPosition;
            public float NormalizedHealth;
            public Vector2 Size;
            public float OffsetY;
            public Color BackgroundColor;
            public Color FillColor;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Bars.Dispose();
        }

        public void SetDirty()
        {
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();

            if (_worldCamera == null || _canvas == null)
            {
                return;
            }

            RectTransform canvasRect = _canvas.transform as RectTransform;

            int vertCount = 0;

            for (int i = 0; i < Bars.Length; i++)
            {
                Bar bar = Bars[i];

                if (bar.NormalizedHealth <= 0f)
                {
                    continue;
                }

                Vector3 screenPoint = _worldCamera.WorldToScreenPoint(bar.WorldPosition);
                if (screenPoint.z <= 0)
                {
                    continue;
                }

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, screenPoint, _canvas.renderMode == RenderMode.ScreenSpaceOverlay
                        ? null
                        : _canvas.worldCamera,
                    out Vector2 local);

                local.y += bar.OffsetY;

                float w = bar.Size.x;
                float h = bar.Size.y;

                Vector2 p0 = local + new Vector2(-w * 0.5f, -h * 0.5f);
                Vector2 p1 = local + new Vector2(-w * 0.5f, h * 0.5f);
                Vector2 p2 = local + new Vector2(w * 0.5f, h * 0.5f);
                Vector2 p3 = local + new Vector2(w * 0.5f, -h * 0.5f);

                AddQuad(vertexHelper, p0, p1, p2, p3, bar.NormalizedHealth, ref vertCount);

                // float fw = w * Mathf.Clamp01(bar.NormalizedHealth);
                // Vector2 fp0 = local + new Vector2(-w * 0.5f, -h * 0.5f);
                // Vector2 fp1 = local + new Vector2(-w * 0.5f, h * 0.5f);
                // Vector2 fp2 = local + new Vector2(-w * 0.5f + fw, h * 0.5f);
                // Vector2 fp3 = local + new Vector2(-w * 0.5f + fw, -h * 0.5f);
                //
                // AddQuad(vertexHelper, fp0, fp1, fp2, fp3, Color.white, bar.NormalizedHealth, 1f, ref vertCount);
            }
        }

        static void AddQuad(VertexHelper vh, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3,
            float hp01, ref int vertCount)
        {
            UIVertex v = UIVertex.simpleVert;

            // hp в альфу. rgb белый (можно использовать как tint)
            v.color = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(Mathf.Clamp01(hp01) * 255f));

            v.position = p0; v.uv0 = new Vector2(0, 0); vh.AddVert(v);
            v.position = p1; v.uv0 = new Vector2(0, 1); vh.AddVert(v);
            v.position = p2; v.uv0 = new Vector2(1, 1); vh.AddVert(v);
            v.position = p3; v.uv0 = new Vector2(1, 0); vh.AddVert(v);

            vh.AddTriangle(vertCount + 0, vertCount + 1, vertCount + 2);
            vh.AddTriangle(vertCount + 2, vertCount + 3, vertCount + 0);
            vertCount += 4;
        }
    }
}