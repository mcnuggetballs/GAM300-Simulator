Shader "Custom/GloomyEffect"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _GloomColor("Gloom Color", Color) = (0.1, 0.1, 0.2, 1.0) // Gloomy bluish tint
        _DarkenFactor("Darken Factor", Range(0, 1)) = 0.6 // Controls how dark the scene is
        _Saturation("Saturation", Range(-1, 1)) = -0.2 // Controls desaturation
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

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
                    float4 pos : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _GloomColor;
                float _DarkenFactor;
                float _Saturation;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Sample the texture
                    fixed4 col = tex2D(_MainTex, i.uv);

                // Apply darken factor
                col.rgb *= _DarkenFactor;

                // Apply gloomy tint
                col.rgb = lerp(col.rgb, _GloomColor.rgb, 0.5);

                // Desaturate the scene
                float gray = dot(col.rgb, float3(0.3, 0.59, 0.11));
                col.rgb = lerp(col.rgb, gray, -_Saturation);

                return col;
            }
            ENDCG
        }
        }
            FallBack "Diffuse"
}
