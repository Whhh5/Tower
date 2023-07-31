Shader "Custom/EdgeShader"
{
    Properties
    {
        _MainTex            ("MainTex",         2D)             =   "white" {}
        _NoiseTex           ("EdgeTex",         2D)             =   "white" {}


        _Coefficient        ("Coefficient",     Int)            =   20

        // 边缘参数
        _EdgeColor          ("Edge Color",      Color)          =   (1, 1, 1, 1)   
        _EdgeSize           ("Edge Size",       Range(0, 1))    =   0.15
        _ShakeSpeed         ("Shake Speed",     Range(0, 0.1))  =   0.3
        _ShakeExtend        ("Shake Extend",    Range(0, 2))    =   1

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

            // 内部图片
            sampler2D   _MainTex;
            float4      _MainTex_TexelSize;
            float4      _MainTex_ST;
            sampler2D   _NoiseTex;
            float4      _EdgeTex_TexelSize;


            // 内部参数
            fixed       _Coefficient;
            fixed       _EdgeSize;
            fixed       _ShakeSpeed;
            fixed       _ShakeExtend;
            fixed4      _EdgeColor;


            
 


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.vertex = UnityObjectToViewPos(v.vertex);
                // o.uv = v.uv;
                
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f data) : SV_Target
            {
                int loopNum = 1;
                // 剔除颜色 Begin 
                // clip(col.a - 0.001);
                // End

                // Regin 噪声值
                fixed noiseValue = (tex2D(_NoiseTex, data.uv + _Time.y * _ShakeExtend).r + tex2D(_NoiseTex, data.uv - _Time.x * _ShakeExtend).r) * 0.5;

                // End

                // Regin 参考参数
                fixed shakeValue = noiseValue * _ShakeSpeed;

                // End

                // Begin 提取边缘
                fixed4 curAreaCol = fixed4(0, 0, 0, 0);
                fixed4 curOffsetPixCol = fixed4(0, 0, 0, 0);
                float2 curPixPos = _MainTex_TexelSize * data.uv;
                int num = 0;


				[loop]for(int i = -loopNum; i <= loopNum; i++)
                {
                    [loop]for(int j = -loopNum; j <= loopNum; j++)
                    {
                        float2 pixPos = curPixPos + float2(i, j);
                        curAreaCol = (curAreaCol + tex2D(_MainTex, pixPos / _MainTex_TexelSize)) * 1;//0.5;
                        curOffsetPixCol = (curOffsetPixCol + tex2D(_MainTex, data.uv + fixed2(i, j) * shakeValue)) * 1;//0.5;
                        num += 1;
                    }
                }
                curAreaCol /= num;
                curOffsetPixCol /= num;
                fixed difference = (
                    abs(curAreaCol.r - curOffsetPixCol.r) + 
                    abs(curAreaCol.g - curOffsetPixCol.g) + 
                    abs(curAreaCol.b - curOffsetPixCol.b) +
                    abs(curAreaCol.a - curOffsetPixCol.a)
                    ) / 4;




                fixed disValue = min(max((difference - _EdgeSize), 0), 1);
                // fixed disValue = min(max(difference - _EdgeSize, 0), 1);

                fixed4 tempColor = fixed4(_EdgeColor.rgb, _EdgeColor.a * disValue);
                fixed4 col = disValue;
                // End


                // Begin 边缘颜色偏移



                // End



                // Begin 内部着色



                // End




                // Begin 边缘着色



                // End



                // col = tex2D(_MainTex, data.uv);
				return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
