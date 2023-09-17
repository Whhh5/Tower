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
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 200

        Pass{
                         Tags { "LightMode"="ForwardBase" }
             
             // �ر����д��
             ZWrite Off
             // �������ģʽ�������û������ΪSrcAlpha��OneMinusSrcAlpha
             Blend SrcAlpha OneMinusSrcAlpha
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
                // ·������ uv
                fixed2 worldRoadUV : TEXCOORD1;
                // ������ uv
                fixed2 worldGrassUV : TEXCOORD2;
            };


            void FuncVertex(VertexData f_InData, out FragmentData R_FragData)
            {
                // ת����������
                float3 worldPos = mul(unity_ObjectToWorld, f_InData.localPos).xyz;
                
                // ת���ü�����
                R_FragData.pos = UnityObjectToClipPos(f_InData.localPos);
                R_FragData.uv = f_InData.uv;


                // ���� uv ��ֵ
                R_FragData.worldRoadUV = frac(worldPos.xz / _RoadTex_TexelSize.zw);
                R_FragData.worldGrassUV =  frac(worldPos.xz / _RoadTex_TexelSize.zw);
            }

            
            void FuncFragment(FragmentData f_InData, out fixed4 R_Color: SV_TARGET)
            {
                // ·��
                fixed4 roadValue = tex2D(_RoadTex, f_InData.uv);
                fixed4 roadTex = tex2D(_RoadMainTex, f_InData.worldRoadUV);
                fixed4 roadCol = roadValue.r > 0.1 ? roadTex : 0;

                roadCol.a = roadValue.r > 0.1 ? 0.8 : 1;

                // �ݵ�


                R_Color = roadCol;
            }





            ENDCG
        }
    }
    FallBack "Diffuse"
}
