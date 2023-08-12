Shader "Custom/FrameShader"
{
    Properties
    {
        _MainTex            ("MainTex",         2D)             =   "white" {}
        _NoiseTex           ("Noise Tex",       2D)             =   "white" {}

        _RangeTex           ("Range Tex",       2D)             =   "white" {}
        _EdgeTex            ("Edge Tex",        2D)             =   "white" {}
        _StarTex            ("Star Tex",        2D)             =   "white" {}


        _MainCol            ("Main Color", Color) = (0, 0, 0, 1)


        // 边缘参数
        // 颜色
        _EdgeColor          ("Edge Color",      Color)          =   (1, 1, 1, 1)
        // 抖动幅度
        _ShakeAmplitude         ("Shake Amplitude",     Range(0, 0.1))  =   0.3
        // 抖动速度
        _ShakeSpeed        ("Shake Speed",    Range(0, 0.3))    =   0.2


        // 云彩参数
        // 强度
        _CloudStrength ("Cloth Strength", Range(0, 10)) = 1


        // 星星参数


        _Delta ("Delta", Range(0, 1)) = 0.5
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
            float4      _NoiseTex_TexelSize;
            sampler2D   _RangeTex;
            float4      _RangeTex_TexelSize;
            sampler2D   _EdgeTex;
            float4      _EdgeTex_TexelSize;
            sampler2D   _StarTex;
            float4      _StarTex_TexelSize;


            // 内部参数
            fixed4      _MainCol;
            fixed4      _EdgeColor;
            fixed       _ShakeAmplitude;
            fixed       _ShakeSpeed;
            fixed       _CloudStrength;

            fixed _Delta;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f data) : SV_Target
            {
                // Regin
                float4 mainTexSize = _MainTex_TexelSize;


                // End

                // Regin 局部公共参数
                int loopNum = 1;
                // End

                // Regin 噪声值
                fixed noiseValueEdge = (tex2D(_NoiseTex, data.uv + _Time.y * _ShakeSpeed).r + tex2D(_NoiseTex, data.uv - _Time.x * _ShakeSpeed).r) * 0.5;
                fixed noiseValueEdge2 = (tex2D(_NoiseTex, data.uv - _Time.y * _ShakeSpeed * 2).r + tex2D(_NoiseTex, data.uv + _Time.x * _ShakeSpeed * 2).r ) * 0.5;
                // fixed noiseValueInside = (tex2D(_NoiseTex, data.uv + _Time.y * _ShakeSpeed).r + tex2D(_NoiseTex, data.uv - _Time.x * _ShakeSpeed).r) * 0.5;
                // End

                // Regin 参考参数
                fixed shakeValue = noiseValueEdge * _ShakeAmplitude;
                fixed shakeValue2 = noiseValueEdge2 * _ShakeAmplitude;
                // End

                // uv 偏移
                fixed2 offsetUV = data.uv + shakeValue - shakeValue2;
                // End

                // # Regin 颜色过度
                fixed2 mainSizeXY = mainTexSize.xy;
                fixed delty = offsetUV % mainSizeXY;
                fixed scheme = delty / mainSizeXY;
                fixed2 fromUV = offsetUV - delty;

                // fixed2 toUV = fromUV + mainSizeXY;
                // fixed4 fromCol = tex2D(_MainTex, fromUV);
                // fixed4 toCol = tex2D(_MainTex, toUV);
                // fixed4 lerpEdge = lerp(fromCol, toCol, scheme);
                
                fixed4 lerpNum = 0;
                for(int i = -loopNum; i <= loopNum; i++)
                {
                    for(int j = -loopNum; j <= loopNum; j++)
                    {
                        fixed2 toUV = (mainTexSize.zw * offsetUV + float2(i, j)) / mainTexSize.zw;
                        fixed4 toCol = tex2D(_MainTex, toUV);
                        toCol = fixed4(toCol.rgb, toCol.a * 30 * toCol.a);

                        lerpNum += toCol;
                    }
                }
                lerpNum /= pow(2 * loopNum + 1, 2);
                // End
                


                // Begin 内部着色
                fixed rangeCol = 1 - lerpNum.r;
                // End

                // Begin 边缘颜色
                fixed4 edgeCol = fixed4(_EdgeColor.rgb, _EdgeColor.a * lerpNum.g);
                // End



                // Begin 内部行为
                fixed4 cloudCol = fixed4(1, 1, 1, pow(noiseValueEdge, _CloudStrength));
                
                // End


                // Begin 边缘行为



                // End


                // Begin star
                fixed4 starCol = tex2D(_StarTex, data.uv);

                // End



                // 当前颜色



                fixed4 addStar = starCol * fixed4(1, 1, 1, rangeCol);
                fixed4 addCloud = fixed4(cloudCol.rgb, 1) * cloudCol.a + addStar * (1 - cloudCol.a);
                // fixed4 addEdge = addCloud * (1 - lerpNum.g);
                // fixed4 clipCol = fixed4(edgeCol.rgb * lerpNum.g, lerpNum.g) + addEdge;
                fixed4 addEdge = fixed4(edgeCol.rgb * edgeCol.a / addCloud.a, edgeCol.a) + addCloud * (1 - edgeCol.a);
                fixed4 clipCol = addEdge;
                // End

                // fixed4 addCloud = pow((pow(cloudCol.rgb, 2.2) * cloudCol.a) + (pow(addStar.rgb, 2.2) * (1 - cloudCol.a)),(1 / 2.2));
                // fixed4 addEdge = pow((pow(edgeCol.rgb, 2.2) * edgeCol.a) + (pow(addCloud.rgb, 2.2) * (1 - edgeCol.a)),(1 / 2.2));
                


                fixed4 col = clipCol; 
				return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
