Shader "UI/AnimatedOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,0.5,0,1)
        _OutlineWidth ("Outline Width", Range(0, 10)) = 2
        _AnimSpeed ("Animation Speed", Range(0, 10)) = 3
        _Progress ("Progress", Range(0, 1)) = 0.75

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct meshdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct interpolator
            {
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineWidth;
            float _AnimSpeed;
            float _Progress;
            float4 _ClipRect;

            interpolator vert(meshdata v)
            {
                interpolator o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            // Gets perimeter coordinate (0-1) starting from top middle, going clockwise
            float getPerimeterCoord(float2 uv)
            {
                // Center UV (from -0.5 to 0.5)
                float2 centered = uv - 0.5;
                float2 absUV = abs(centered);
                
                // Check which edge we're on
                if (absUV.x >= absUV.y) {
                    // On left or right edge
                    if (centered.x >= 0) {
                        // Right edge (0.25 to 0.5)
                        return 0.25 + 0.25 * (0.5 - centered.y);
                    } else {
                        // Left edge (0.75 to 1)
                        return 0.75 + 0.25 * (0.5 + centered.y);
                    }
                } else {
                    // On top or bottom edge
                    if (centered.y >= 0) {
                        // Top edge (0 to 0.25)
                        return 0.25 * (0.5 + centered.x);
                    } else {
                        // Bottom edge (0.5 to 0.75)
                        return 0.5 + 0.25 * (0.5 - centered.x);
                    }
                }
            }

            fixed4 frag(interpolator i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv) * i.color;
                float2 centered = i.uv - 0.5;
                float2 absUV = abs(centered);
                float distFromEdge = 0.5 - max(absUV.x, absUV.y);

                float pixelWidth = _OutlineWidth * 0.01;
                float outlineMask = step(distFromEdge, pixelWidth) * step(0, distFromEdge);

                if (outlineMask > 0) {
                    float perimPos = getPerimeterCoord(i.uv);
                    float segmentMask = step(0.0, perimPos) * step(perimPos, _Progress);
                    outlineMask *= segmentMask;
                }

                float3 finalColor = lerp(color.rgb, _OutlineColor.rgb, outlineMask);
                float finalAlpha = max(color.a, outlineMask * _OutlineColor.a);

                #ifdef UNITY_UI_CLIP_RECT
                finalAlpha *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(finalAlpha - 0.001);
                #endif

                return half4(finalColor, finalAlpha);
            }


            ENDCG
        }
    }
}