Shader "iKame/FilledMotionBackground"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FillAmount ("Fill Amount", Range(0, 1)) = 0.5
        _Opacity ("Opacity", Range(0, 1)) = 0.5
        _Speed ("Speed", float) = 0.5
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "CanUseSpriteAtlas" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off
        ZWrite Off

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
            float _FillAmount;
            float _Speed;

            interpolator vert(meshdata v)
            {
                interpolator i;
                i.vertex = UnityObjectToClipPos(v.vertex);
                
                i.uv = v.uv;
                
                return i;
            }

            fixed4 frag(interpolator i) : SV_Target
            {
                // TODO : Fix uv movement
                // if (i.uv.x < _FillAmount && frac(i.uv.x) > 0.01 && frac(i.uv.x) < 0.99) {
                //     i.uv.x += _Time.y * _Speed;
                //     i.uv.x = clamp(i.uv.x, 0.01, 0.99);
                // }
                
                fixed4 col = tex2D(_MainTex, frac(i.uv));
                
                if (i.uv.x > _FillAmount) col.a = 0;
                
                return col;
            }
            ENDCG
        }
    }
}