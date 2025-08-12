Shader "iKame/OutlinedObject" {
    Properties {
        _Thickness ("Thickness", Range(0, 0.5)) = 0.1
        _Bias ("Bias", Range(0, 0.1)) = 0.01
        _Selected ("Selected", Float) = 0
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
    }

    SubShader {
        
        ZWrite Off
        ZTest Less
        
        Pass {
            Cull Front
            Stencil {
                Ref 1
                Comp NotEqual
                Pass Keep
            }

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