Shader "Custom/EnhanceEdgeShader"
{
	//-----------------------------【属性】-----------------------------
	Properties {
		_MainTex ("Main Tex", 2D) = "white" {}

		// 边缘
		_EdgeWidth ("Edge Width", Range(0, 100)) = 0
        _Subscetion ("细分", Range(0.00001, 0.001)) = 0.0009
	}
	//-----------------------------【子着色器】-----------------------------
	SubShader {
		Pass {  
			ZTest Always Cull Off ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			
			#include "UnityCG.cginc"
			
			#pragma vertex vert  
			#pragma fragment frag
			
			//-----------------------------【变量】-----------------------------
			sampler2D _MainTex;  
			uniform half4 _MainTex_TexelSize;

			int _EdgeWidth;
			fixed _Subscetion;

			//-----------------------------【数据结构】-----------------------------
			struct appdata
			{
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			  
			//-----------------------------【顶点着色器】-----------------------------
			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}
			
			//-----------------------------【片段着色器】-----------------------------
			fixed4 frag(v2f data) : SV_Target {
                fixed4 edgeColor = fixed4(0, 0, 0, 0);
                float2 curPixPos = _MainTex_TexelSize.zw * data.uv;
				
				// 硬边
				for(int i = 0; i < _EdgeWidth; i++)
				{
					for(int j = -1; j <= 1; j++)
					{
						int x = i * j;
						for(int k = -1; k <= 1; k++)
						{
							int y = i * k;
	
							float2 changePixPos = curPixPos + float2(x, y);
							fixed2 changeUV = changePixPos / _MainTex_TexelSize.zw;

							fixed4 changeCol = tex2D(_MainTex, changeUV);
							if(changeCol.a > 0)
							{
								edgeColor = fixed4(1, 1, 1, 1);
							}
						}
					}
				}





				return edgeColor;
 			}
			
			ENDCG
		}
	}
	FallBack Off
}
