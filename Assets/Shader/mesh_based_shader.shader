Shader "Unlit/mesh_based_shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Progress ("Progress", Range(0, 1)) = 0.0
        _ElapsedColor ("Elapsed Color", Color) = (1, 1, 1, 1)
        _RemainingColor("Remaining Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct meshdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct interpolator
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _RemainingColor;
            fixed4 _ElapsedColor;
            float _Progress;

            interpolator vert(meshdata v)
            {
                interpolator i;
                i.vertex = UnityObjectToClipPos(v.vertex);
                i.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return i;
            }

            fixed4 frag(interpolator i) : SV_Target
            {
                fixed2 mask = tex2D(_MainTex, i.uv).ra;
                float pross = _Progress*1.1-0.1;
                fixed4 finalColor = lerp(_RemainingColor, _ElapsedColor, smoothstep(pross,pross+0.05, mask.x));
                finalColor.a *= mask.y;
                return finalColor;
            }
            ENDCG
        }
    }
}