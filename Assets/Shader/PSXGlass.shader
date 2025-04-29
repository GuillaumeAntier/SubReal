Shader "Custom/PSXGlass" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,0.5)
        _Transparency ("Transparency", Range(0,1)) = 0.7
        _VertexJitterAmount ("Vertex Jitter", Range(0,0.1)) = 0.0  // Mis à 0 par défaut
        _AffineMapIntensity ("Affine Mapping", Range(0,1)) = 0.0  // Mis à 0 par défaut
        _PixelSnap ("Pixel Snapping", Range(0,512)) = 256
        [Toggle] _EnableTimeJitter ("Enable Time-based Jitter", Float) = 0  // Option pour activer/désactiver
    }
    
    SubShader {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "IgnoreProjector"="True"
        }
        
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 rawUV : TEXCOORD1;  // Original, unmodified UVs
                float depth : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Transparency;
            float _VertexJitterAmount;
            float _AffineMapIntensity;
            float _PixelSnap;
            float _EnableTimeJitter;
            
            float4 PSXSnap(float4 vertex) {
                // Add jitter only if enabled
                if (_EnableTimeJitter > 0.5) {
                    // Time-based jitter (causes movement)
                    float jitter = sin(_Time.y * 20.0) * _VertexJitterAmount;
                    vertex.x += jitter;
                    vertex.y += jitter;
                } else {
                    // Static jitter based on position (no movement)
                    float jitter = frac(vertex.x * 73.7 + vertex.y * 52.3) * _VertexJitterAmount;
                    vertex.x += jitter;
                    vertex.y += jitter;
                }
                
                // Snap to pixel grid
                if (_PixelSnap > 0) {
                    vertex.xyz = round(vertex.xyz * _PixelSnap) / _PixelSnap;
                }
                
                return vertex;
            }
            
            v2f vert (appdata v) {
                v2f o;
                // Apply PSX-style vertex jitter
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                worldPos = PSXSnap(worldPos);
                o.vertex = UnityWorldToClipPos(worldPos);
                
                // Store both modified and original UVs
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.rawUV = o.uv;
                o.depth = max(0.1, o.vertex.z * _AffineMapIntensity);
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                // Choose between affine mapping or normal mapping
                float2 finalUV;
                if (_AffineMapIntensity > 0.01) {
                    // Apply PSX-style affine texture mapping
                    finalUV = lerp(i.rawUV, i.uv / i.depth, _AffineMapIntensity);
                } else {
                    // Use normal UV mapping (stable)
                    finalUV = i.rawUV;
                }
                
                // Simulate PSX texture point sampling (disable filtering)
                float2 textureSize = 64.0; // Adjust based on your texture size
                float2 pixelatedUV = floor(finalUV * textureSize) / textureSize;
                
                // Sample the texture with point filtering
                fixed4 col = tex2D(_MainTex, pixelatedUV) * _Color;
                
                // Apply glass transparency
                col.a *= _Transparency;
                
                // Apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}