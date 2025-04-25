// PSX Style Shader for Unity
// Attach this script to a material that uses the shader

Shader "Custom/PSX"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _VertexSnappingDetail ("Vertex Snapping Detail", Range(1, 512)) = 120
        _ColorDepth ("Color Depth", Range(1, 256)) = 32
        _DitherPattern ("Dither Pattern", 2D) = "white" {}
        _DitherStrength ("Dither Strength", Range(0, 1)) = 0.1
        _AffineMapping ("Affine Mapping Strength", Range(0, 1)) = 0.8
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float depth : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            sampler2D _DitherPattern;
            float4 _MainTex_ST;
            float4 _Color;
            float _VertexSnappingDetail;
            float _ColorDepth;
            float _DitherStrength;
            float _AffineMapping;
            
            v2f vert (appdata v)
            {
                v2f o;
                
                // Get position in world space
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                
                // Apply vertex snapping in screen space
                float4 clipPos = mul(UNITY_MATRIX_VP, worldPos);
                clipPos.xyz = clipPos.xyz / clipPos.w;
                clipPos.xy = floor(clipPos.xy * _VertexSnappingDetail) / _VertexSnappingDetail;
                clipPos.xyz *= clipPos.w;
                
                o.vertex = clipPos;
                
                // Calculate affine texture mapping
                float2 affineUV = TRANSFORM_TEX(v.uv, _MainTex);
                float depth = UnityObjectToViewPos(v.vertex).z * _AffineMapping;
                o.depth = 1 + depth * 0.1;
                o.uv = affineUV * o.depth;
                
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.screenPos = ComputeScreenPos(clipPos);
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Perspective correction for texture mapping based on affine strength
                fixed2 correctedUV = i.uv / i.depth;
                
                // Sample the texture
                fixed4 col = tex2D(_MainTex, correctedUV) * _Color;
                
                // Apply color depth reduction (quantization)
                col.rgb = floor(col.rgb * _ColorDepth) / _ColorDepth;
                
                // Apply dithering
                float2 screenPos = i.screenPos.xy / i.screenPos.w * _ScreenParams.xy;
                float2 ditherCoord = screenPos % 4; // 4x4 dither pattern
                float dither = tex2D(_DitherPattern, ditherCoord / 4.0).r - 0.5;
                col.rgb += dither * _DitherStrength;
                
                // Re-quantize after dithering
                col.rgb = floor(col.rgb * _ColorDepth) / _ColorDepth;
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}