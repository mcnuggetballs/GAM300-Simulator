Shader "Custom/Outline"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1) // Outline color
        _OutlineThickness("Outline Thickness", Range(0.0, 0.1)) = 0.015 // Thickness of the outline
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }

        // Single pass: Create the outline by rendering a slightly scaled version of the object
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "Always" }
            Cull Front // Cull the front faces to ensure we see the outline from the outside

            CGPROGRAM
            #pragma vertex vertOutline
            #pragma fragment fragOutline
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _OutlineThickness;
            float4 _OutlineColor;

            // Vertex shader to scale the object slightly
            v2f vertOutline(appdata v)
            {
                v2f o;

                // Scale the vertex position along the normal to create an outline
                float3 normal = normalize(v.vertex.xyz);
                v.vertex.xyz += normal * _OutlineThickness;

                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            // Fragment shader to render the outline color
            fixed4 fragOutline(v2f i) : SV_Target
            {
                return _OutlineColor; // Render only the outline color
            }
            ENDCG
        }
    }
        FallBack "Diffuse"
}
