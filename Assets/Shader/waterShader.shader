Shader "Custom/Waves"{
	Properties{
		_MainColor("MainColor", Color) = (0,0,0,1)
		_Strength("Strength",Range(0,2)) = 1.0
		_Speed("Speed",Range(-200,200)) = 100
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
		SubShader{
		Tags{
				"RenderType" = "transparent"
			}
		Pass
			{
		Cull Off

		CGPROGRAM
		#pragma vertex vertexFunc
		#pragma fragment fragmentFunc

		float4 _MainColor;
		float _Strength;
		float _Speed;

		struct vertexInput {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};
		struct vertexOutput {
			float2 uv : TEXCOORD0;
			float4 pos : SV_POSITION;
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;

		vertexOutput vertexFunc(vertexInput IN) {
			vertexOutput o;

			float4 worldPos = mul(unity_ObjectToWorld, IN.vertex);
			float displacement = (cos(worldPos.y) + cos(worldPos.z + (_Speed*_Time)));
			worldPos.y = worldPos.y + (displacement * _Strength);
			o.pos = mul(UNITY_MATRIX_VP, worldPos);
			o.uv = IN.uv;
			return o;
		}
		float4 fragmentFunc(vertexOutput IN) : COLOR{
			_MainColor = tex2D(_MainTex, IN.uv);
			return _MainColor;
		}
			ENDCG
		}
	}
}