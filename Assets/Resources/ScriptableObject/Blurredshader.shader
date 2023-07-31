Shader "Custom/BlurredShader"
{
	//-----------------------------【属性】-----------------------------
	Properties {
		_MainTex ("Main Tex", 2D) = "white" {}

		// 边缘
		_Width ("Edge Width", Range(0, 200)) = 0
        _Strength ("强度", Range(0, 10)) = 1
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
			float4 _MainTex_TexelSize;

			int _Width;
			fixed _Strength;

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
                fixed4 curCol = tex2D(_MainTex, data.uv);
                float2 curPixPos = _MainTex_TexelSize.zw * data.uv;

				// 过度边
                if(curCol.a < 1)
                {
				    half excessiveValue = 0;
				    int num = 0;
				    [loop]for(int i = -_Width; i <= _Width; i++)
				    {
                        [loop]for(int j = -_Width; j <= _Width; j++)
				        {
                            float2 customPos = curPixPos + float2(i, j);
                            fixed2 customUV = customPos / _MainTex_TexelSize.zw;
                            fixed4 customCol = tex2D(_MainTex, customUV);
                            excessiveValue += customCol.a;
                            num++;
				        }
				    }
				    excessiveValue /= num;
				    curCol = fixed4(1, 1, 1, excessiveValue * _Strength);
                }

				return curCol;
 			}
			
			ENDCG
		}
	}
	FallBack Off
}
