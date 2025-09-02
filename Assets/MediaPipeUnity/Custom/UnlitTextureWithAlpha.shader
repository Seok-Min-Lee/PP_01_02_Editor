Shader "Custom/UnlitTextureWithAlpha"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // �ؽ�ó ������Ƽ
        _Color ("Color Tint", Color) = (1,1,1,1) // ���� ƾƮ ������Ƽ
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        // ���� ���� (���ĸ� ����Ͽ� ���� ǥ��)
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
            float4 _Color; // ���� ƾƮ

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // �ؽ�ó�� ����� ���� ���� �����ɴϴ�.
                fixed4 texColor = tex2D(_MainTex, i.uv);
                // ���İ��� ������ ����� _Color ������Ƽ�� ���� ƾƮ�� ����
                return texColor * _Color;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Cutout/VertexLit"
}