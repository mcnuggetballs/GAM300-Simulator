Shader "Custom/StencilWriter"
{
    SubShader
    {
        Pass
        {
            // Stencil operations
            Stencil
            {
                Ref 1         // Write 1 to the stencil buffer
                Comp Always   // Always pass stencil test
                Pass Replace  // Replace the stencil buffer value with Ref
            }

        // Basic rendering (you can customize this further if needed)
        ColorMask 0      // Don't write any color to the framebuffer

        // You don't need a custom vertex/fragment program for this simple case
    }
    }

        // Disable fallback to avoid extra unwanted behavior
        Fallback Off
}
