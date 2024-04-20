Shader "Custom/IslandTerrainUnlit"
{
    Properties
    {
        _SandColor ("Sand Color", Color) = (1,1,1,1)
        _GrassColor ("Grass Color", Color) = (0,1,0,1)
        _RockColor ("Rock Color", Color) = (0.5,0.5,0.5,1)
        _SnowColor ("Snow Color", Color) = (1,1,1,1)
        _SandHeight ("Sand Height", Range(-2, 0)) = -1
        _GrassHeight ("Grass Height", Range(0, 10)) = 3
        _RockHeight ("Rock Height", Range(10, 20)) = 15
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

            float4 _SandColor, _GrassColor, _RockColor, _SnowColor;
            float _SandHeight, _GrassHeight, _RockHeight;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // Correctly transforming vertex position to world space
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Height-based blending
                float h = i.worldPos.y;
                float blendSand = smoothstep(_SandHeight - 1, _SandHeight, h);
                float blendGrass = smoothstep(_SandHeight, _GrassHeight, h);
                float blendRock = smoothstep(_GrassHeight, _RockHeight, h);
                float blendSnow = 1 - smoothstep(_RockHeight - 5, _RockHeight, h);

                // Color blending
                fixed4 sand = _SandColor * blendSand;
                fixed4 grass = _GrassColor * blendGrass;
                fixed4 rock = _RockColor * blendRock;
                fixed4 snow = _SnowColor * blendSnow;

                // Combine colors
                fixed4 col = sand + grass + rock + snow;

                // Cell shading effect
               // col.rgb = step(0.5, col.rgb); // Simplify color for a cell-shaded look

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
