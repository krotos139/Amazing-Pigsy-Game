Shader "Hidden/NewImageEffectShader"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_MaskTex("Mask texture", 2D) = "white" {}
		_DisplacementTex("Displacement Tex", 2D) = "white" {}
		_maskBlend("Mask blending", Float) = 0.5
		_maskSize("Mask Size", Float) = 1
		_Strength("Strength", Range(0, 1)) = 0
	}
		SubShader{
		Pass{
		CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		uniform sampler2D _MaskTex;
		uniform sampler2D _DisplacementTex;
		uniform float _Strength;

		fixed _maskBlend;
		fixed _maskSize;

		float4 frag(v2f_img i) : COLOR{
			
			half2 n = tex2D(_DisplacementTex, i.uv);
			half2 d = n * 2 - 1;
			i.uv += d * _Strength;
			i.uv = saturate(i.uv);

			fixed4 mask = tex2D(_MaskTex, i.uv * _maskSize);
			fixed4 base = tex2D(_MainTex, i.uv);
			return lerp(base, mask, _maskBlend);
		}
		ENDCG
	}
	}
}
