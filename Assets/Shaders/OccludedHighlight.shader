Shader "Custom/OccludedHighlight"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {} // Albedo map
        _TintColor("Tint Color", Color) = (1, 1, 1, 1) // Tint color
        _Metallic("Metallic", Range(0, 1)) = 0.0 // Metallic value
        _Glossiness("Smoothness", Range(0, 1)) = 0.5 // Smoothness value
        _HighlightColor("Highlight Color", Color) = (1, 0, 0, 1) // Highlight color
        _HighlightThickness("Highlight Thickness", Range(1, 10)) = 2 // Highlight thickness
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }

            // First pass: render the object normally
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
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 pos : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _TintColor; // Tint color
                float _Metallic;
                float _Glossiness;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 albedo = tex2D(_MainTex, i.uv); // Sample Albedo texture
                    albedo.rgb *= _TintColor.rgb; // Apply the tint to the albedo

                    float metallic = _Metallic;
                    float glossiness = _Glossiness;

                    // Calculate the final color
                    fixed3 color = albedo.rgb * (1 - metallic); // Modulate color based on metallic
                    float alpha = albedo.a * glossiness; // Modulate alpha based on glossiness

                    return fixed4(color, alpha); // Return final color
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
