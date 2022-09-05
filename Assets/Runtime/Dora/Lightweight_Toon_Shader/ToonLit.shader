Shader "Largelabs/ToonLit" {
	Properties {
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {} 
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
	
		
CGPROGRAM

#pragma surface surf ToonRamp

sampler2D _Ramp;

inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half atten)
{
	half d = dot(s.Normal, lightDir) * 0.5 + 0.5;
	half3 ramp = tex2D (_Ramp, half2(d,d)).rgb;
	
	half4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
	c.a = 0;
	return c;
}


uniform fixed4 _Color;

struct Input {
	half2 uv_MainTex : TEXCOORD0;
};

void surf (Input IN, inout SurfaceOutput o) {
	o.Albedo = _Color.rgb;
}
ENDCG

	} 

	Fallback "Diffuse"
}
