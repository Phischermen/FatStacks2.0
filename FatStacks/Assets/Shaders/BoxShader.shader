Shader "Custom/BoxShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _FrameColor ("Frame/Icon Color", Color) = (1,0,0,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _FrameTex ("Fram/Icon (RGB)", 2D) = "white" {}
    }
	SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Lambert
			#pragma target 3.0

			float4 _Color;
			float4 _FrameColor;
			sampler2D _MainTex;
			sampler2D _FrameTex;

			struct Input {
				float2 uv_MainTex;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				float4 c1 = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				float4 c2 = tex2D(_FrameTex, IN.uv_MainTex) * _FrameColor;
				o.Albedo = c1.rgb * (1 - c2.a) + (c2.rgb * c2.a);
			}
			ENDCG
	}
    FallBack "Diffuse"
}
