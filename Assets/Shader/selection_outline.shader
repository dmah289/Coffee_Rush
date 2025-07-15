// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "OutlinedObject" {
    Properties {
        _Thickness ("Thickness", Float) = 0.1
        _MainTex ("Texture", 2D) = "white" {}
    }
        
    SubShader {
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
            
            uniform float _Thickness;
                        
            float4 vert(float4 vertex : POSITION, float3 normal : NORMAL) : SV_POSITION {
                return UnityObjectToClipPos(vertex + normal * _Thickness);
            }
            
            float4 frag(void) : COLOR {
                return float4(1.0, 1.0, 1.0, 1.0);
            }
            ENDCG
        }
    } 
}