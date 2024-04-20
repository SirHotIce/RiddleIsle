Shader "Custom/AdvancedWaterWithFoam"
{
    Properties
    {
        _WaveLength ("Wave Length", Float) = 0.1
        _MinWaveHeight ("Min Wave Height", Float) = 0.02
        _MaxWaveHeight ("Max Wave Height", Float) = 0.05
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _DeepWaterColor ("Deep Water Color", Color) = (0,0,0.5,1)
        _ShallowWaterColor ("Shallow Water Color", Color) = (0,0.5,1,1)
        _FoamColor ("Foam Color", Color) = (1,1,1,1)
        _MaxHeight ("Maximum Water Height", Float) = 0
        _MinHeight ("Minimum Water Height", Float) = -5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard
        #pragma vertex vert

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
        };

        fixed4 _DeepWaterColor, _ShallowWaterColor, _FoamColor;
        float _WaveLength, _MinWaveHeight, _MaxWaveHeight, _WaveSpeed;
        float _MaxHeight, _MinHeight;

        void vert (inout appdata_full v, out Input o)
        {
            // Adding turbulence by combining multiple sine waves
            float wave = sin(v.vertex.x * _WaveLength + _Time.y * _WaveSpeed) *
                         cos(v.vertex.z * _WaveLength + _Time.y * _WaveSpeed) +
                         0.5 * sin(v.vertex.x * 0.5 * _WaveLength - _Time.y * _WaveSpeed) *
                         cos(v.vertex.z * 0.5 * _WaveLength - _Time.y * _WaveSpeed);
            float waveHeight = lerp(_MinWaveHeight, _MaxWaveHeight, wave);
            v.vertex.y += waveHeight;

            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            o.worldNormal = UnityObjectToWorldNormal(v.normal);
            o.uv_MainTex = v.texcoord.xy;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // // Height-based water color blending with more pronounced transition
            // float heightFactor = (IN.worldPos.y - _MinHeight) / (_MaxHeight - _MinHeight);
            // heightFactor = clamp(heightFactor, 0.0, 1.0); // Ensures the factor is within [0, 1]
            //
            // // Amplify the transition effect by using a power function
            // heightFactor = pow(heightFactor, 1.0); // You can adjust the exponent to control the transition sharpness
            //
            // fixed4 waterColor = lerp(_DeepWaterColor, _ShallowWaterColor, heightFactor);
            o.Albedo = _ShallowWaterColor.rgb;

            // // Improved foam blending
            // float foam = saturate((IN.worldNormal.y - 0.9) * 10);
            // o.Albedo = lerp(o.Albedo, _FoamColor.rgb, foam);
            //
            // o.Alpha = lerp(0.8, 1.0, foam); // Adjusting transparency for foam areas
        }
        ENDCG
    }
    FallBack "Diffuse"
}
