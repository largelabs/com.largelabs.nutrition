Shader "Largelabs/Smoke"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_Alpha("_Alpha", Range(0,1)) = 1
		_Wind("_Wind", Range(0,5)) = 2

			//seed ("Rotation", Float) = 0

	}

		SubShader
		{

		   Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		   Blend SrcAlpha OneMinusSrcAlpha
			//Blend One One
			//Blend OneMinusDstColor One // Soft Additive

			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			ColorMaterial AmbientAndDiffuse

			 Pass
			 {
				 CGPROGRAM
					 #pragma target 3.0
					 #pragma vertex vert
					 #pragma fragment frag
					 #include "UnityCG.cginc"

					 uniform sampler2D _MainTex;
					 uniform float4 _MainTex_ST;
					 uniform float _Alpha;
					 uniform float _Wind;

					 //float seed;

					 struct vertexInput
					{
						float4 vertex : POSITION;
						float2 uv : TEXCOORD0;
						float4 color : COLOR;
					};

					 struct v2f
					 {
						 float4 pos : POSITION;
						 float2  uv : TEXCOORD0;
						 float4 col : COLOR;
					 };

					 v2f vert(vertexInput v)
					 {
						v2f o;

						o.uv = TRANSFORM_TEX(v.uv, _MainTex);

						//float d = sin(o.uv.y*15 - _Time.x*3 + seed*13.417) * o.uv.y;
						//d += sin(o.uv.y*20+o.uv.x - _Time.y*0.5 + seed) * lerp(-0.2, 0.2, o.uv.x);
						//d += lerp(-0.3, 1.4, o.uv.y*o.uv.y*o.uv.y) * lerp(-1, 1, o.uv.x);

					 //	float wind = 2;

						float d = o.uv.y * _Wind;	// Wind strength
						d += lerp(-0.5, 0.5, o.uv.x) * lerp(-0.75, 2.5, o.uv.y * o.uv.y);	// Larger at the top
						d += sin(o.uv.y * (25 + 5 * o.uv.x) - _Time.y + o.uv.x * 2.0) * o.uv.y * lerp(-0.5, 0.5, o.uv.x) * 2;	// Smoke waves - little
						d += sin(o.uv.y * 4 - _Time.x) * 5 * (o.uv.y * o.uv.y);	// Smoke waves - large

						//d = clamp(d,0,10000);

						v.vertex.x += d;
						o.col = v.color;


						o.pos = UnityObjectToClipPos(v.vertex);



						return o;
					 }

					 fixed4 frag(v2f i) : COLOR
					 {
						fixed4 main = tex2D(_MainTex, i.uv);
					 //main.a *= 1-i.uv.y;
					 main.a *= _Alpha;

					 return main * i.col;
				  }
			  ENDCG
		 }
		}
}