Shader "Fog Of War/Entity Transparent" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert alpha
        #include "FOWIncludes.cginc"

		sampler2D _MainTex;
		sampler2D _FOWTex;
		float4 _FOWTex_ST;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			half4 fow = tex2D(_FOWTex, TRANSFORM_TEX(IN.worldPos.xz, _FOWTex));
			half4 t = TransformColourFOWAO(c, fow);
			o.Albedo = t.rgb;
			o.Alpha = t.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
