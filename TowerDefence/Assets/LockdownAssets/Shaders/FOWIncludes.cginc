half4 LightingNoLighting(SurfaceOutput s, half3 lightDir, half atten) {
	return half4(s.Albedo * 0.75f, s.Alpha);
}


void GrayBrightFromFOW(half4 fow, out half lightness, out half grayscale) {
	grayscale = fow.b;
	fow.rg = saturate(fow.rg * 5 - 2);
	lightness = (fow.r + fow.g * (1 + fow.b)) / 3;
}

half4 TransformColourFOW(half4 c, half4 fow) {
	half lightness, grayscale;
	GrayBrightFromFOW(fow, lightness, grayscale);
	half3 t = c.rgb * lightness;
	return half4(lerp(dot(t, half3(0.5f, 0.4f, 0.1f)).rrr, t.rgb, grayscale), c.a);
}
half4 TransformColourFOWAO(half4 c, half4 fow) {
	half lightness, grayscale;
	GrayBrightFromFOW(fow, lightness, grayscale);
	half3 t = c.rgb * lightness * fow.a;
	return half4(lerp(dot(t, half3(0.5f, 0.4f, 0.1f)).rrr, t.rgb, grayscale), c.a);
}
