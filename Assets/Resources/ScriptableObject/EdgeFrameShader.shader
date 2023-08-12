Shader "Custom/EdgeFrameShader" 
{
	//-----------------------------【属性】-----------------------------
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		// 描边程度
		_EdgeOnly ("Edge Only", Float) = 1.0
		// 边缘颜色
		_EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
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
			
			sampler2D _MainTex;  
			uniform half4 _MainTex_TexelSize;
			fixed _EdgeOnly;
			fixed4 _EdgeColor;

			struct appdata
			{
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				half2 ver[9] : TEXCOORD1;
			};
			  
			//-----------------------------【顶点着色器】-----------------------------
			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				
				half2 uv = v.texcoord;
				//计算周围像素的纹理坐标位置，其中4为原始点，
				o.ver[0] = uv + _MainTex_TexelSize.xy * half2(-1, -1);
				o.ver[1] = uv + _MainTex_TexelSize.xy * half2(0, -1);
				o.ver[2] = uv + _MainTex_TexelSize.xy * half2(1, -1);
				o.ver[3] = uv + _MainTex_TexelSize.xy * half2(-1, 0);
				o.ver[4] = uv + _MainTex_TexelSize.xy * half2(0, 0);		//原点
				o.ver[5] = uv + _MainTex_TexelSize.xy * half2(1, 0);
				o.ver[6] = uv + _MainTex_TexelSize.xy * half2(-1, 1);
				o.ver[7] = uv + _MainTex_TexelSize.xy * half2(0, 1);
				o.ver[8] = uv + _MainTex_TexelSize.xy * half2(1, 1);
				o.uv = uv;
				return o;
			}
			
			// 转换为灰度
			fixed luminance(fixed4 color) {
				return  0.299 * color.r + 0.587 * color.g + 0.114 * color.b; 
			}
			
			// sobel算子
			half Sobel(v2f i) {
				const half Gx[9] = {-1,  0,  1,
									-2,  0,  2,
									-1,  0,  1};
				const half Gy[9] = {-1, -2, -1,
									0,  0,  0,
									1,  2,  1};
				
				half texColor;
				half edgeX = 0;
				half edgeY = 0;
				for (int it = 0; it < 9; it++) 
				{
					// 转换为灰度值
					texColor = luminance(tex2D(_MainTex, i.ver[it]));

					edgeX += texColor * Gx[it];
					edgeY += texColor * Gy[it];
				}
				// 合并横向和纵向
				half edge = 1 - (abs(edgeX) + abs(edgeY));
				return edge;
			}
			//-----------------------------【片元着色器】-----------------------------
			fixed4 frag(v2f data) : SV_Target {
				half edge = Sobel(data);
				fixed4 originCol = fixed4(0, 0, 0, 0);
				fixed4 edgeColor = lerp(_EdgeColor, originCol, edge);
				edgeColor = lerp(originCol, edgeColor, _EdgeOnly);
				// fixed4 edgeColor = lerp(_EdgeColor, tex2D(_MainTex, data.ver[4]), edge);
				// edgeColor = lerp(tex2D(_MainTex, data.ver[4]),edgeColor, _EdgeOnly);
				return edgeColor;
 			}
			
			ENDCG
		}
	}
	FallBack Off
}
