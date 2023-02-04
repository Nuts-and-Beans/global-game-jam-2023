Shader "Unlit/FogOfWar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Texture", 2D) = "white" {}
        _BaseColour ("Colour", Color) = (1.0, 1.0, 1.0, 1.0)
        _TimeMultiplier ("TimeMultiplier", Float) = 1.0
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            /* #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" */


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 screen_pos : TEXCOORD3;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float4 _NoiseTex_ST;
            float4 _BaseColour;
            float _TimeMultiplier;

            
            #define Time (_Time.y * _TimeMultiplier)

            v2f vert(appdata i) 
            {
                v2f o;
                o.uv = i.uv;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.screen_pos = UnityObjectToViewPos(i.vertex); 
                return o;
            }

            float rand(float2 n)
            {
                float value = frac(sin(cos(dot(n, float2(12.9898f, 8.1414f)))) * 42758.5453f);
                return sqrt(value) / 1.25f;
            }

            float noise(float2 n)
            {
                /* return tex2D(_NoiseTex, n); */
                const float2 d = float2(0.f, 1.f);
                float2 b = floor(n), f = smoothstep(float2(0.f, 0.f), float2(1.f, 1.f), frac(n));
                return lerp(lerp(rand(b), rand(b + d.yx), f.x), lerp(rand(b + d.xy), rand(b + d.yy), f.x), f.y);
            }

            float fbm(float2 n)
            {
                float total = 0.f;
                float amplitude = 1.f;
                for (int i = 0; i < 4; i++) {
                    total += noise(n) * amplitude;
                    n     += n * 2.1f;
                    amplitude *= 0.377f + sin(Time) / 500.f;
                }
                return total;
            }

            float fbm_readded(float2 uv)
            {
                float i = fbm(uv);
                uv.x = uv.x * 1.5f; 
                uv.y += 0.5f;

                float i2 = fbm(uv);
                uv.y = uv.y * 2.0f;
                uv.x -= 0.3f;

                float i3 = fbm(uv);
                uv.x = uv.x * 2.0f;
                uv.y += 0.7f;

                float i4 = fbm(uv);
                uv.y = uv.y * 1.5f;
                uv.x += 0.4f;

                float i5 = fbm(uv);
                return (i + i2 + i3 - i4 + i5) / 3.f;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.screen_pos; 
                uv.x -= Time / 8.f + 1.8f;
                uv.y -= Time / 12.4f + 0.89f;

                float2 uv2 = uv;
                uv2.x += (Time + 10.f) / 15.f;
                uv2.y += (Time - 8.f) / 12.f;

                float2 uv3 = uv2;
                uv3.x += (Time - 7.f) / 102.f;
                uv3.y += (Time + 9.f) / 96.f;

                float main_cloudiness = fbm_readded(uv);
                float intensity = max(main_cloudiness, 1.3f * fbm_readded(uv2)) + 0.9f * main_cloudiness * fbm(uv2) * fbm(uv3);
                intensity *= 0.64f;
                intensity += cos(sin(Time / 10.f)) / 7.f - 0.3f;
                /* return float4(intensity, intensity, intensity, 1.f); */
                
                float3 color = float3(1.f, 1.f, 1.f);
                color *= intensity; 
                color.z *= color.z;
                /* return float4(color, 1.f); */

                float overflow = 0.f;
                color.z -= 0.02f;
                color.y -= 0.02f;
                /* return float4(color, 1.f); */

                color = color * sqrt(color);
                color.r -= 2.f * overflow; 
                color.g -= 2.f * overflow;

                color *= _BaseColour;
                return float4(color, 1.f);
            }
            
            ENDCG
        }
    }
}
