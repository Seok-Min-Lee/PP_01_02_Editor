Shader "Custom/UnlitTextureWithAlpha"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // 텍스처 프로퍼티
        _Color ("Color Tint", Color) = (1,1,1,1) // 색상 틴트 프로퍼티
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        // 블렌딩 설정 (알파를 사용하여 투명도 표현)
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Lighting Off
        Cull Off

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color; // 색상 틴트

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 텍스처의 색상과 알파 값을 가져옵니다.
                fixed4 texColor = tex2D(_MainTex, i.uv);
                // 알파값을 포함한 색상과 _Color 프로퍼티의 색상 틴트를 적용
                return texColor * _Color;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Cutout/VertexLit"
}