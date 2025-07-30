Shader "iKame/glossy_ui"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ReflectColor("Reflection Color", Color) = (1,1,1,1)
        _Shininess("Shininess", Float) = 10
        _Speed("Speed", Float) = 1
        _Interval("Scan Interval", Float) = 2
        _Width("Scan Width", Float) = 0.2
        _ScanStart("Scan Start", Float) = -1.5
        _ScanEnd("Scan End", Float) = 1.5
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        
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
            fixed4 _ReflectColor;
            float _Shininess;
            float _Speed;
            float _Interval;
            float _Width;
            float _ScanStart;
            float _ScanEnd;

            interpolator vert(meshdata v)
            {
                interpolator i;
                i.vertex = UnityObjectToClipPos(v.vertex);
                i.uv = v.uv;
                return i;
            }

            fixed4 frag(interpolator i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                float timeMod = fmod(_Time.y * _Speed, _Interval);

                float scanProcess = timeMod / _Interval;
                float scanLine = i.uv.x + i.uv.y - lerp(_ScanStart, _ScanEnd, scanProcess);
                float scanEffect = smoothstep(-_Width * 2, _Width, scanLine) * smoothstep(1.5, 1.5 - _Width, scanLine);
                scanEffect = pow(scanEffect, _Shininess);

                fixed4 reflectColor = _ReflectColor * scanEffect;
                reflectColor.a *= col.a;
                
                return col + reflectColor;
            }
            ENDCG
        }
    }
}