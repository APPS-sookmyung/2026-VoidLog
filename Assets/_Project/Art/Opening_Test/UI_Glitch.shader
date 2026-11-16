Shader "VoidLog/UI_Glitch"
{
    // 오프닝 연출용 글리치 오버레이 셰이더.
    // 전체 화면을 덮는 UI Image에 이 머티리얼을 씌우고,
    // GlitchController 스크립트가 _NoiseIntensity / _LineJitter / _RGBSplit / _Seed 값을
    // 코드로 조절해서 "터졌다 사라지는" 지직거림을 만듭니다.

    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _NoiseIntensity ("Noise Intensity", Range(0,1)) = 0
        _LineJitter ("Line Jitter", Range(0,1)) = 0
        _RGBSplit ("RGB Split", Range(0,0.05)) = 0
        _Seed ("Seed", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _NoiseIntensity;
            float _LineJitter;
            float _RGBSplit;
            float _Seed;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            // 간단한 해시 기반 의사 난수. _Seed를 매 프레임 바꿔주면 매번 다른 패턴이 나옵니다.
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453 + _Seed);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 가로줄 단위로 살짝씩 어긋나는 라인 지터 (지직거리는 화면 밀림 느낌)
                float lineId = floor(uv.y * 60.0);
                float lineOffset = (hash(float2(lineId, _Seed)) - 0.5) * _LineJitter * 0.05;
                uv.x += lineOffset;

                // RGB 채널을 살짝 어긋나게 샘플링해서 색수차(RGB 분리) 느낌
                float2 uvR = uv + float2(_RGBSplit, 0);
                float2 uvB = uv - float2(_RGBSplit, 0);

                fixed4 texColor;
                texColor.r = tex2D(_MainTex, uvR).r;
                texColor.g = tex2D(_MainTex, uv).g;
                texColor.b = tex2D(_MainTex, uvB).b;
                texColor.a = tex2D(_MainTex, uv).a;

                // 랜덤 노이즈 스태틱 (지지직 알갱이)
                float noise = hash(uv * 500.0 + _Seed);
                float staticFlicker = step(1.0 - _NoiseIntensity * 0.5, noise);

                fixed4 col = texColor * i.color;
                col.rgb = lerp(col.rgb, float3(noise, noise, noise), staticFlicker * _NoiseIntensity);
                col.a = max(col.a, _NoiseIntensity * 0.6);

                return col;
            }
            ENDCG
        }
    }
}
