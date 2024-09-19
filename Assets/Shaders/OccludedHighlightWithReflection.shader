Shader "Custom/OccludedHighlightWithReflection"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {} // Albedo map
        _TintColor("Tint Color", Color) = (1, 1, 1, 1) // Tint color
        _SpecColor("Specular Color", Color) = (1, 1, 1, 1) // Specular color
        _Glossiness("Glossiness", Range(0, 1)) = 0.5 // Smoothness value for specular highlights
        _Metallic("Metallic", Range(0, 1)) = 0.0 // Metallic value
        _ReflectionCubemap("Reflection Cubemap", Cube) = "" {} // Cubemap for reflections
        _ReflectionStrength("Reflection Strength", Range(0, 1)) = 0.5 // Reflection intensity
        _HighlightColor("Highlight Color", Color) = (1, 0, 0, 1) // Highlight color
        _HighlightThickness("Highlight Thickness", Range(1, 10)) = 2 // Highlight thickness
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }

            // First pass: render the object normally with specular highlights and reflections
            Pass
            {
                Name "NormalPass"
                ZWrite On
                Stencil
                {
                    Ref 1
                    Comp Always
                    Pass Replace
                }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 pos : SV_POSITION;
                    float3 worldPos : TEXCOORD1;
                    float3 worldNormal : TEXCOORD2;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _TintColor; // Tint color
                float4 _SpecColor; // Specular color
                float _Glossiness; // Smoothness for specular highlights
                float _Metallic;
                samplerCUBE _ReflectionCubemap; // Reflection cubemap
                float _ReflectionStrength; // Reflection intensity

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.worldNormal = mul((float3x3)unity_ObjectToWorld, v.normal);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Albedo and Tint
                    fixed4 albedo = tex2D(_MainTex, i.uv) * _TintColor;

                // Calculate world reflection vector
                float3 worldReflect = reflect(normalize(i.worldPos - _WorldSpaceCameraPos), normalize(i.worldNormal));

                // Sample the reflection cubemap
                fixed4 reflection = texCUBE(_ReflectionCubemap, worldReflect) * _ReflectionStrength;

                // Specular highlights
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 halfDir = normalize(viewDir + normalize(i.worldNormal));
                float specIntensity = pow(saturate(dot(i.worldNormal, halfDir)), _Glossiness * 128);
                fixed4 specular = _SpecColor * specIntensity;

                // Combine albedo, reflection, and specular
                fixed3 finalColor = albedo.rgb * (1 - _Metallic) + reflection.rgb + specular.rgb;
                return fixed4(finalColor, albedo.a);
            }
            ENDCG
        }

            // Second pass: render occluded parts with highlight
            Pass
            {
                Name "OccludedHighlightPass"
                ZWrite Off
                ZTest Greater
                Blend SrcAlpha OneMinusSrcAlpha

                Stencil
                {
                    Ref 1
                    Comp NotEqual
                    Pass Keep
                }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragHighlight
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

                float4 _HighlightColor;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 fragHighlight(v2f i) : SV_Target
                {
                    return _HighlightColor;
                }
                ENDCG
            }
        }

            FallBack "Diffuse"
}
