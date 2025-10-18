Shader "RadialFade"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _Cutoff ("Cutoff", Range(0,1)) = 0
    }
    SubShader
    {
        Tags {"Queue"="Overlay" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            fixed4 _Color;
            float _Cutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                float alpha = smoothstep(_Cutoff, _Cutoff - 0.1, dist);
                return fixed4(_Color.rgb, alpha * _Color.a);
            }
            ENDCG
        }
    }
}
