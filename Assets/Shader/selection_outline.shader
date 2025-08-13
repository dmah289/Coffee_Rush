Shader "iKame/OutlinedObject" {
    Properties {
        _Thickness ("Thickness", Float) = 0.1
        _Bias ("Bias", Range(0, 0.1)) = 0.01
        _Selected ("Selected", Float) = 0
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
    }

    SubShader {
        Pass {
            ZWrite Off
            Blend One Zero
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _Thickness, _Bias, _Selected;
            fixed4 _OutlineColor;

            float4 vert(float4 vertex : POSITION, float3 normal : NORMAL) : SV_POSITION {
                float3 extruded = vertex.xyz + normal * (_Thickness * _Selected);
                float4 pos = UnityObjectToClipPos(float4(extruded, 1));
                pos.z -= _Bias * _Selected;
                return pos;
            }

            fixed4 frag() : SV_Target {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}