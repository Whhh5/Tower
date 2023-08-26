// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Terrain"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "black" {}
        _MainColor ("Main Color", color) = (1, 1, 1, 1)
        _RoadTex ("Road Tex", 2D) = "black" {}
        _RoadMainTex ("Road Main Tex", 2D) = "black" {}
        _GrassTex ("Grass Tex", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass{

            CGPROGRAM

            #pragma vertex FuncVertex
            #pragma fragment FuncFragment
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            fixed4 _MainColor;
            
            sampler2D _RoadTex;
            sampler2D _RoadMainTex;
            float4 _RoadTex_TexelSize;;


            sampler2D _GrassTex;


            struct VertexData{
                float4 localPos : POSITION;
                fixed2 uv : TEXCOORD;
            };
            struct FragmentData{
                float4 pos : SV_POSITION;
                fixed2 uv : TEXCOORD0;
                // 路面纹理 uv
                fixed2 worldRoadUV : TEXCOORD1;
                // 草纹理 uv
                fixed2 worldGrassUV : TEXCOORD2;
            };


            void FuncVertex(VertexData f_InData, out FragmentData R_FragData)
            {
                // 转换世界坐标
                float3 worldPos = mul(unity_ObjectToWorld, f_InData.localPos).xyz;
                
                // 转换裁剪坐标
                R_FragData.pos = UnityObjectToClipPos(f_InData.localPos);
                R_FragData.uv = f_InData.uv;


                // 世界 uv 赋值
                R_FragData.worldRoadUV = frac(worldPos.xz / _RoadTex_TexelSize.zw);
                R_FragData.worldGrassUV =  frac(worldPos.xz / _RoadTex_TexelSize.zw);
            }

            
            void FuncFragment(FragmentData f_InData, out fixed4 R_Color: SV_TARGET)
            {
                // 路面
                fixed roadValue = tex2D(_RoadTex, f_InData.uv).r;
                fixed4 roadTex = tex2D(_RoadMainTex, f_InData.worldRoadUV).r;
                fixed4 roadCol = roadValue * roadTex;


                // 草地


                R_Color = roadCol;
            }





            ENDCG
        }
    }
    FallBack "Diffuse"
}
