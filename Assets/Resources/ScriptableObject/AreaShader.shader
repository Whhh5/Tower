Shader "Custom/AreaShader"
{
    Properties
    {

    }
    SubShader
    {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True" }
		// Tags { "Queue" = "ALphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" "DisableBatching" = "True" }

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


            // 外部参数
            fixed4      _ArrPoint[50];
            int         _ArrLength;
            
 


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
				// o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f data) : SV_Target
            {
                fixed4 col = fixed4(0, 0, 0, 0);

                // Begin  区域判断
                int rangeNum = 0;
                int centerNum = 0;
                int loopNum = 3;
                // float rangePoint = 3;
                // data.uv.x *= _ScreenParams.x / _ScreenP+arams.y;
                // data.uv.y *= _ScreenParams.y / _ScreenParams.x;
                float maxEdgeLength = max(_ScreenParams.x, _ScreenParams.y);
                fixed2 customUV = fixed2(data.uv.x * _ScreenParams.x / _ScreenParams.y, data.uv.y);
                // fixed2 customUV = fixed2(data.uv.x * _ScreenParams.x / maxEdgeLength, data.uv.y * _ScreenParams.y / maxEdgeLength);

                [loop]for(int k = 0; k < _ArrLength; k++)
                {
                    fixed4 pos1 = _ArrPoint[k];
                    pos1.x *= _ScreenParams.x / _ScreenParams.y;
                    [loop]for(int h = k; h < _ArrLength; h++)
                    {
                        fixed4 pos2 = _ArrPoint[h];
                        pos2.x *= _ScreenParams.x / _ScreenParams.y;

                        fixed dis = max(pos1.z, pos2.z);
                        fixed dim = max(pos1.w, pos2.w);

                        dis = pos1.z + pos2.z;
                        dim = pos1.w + pos2.w;

				        float temp;
				        [loop]for(int j = -loopNum; j <= loopNum; j++)
                        {
				        	[loop]for(int k = -loopNum; k <= loopNum; k++)
                            {
				        		// float dis1 = distance(pos1.xy, float2(j * pos1.w + customUV.x, k * pos1.w + customUV.y));
				        		// float dis2 = distance(pos2.xy, float2(j * pos2.w + customUV.x, k * pos2.w + customUV.y));
                                float dis1 = distance(pos1.xy, float2(j, k) * dim + customUV);
				        		float dis2 = distance(pos2.xy, float2(j, k) * dim + customUV);
				        		temp += min(dis1, dis2);
				        	}
				        }

				        temp /= pow((loopNum * 2 + 1), 2);
				        if(temp < dis)
                        {
				        	col = floor(temp * 50) * (loopNum / 10.0);
				        }
                    }
                }
                // End

                // 剔除颜色 Begin 
                // clip(col.r - 0.001);
                // End
				return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
