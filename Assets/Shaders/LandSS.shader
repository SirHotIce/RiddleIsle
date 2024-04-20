Shader "Custom/IslandTerrainSurface"
{
    Properties
    {
        _SandColor ("Sand Color", Color) = (1,1,1,1)
        _GrassColor ("Grass Color", Color) = (0,1,0,1)
        _RockColor ("Rock Color", Color) = (0.5,0.5,0.5,1)
        _SnowColor ("Snow Color", Color) = (1,1,1,1)
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
            float3 worldPos;
        };

        fixed4 _SandColor, _GrassColor, _RockColor, _SnowColor;
        float _SandHeight, _GrassHeight, _RockHeight;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Height-based blending
            float h = IN.worldPos.y;
            float blendSand = 1 - smoothstep(_SandHeight - 0.5, _SandHeight, h);
            float blendGrass = smoothstep(_SandHeight - 0.5, _SandHeight, h) * (1 - smoothstep(_GrassHeight - 0.5, _GrassHeight, h));
            float blendRock = smoothstep(_GrassHeight - 0.5, _GrassHeight, h) * (1 - smoothstep(_RockHeight - 0.5, _RockHeight, h));
            float blendSnow = smoothstep(_RockHeight - 0.5, _RockHeight, h);

            // Color blending
            fixed3 sand = _SandColor.rgb * blendSand;
            fixed3 grass = _GrassColor.rgb * blendGrass;
            fixed3 rock = _RockColor.rgb * blendRock;
            fixed3 snow = _SnowColor.rgb * blendSnow;

            // Combine colors
            fixed3 col = sand + grass + rock + snow;

            o.Albedo = col;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
