Shader "Custom/StarrySkyShader"
{
    Properties
    {
        _MainTex            ("MainTex",         2D)             =   "white" {}

        _StarNum ("Star Num", Range(0, 1)) = 10

    }
    SubShader
    {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True" }

        Pass
        {
			Tags{"LightMode"="ForwardBase"}
			Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            // Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            float _StarNum;


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // 内部图片
            sampler2D   _MainTex;
            float4      _MainTex_TexelSize;
            float4      _MainTex_ST;

            
            float random (float2 st) {
                return frac(sin(dot(st.xy, float2(565656.233,123123.2033))) * 323434.34344);
            }
            float2 random2 (float2 p) {
                return frac(sin(float2(dot(p,float2(234234.1,54544.7)), sin(dot(p,float2(33332.5,18563.3))))) *323434.34344);
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

                float2 uv = data.uv * 10;// / (_ScreenParams);
                float2 ipos = floor(uv);  // 整数部分组成二维数组：网格的坐标
                float2 fpos = frac(uv + _Time.x / 500);  // 小数部分组成二维数组：网格内的UV
                // 通过坐标，生成星星的uv坐标
                float2 targetPoint = random2(ipos);    
                float dist = abs(fpos - targetPoint);//当前uv坐标到星星uv坐标的距离
                fixed3 color = fixed3(1, 1, 1) * (1.0 - step(0.013 * _StarNum, dist));//距离大于0.013就显示成黑色



                fixed4 col = fixed4(0, 0, 0, 0);
				// return fixed4(color, 1);
				return random(uv / 1000000);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
