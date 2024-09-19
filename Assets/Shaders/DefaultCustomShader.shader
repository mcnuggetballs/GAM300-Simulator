Shader "Custom/SetStencilValue"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _StencilValue("Stencil Value", int) = 1
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            // Set stencil buffer settings
            Stencil
            {
                Ref[_StencilValue]  // Reference stencil value
                Comp Always          // Always set the stencil value
                Pass Replace         // Replace stencil value on pass
            }

        // Normal depth testing
        ZTest LEqual
        ZWrite On

        // Regular color rendering
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            float4 color : COLOR;
        };

        struct v2f
        {
            float4 pos : SV_POSITION;
            float4 color : COLOR;
        };

        float4 _Color;

        v2f vert(appdata v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.color = v.color * _Color;
            return o;
        }

        half4 frag(v2f i) : SV_Target
        {
            return i.color;
        }
        ENDCG
    }
    }

        // Fallback shader
            Fallback "Diffuse"
}
