Shader "Fog Of War/Entity" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TeamMask ("Colour Mask (RGB)", 2D) = "black" {}
		_Color ("Tint Color", Color) = (1, 1, 1, 1)
		_TeamColor ("Team Color", Color) = (0, 1, 0, 1)
		_FOWColor ("FOW Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert
        #include "FOWIncludes.cginc"

		sampler2D _MainTex;
		sampler2D _TeamMask;
		half4 _Color;
		half4 _TeamColor;
		half4 _FOWColor;

		struct Input {
			float2 uv_MainTex;
			float2 uv_TeamMask;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			half4 m = tex2D (_TeamMask, IN.uv_TeamMask);
			c.rgb = lerp(c.rgb, _TeamColor.rgb * m.rgb, m.a);
			float4 t = TransformColourFOW(c * _Color, _FOWColor);
			o.Albedo = t.rgb;
			o.Alpha = t.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
