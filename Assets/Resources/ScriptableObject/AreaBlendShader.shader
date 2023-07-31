Shader "Custom/AreaBlendShader"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}

        _TestValue ("Tast Value", Range(0, 2)) = 1.0
    }
    SubShader
    {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True" }

        Pass
        {
			Tags{"LightMode"="ForwardBase"}
			Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D   _MainTex;
            float4      _MainTex_TexelSize;
            float4      _MainTex_ST;

            fixed _TestValue;

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


            
            // const fixed4 _Arr[2] = 
            //     {
            //         fixed4(0.3, 0.5, 0.2, 0.5), 
            //         fixed4(0.6, 0.3, 0.2, 0.5),
            //     };
            // const int _ArrLength = 2;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f data) : SV_Target
            {
                fixed4 col = fixed4(0, 0, 0, 1);

                fixed2 uv = data.uv;//fixed2(data.uv.x * (_ScreenParams.x / _ScreenParams.y), data.uv.y);


                const fixed4 _Arr[2] = 
                {
                    fixed4(0.3, 0.3, 0.1, 0.5), 
                    fixed4(0.7, 0.5, 0.1, 0.5),
                };
                const int _ArrLength = 2;



                [loop]for(int i = 0; i < _ArrLength; i++)
                {
                    // 圆心1
                    fixed4 arrData = _Arr[i];

                    if(distance(uv, arrData.xy) < arrData.z)
                    {
                        col = fixed4(1, 0, 0, 1);
                    }
                    else
                    {
                        [loop]for(int j = i + 1; j < _ArrLength; j++)
                        {
                            // 圆心2
                            fixed4 arrData2 = _Arr[j];
                            // 两中心点
                            fixed2 center = (arrData2.xy + arrData.xy) * 0.5;

                            // half dis = distance(uv, arrData.xy) + distance(uv, center) + distance(uv, arrData2.xy);
                            // if(dis < _TestValue)
                            // {
                            //     col = fixed4(0, 1, 0, 1);
                            // }

                            // 坐标系变换 两个圆心为 原点，x+ 指向第二个圆的圆心
                            fixed2 dir = arrData2.xy - arrData.xy;//
                            half angle = atan(dir.y / dir.x);//
                            fixed2 newUV = uv - center;
                            fixed newUVLength = distance(fixed2(0, 0), newUV);
                            half curUVAngle = atan(newUV.y / newUV.x);
                            half changeAngle = curUVAngle - angle;
                            fixed2 changeUV = fixed2(cos(changeAngle), sin(changeAngle)) * newUVLength;
                            
                            if(distance(changeUV, fixed2(0, 0)) < 0.1)
                            {
                                col = fixed4(0, 1, 0, 1);
                            }

                            

                        }
                    }
 
                }







				return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
