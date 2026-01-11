Shader "UI/HealthBarRoundedPxURP"
{
    Properties
    {
        [MainColor]_BackColor  ("Back Color", Color) = (0.15,0.15,0.15,1)
        [MainColor]_BorderColor("Border Color", Color) = (0,0,0,1)

        _LowColor ("Low HP Color", Color) = (0.85,0.15,0.15,1)
        _MidColor ("Mid HP Color", Color) = (0.95,0.75,0.15,1)
        _HighColor("High HP Color", Color) = (0.15,0.85,0.25,1)

        _SizePx ("Bar Size Px (x=width,y=height)", Vector) = (60,8,0,0)

        _RadiusPx   ("Corner Radius Px", Float) = 3
        _BorderPx   ("Border Thickness Px", Float) = 1
        _PaddingPx  ("Inner Padding Px", Float) = 1
        _SoftnessPx ("Edge Softness Px", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "RenderPipeline"="UniversalPipeline"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "UI"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv0        : TEXCOORD0; // 0..1
                half4  color      : COLOR;     // color.a = hp01
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv0         : TEXCOORD0;
                half4  color       : COLOR; // pass through
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BackColor;
                half4 _BorderColor;
                half4 _LowColor;
                half4 _MidColor;
                half4 _HighColor;

                float4 _SizePx;
                float  _RadiusPx;
                float  _BorderPx;
                float  _PaddingPx;
                float  _SoftnessPx;
            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv0 = v.uv0;
                o.color = v.color;
                return o;
            }

            float sdRoundedRectPx(float2 pPx, float2 halfSizePx, float radiusPx)
            {
                float2 b = halfSizePx - radiusPx;
                float2 d = abs(pPx) - b;
                float outside = length(max(d, 0.0));
                float inside = min(max(d.x, d.y), 0.0);
                return outside + inside - radiusPx;
            }

            half4 EvalHpGradient(float t)
            {
                t = saturate(t);
                if (t < 0.5)
                {
                    float k = t / 0.5;
                    return lerp(_LowColor, _MidColor, k);
                }
                else
                {
                    float k = (t - 0.5) / 0.5;
                    return lerp(_MidColor, _HighColor, k);
                }
            }

            half4 frag(Varyings i) : SV_Target
            {
                // HP закодирован в альфе вершинного цвета
                float hp01 = saturate(i.color.a);

                float2 sizePx = max(_SizePx.xy, float2(1.0, 1.0));
                float2 halfSizePx = sizePx * 0.5;

                float2 pPx = (i.uv0 - 0.5) * sizePx;

                float radiusPx   = clamp(_RadiusPx, 0.0, min(halfSizePx.x, halfSizePx.y));
                float borderPx   = max(_BorderPx, 0.0);
                float padPx      = max(_PaddingPx, 0.0);
                float softnessPx = max(_SoftnessPx, 0.0001);

                // Outer shape
                float distOuter = sdRoundedRectPx(pPx, halfSizePx, radiusPx);
                float alphaOuter = saturate(0.5 - distOuter / softnessPx);

                // Inner region (subtract border inward)
                float2 halfInnerPx = halfSizePx - borderPx;
                float innerRadiusPx = max(radiusPx - borderPx, 0.0);

                float distInner = sdRoundedRectPx(pPx, halfInnerPx, innerRadiusPx);
                float alphaInner = saturate(0.5 - distInner / softnessPx);

                float alphaBorder = saturate(alphaOuter - alphaInner);

                // Base: border + back
                half4 col = 0;
                col += _BorderColor * alphaBorder;
                col += _BackColor   * alphaInner;

                // Fill region (inset by padding)
                float2 halfFillPx = halfInnerPx - padPx;
                float fillRadiusPx = max(innerRadiusPx - padPx, 0.0);

                float distFill = sdRoundedRectPx(pPx, halfFillPx, fillRadiusPx);
                float alphaFillShape = saturate(0.5 - distFill / softnessPx);

                // X-clip inside fill rect in pixels
                float leftPx  = -halfFillPx.x;
                float rightPx =  halfFillPx.x;
                float fillRightPx = leftPx + (rightPx - leftPx) * hp01;

                float fillMaskX = step(leftPx, pPx.x) * step(pPx.x, fillRightPx);

                half4 fillCol = EvalHpGradient(hp01);

                // Overlay fill only (без рамки)
                col = lerp(col, fillCol, alphaFillShape * fillMaskX);

                col.a *= alphaOuter;

                // RGB tint из вершинного цвета (оставь белым, если не нужно)
                col.rgb *= max(i.color.rgb, 0.0);

                return col;
            }
            ENDHLSL
        }
    }
}