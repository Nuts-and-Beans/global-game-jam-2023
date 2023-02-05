Shader "Unlit/FogOfWar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColour ("Colour", Color) = (1.0, 1.0, 1.0, 1.0)
        _TimeMultiplier ("TimeMultiplier", Float) = 1.0
        _LoopIterations ("Loop Iterations", Int) = 1 // NOTE(Zack): we clamp this value to 20 to ensure we don't get awful performance
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 screen_pos : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BaseColour;
            float _TimeMultiplier;
            int _LoopIterations;
            #define Time (_Time.y * _TimeMultiplier)

            v2f vert(appdata i) 
            {
                v2f o;
                o.uv = i.uv;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.screen_pos = UnityObjectToViewPos(i.vertex); 
                return o;
            }

            // effect modified from: https://www.shadertoy.com/view/Mtd3W7
            float rand(float2 n)
            {
                float d = dot(n, float2(12.9898f, 8.1414f));
                float c = cos(d);
                float s = sin(c);
                float value = frac(s * 42758.5453f);
                return value;
            }

            float noise(float2 n)
            {
                const float2 d = float2(0.f, 1.f);
                const float2 start = float2(0.f, 0.f);
                const float2 end = float2(1.f, 1.f);
                float2 b = floor(n);
                float2 f = lerp(start, end, frac(n));

                float r1 = rand(b);
                float r2 = rand(b + d.yx);
                float r3 = rand(b + d.xy);
                float r4 = rand(b + d.yy);

                return lerp(lerp(r1, r2, f.x), lerp(r3, r4, f.x), f.y);
            }

            float fbm(float2 n)
            {
                float total = 0.f;
                float amplitude = 1.f;
               
                // REVIEW(Zack): this may be worse for performance as we're no longer using a constant value
                int loop_count = clamp(_LoopIterations, 1, 15);
                for (int i = 0; i < loop_count; ++i) {
                    total += noise(n) * amplitude;
                    amplitude *= 0.377f + sin(Time) / 500.f;
                    n += n;
                }
                return total;
            }

            float fbm_readded(float2 uv)
            {
                float i = fbm(uv);
                uv.x = uv.x * 1.5f; 
                uv.y += 0.5f;

                float i2 = 0.6f; 
                uv.y = uv.y * 2.0f;
                uv.x -= 0.3f;

                float i3 = 1.5f;
                uv.x = uv.x * 2.0f;
                uv.y += 0.7f;

                float i4 = 1.f; 
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
                
                float3 colour = float3(1.f, 1.f, 1.f);
                colour *= intensity; 
                colour.z *= colour.z;

                float overflow = 0.f;
                if (colour.z > 0.6f)
                {
                    colour.y -= 0.05f;
                    colour.z -= 0.096f;
                    colour.z *= 0.93f;
                    colour.z *= sqrt(colour.z) * 1.29f;        
                }
                else
                {
                    colour.z -= 0.1f;
                    colour.z *= 0.9f;
                }

                colour.z -= 0.02f;
                colour.y -= 0.02f;

                colour = colour * sqrt(colour);
                colour.r -= 2.f * overflow; 
                colour.g -= 2.f * overflow;

                colour *= _BaseColour;
                return float4(colour, 1.f);
            }
            
            ENDCG
        }
    }
}
