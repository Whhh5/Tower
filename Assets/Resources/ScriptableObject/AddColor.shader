Shader "Custom/AddColor"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "black"{}
        _SourceTex ("Main Tex", 2D) = "black"{}
        _AreaTex ("Tex", 2D) = "white" {}
    }
    SubShader
    {
		Tags { "Queue" = "ALphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" "DisableBatching" = "True" }

        Pass
        {
			Tags{"LightMode"="ForwardBase"}
			Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
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
                float4 vertex : SV_POSITION;
            };


            sampler2D _MainTex;
            fixed4 _MainTex_TexelSize;


            sampler2D _SourceTex;
            float4 _SourceTex_TexelSize;

            sampler2D _AreaTex;
            float4 _AreaTex_TexelSize;


            float2 _StartPoint;

            float _MainScale = 1;
 

            fixed GetTipMapColor(float2 f_MainPos)
            {
                float2 mainPos = (f_MainPos - _StartPoint);

                float2 texSize = _AreaTex_TexelSize.zw * 2;

                mainPos.x = min(max(mainPos.x, 0), texSize.x) % texSize.x;
                mainPos.y = min(max(mainPos.y, 0), texSize.y) % texSize.y;

                fixed2 uv = mainPos / texSize;

                fixed4 col = tex2D(_AreaTex, uv);


                return col.r;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f data) : SV_Target
            {
                fixed4 col = tex2D(_SourceTex, data.uv);
                
                float2 mainPos = data.uv * _SourceTex_TexelSize.zw * _MainScale;

                fixed curCol = GetTipMapColor(mainPos);

                fixed areaCol = floor(curCol);

                fixed endColR = max(col.r + areaCol, areaCol);



                fixed edgeCol = (1 - abs(curCol - 1)) * ceil(curCol) * (1 - areaCol);

                edgeCol = min(edgeCol + ceil(max(edgeCol - 0.7, 0)), 1);

                fixed endColG = max(col.g + edgeCol, edgeCol) * (1 - endColR);


				return fixed4(endColR, endColG, 0, 1);

            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
