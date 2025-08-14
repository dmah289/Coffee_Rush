Shader "Unlit/background_gradient"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (1, 1, 1, 1)
        _Color2 ("Color 2", Color) = (0, 0, 0, 1)
        _Speed ("Speed", Range(0.5, 2)) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
        }
        
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off

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
            fixed4 _Color1, _Color2;
            half _Speed;

            interpolator vert(meshdata v)
            {
                interpolator i;
                i.vertex = UnityObjectToClipPos(v.vertex);
                i.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return i;
            }

            fixed4 frag(interpolator i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 gradientColor = lerp(_Color1, _Color2, i.uv.y * _Speed);
                return fixed4(col.rgb * gradientColor.rgb, _Color1.a);
            }
            ENDCG
        }
    }
}