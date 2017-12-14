Shader "Unlit/YUV"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Witdh ("Width", Float) = 1280.0
        _Height ("Height", Float) = 960.0
        _YSize ("YSize", Float) = 1228800.0 // _Witdh * _Height;
        _USize ("USize", Float) = 307200.0 // _YSize / 4;
        _FrameSize ("FrameSize", Float) = 1843200.0 // _YSize * 1.5f;
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull off //duty

       Pass
        {
            CGPROGRAM

           #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            
           #include "UnityCG.cginc"

           struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

           struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

           static const float3x3 yuvCoef =
            {
                1.0f,  0.0f, 1.403f,
                1.0f, -0.344f, -0.714f,
                1.0f, 1.77f, 0.0
            };

           sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Witdh;
            float _Height;
            float _YSize;
            float _USize;
            float _FrameSize;

           v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
           fixed4 frag (v2f input) : SV_Target
            {
                float i = input.uv.x * _Witdh;
                float j = input.uv.y * _Height;

               float iSub = floor(i / 2);
                float jSub = floor(j / 2);
                float uOffset = jSub * (_Witdh / 2) + iSub;

               float uIndex = _YSize + uOffset;
                float vIndex = uIndex + _USize;

               float ux = (uIndex % _Witdh) / _Witdh;
                float uy = uIndex / _FrameSize;
                float vx = (vIndex % _Witdh) / _Witdh;
                float vy = vIndex / _FrameSize;

               float y = tex2D(_MainTex, float2(input.uv.x, input.uv.y / 1.5f));
                float u = tex2D(_MainTex, float2(ux, uy));
                float v = tex2D(_MainTex, float2(vx, vy));

               if(y == 0.0f && u == 0.0f && v == 0.0f)
                {
                    return fixed4(0.0f, 0.0f, 0.0f, 1.0f);
                }
                else
                {
                    float3 yuv = float3(y, u - 0.5f, v - 0.5f);
                    return float4(mul(yuvCoef, yuv) , 1.0f);
                }
            }

           ENDCG
        }
    }
}
