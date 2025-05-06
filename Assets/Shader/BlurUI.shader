Shader "Custom/UIBlur"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0,10)) = 3.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        GrabPass { "_GrabTexture" }
        
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 grabPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _GrabTexture;
            float4 _Color;
            float _BlurSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 grabTexcoord = i.grabPos.xy / i.grabPos.w;
                
                // Blur effect
                half4 blur = half4(0,0,0,0);
                float blurStep = _BlurSize / 100.0;
                
                // 9-tap gaussian blur
                blur += tex2D(_GrabTexture, grabTexcoord + float2(-blurStep, -blurStep)) * 0.0625;
                blur += tex2D(_GrabTexture, grabTexcoord + float2(0, -blurStep)) * 0.125;
                blur += tex2D(_GrabTexture, grabTexcoord + float2(blurStep, -blurStep)) * 0.0625;
                
                blur += tex2D(_GrabTexture, grabTexcoord + float2(-blurStep, 0)) * 0.125;
                blur += tex2D(_GrabTexture, grabTexcoord) * 0.25;
                blur += tex2D(_GrabTexture, grabTexcoord + float2(blurStep, 0)) * 0.125;
                
                blur += tex2D(_GrabTexture, grabTexcoord + float2(-blurStep, blurStep)) * 0.0625;
                blur += tex2D(_GrabTexture, grabTexcoord + float2(0, blurStep)) * 0.125;
                blur += tex2D(_GrabTexture, grabTexcoord + float2(blurStep, blurStep)) * 0.0625;
                
                // Combine with tint color
                half4 col = blur * _Color;
                return col;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}