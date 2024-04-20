Shader "Custom/IslandTerrainSurfaceTextures"
{
    Properties
    {
        _SandTex ("Sand Texture", 2D) = "white" {}
        _GrassTex ("Grass Texture", 2D) = "white" {}
        _RockTex ("Rock Texture", 2D) = "white" {}
        _SnowTex ("Snow Texture", 2D) = "white" {}
        // Adjusted height ranges to fit within -2 to 6
        _SandHeight ("Transition to Grass Height", Range(-2, 6)) = 1
        _GrassHeight ("Transition to Rock Height", Range(-2, 6)) = 3
        _RockHeight ("Transition to Snow Height", Range(-2, 6)) = 5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        sampler2D _SandTex, _GrassTex, _RockTex, _SnowTex;
        float _SandHeight, _GrassHeight, _RockHeight;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Height-based blending
            float h = IN.worldPos.y;
            float blendSand = 1 - smoothstep(_SandHeight - 0.5, _SandHeight, h);
            float blendGrass = smoothstep(_SandHeight - 0.5, _SandHeight, h) * (1 - smoothstep(_GrassHeight - 0.5, _GrassHeight, h));
            float blendRock = smoothstep(_GrassHeight - 0.5, _GrassHeight, h) * (1 - smoothstep(_RockHeight - 0.5, _RockHeight, h));
            float blendSnow = smoothstep(_RockHeight - 0.5, _RockHeight, h);

            // Texture sampling
            fixed4 sand = tex2D(_SandTex, IN.uv_MainTex) * blendSand;
            fixed4 grass = tex2D(_GrassTex, IN.uv_MainTex) * blendGrass;
            fixed4 rock = tex2D(_RockTex, IN.uv_MainTex) * blendRock;
            fixed4 snow = tex2D(_SnowTex, IN.uv_MainTex) * blendSnow;

            // Combine textures
            fixed4 col = sand + grass + rock + snow;

            o.Albedo = col.rgb;
            o.Alpha = col.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
