Shader "BoxHighlight" {
	Properties
	{
		_Color("Color (RGBA)", Color) = (1, 1, 1, 1) // add _Color property
	}
	SubShader{
		Tags { "RenderType" = "Transparent" "Queue" = "Overlay"}
		ZWrite Off
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass {
			Stencil {
				Ref 0
				Comp equal
				Pass IncrWrap
			}
			CGPROGRAM
			#pragma vertex vert alpha
			#pragma fragment frag alpha
			struct appdata {
				float4 vertex : POSITION;
			};
			struct v2f {
				float4 pos : SV_POSITION;
			};

			float4 _Color;

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			half4 frag(v2f i) : SV_Target {
				return _Color;
			}
			ENDCG
		}
	}
}