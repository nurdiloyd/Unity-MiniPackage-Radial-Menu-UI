// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Waterline"
{
	Properties
	{
		_WaterColor("Water Color", Color) = (0.004716992,0.9610302,1,0)
		_FoamColor("Foam Color", Color) = (0,0,0,0)
		_WaterScale("Water Scale", Float) = 1
		_Displace("Displace", Float) = 1
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_WaterFresnel("Water Fresnel", Float) = 0
		_DisplaceNormal("Displace Normal", Color) = (1,0.9847543,0,1)
		_Tesselation("Tesselation", Float) = 0
		_FoamIntensity("Foam Intensity", Range( 0 , 1)) = 1
		_EmissionContribute("Emission Contribute", Float) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float _WaterScale;
		uniform float _Displace;
		uniform float4 _DisplaceNormal;
		uniform float _WaterFresnel;
		uniform float4 _WaterColor;
		uniform float4 _FoamColor;
		uniform float _FoamIntensity;
		uniform float _EmissionContribute;
		uniform float _Smoothness;
		uniform float _Tesselation;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			float4 temp_cast_1 = (_Tesselation).xxxx;
			return temp_cast_1;
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 appendResult3 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 panner43 = ( 1.0 * _Time.y * float2( 0,0 ) + appendResult3);
			float simplePerlin2D39 = snoise( panner43*( 0.5 * _WaterScale ) );
			simplePerlin2D39 = simplePerlin2D39*0.5 + 0.5;
			float2 appendResult25 = (float2(-1.0 , 0.0));
			float2 panner6 = ( 1.0 * _Time.y * appendResult25 + appendResult3);
			float simplePerlin2D1 = snoise( panner6*_WaterScale );
			simplePerlin2D1 = simplePerlin2D1*0.5 + 0.5;
			float temp_output_41_0 = ( simplePerlin2D39 * simplePerlin2D1 );
			v.vertex.xyz += ( ( temp_output_41_0 * _Displace ) * _DisplaceNormal ).rgb;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV27 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode27 = ( 0.0 + 2.09 * pow( 1.0 - fresnelNdotV27, 2.55 ) );
			float3 temp_cast_0 = (0.35).xxx;
			float2 appendResult3 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 panner43 = ( 1.0 * _Time.y * float2( 0,0 ) + appendResult3);
			float simplePerlin2D39 = snoise( panner43*( 0.5 * _WaterScale ) );
			simplePerlin2D39 = simplePerlin2D39*0.5 + 0.5;
			float2 appendResult25 = (float2(-1.0 , 0.0));
			float2 panner6 = ( 1.0 * _Time.y * appendResult25 + appendResult3);
			float simplePerlin2D1 = snoise( panner6*_WaterScale );
			simplePerlin2D1 = simplePerlin2D1*0.5 + 0.5;
			float temp_output_41_0 = ( simplePerlin2D39 * simplePerlin2D1 );
			float3 temp_cast_1 = (temp_output_41_0).xxx;
			float temp_output_5_0 = saturate( ( 1.0 - ( ( distance( temp_cast_0 , temp_cast_1 ) - 0.13 ) / max( 0.09 , 1E-05 ) ) ) );
			float4 clampResult49 = clamp( ( ( fresnelNode27 * _WaterFresnel ) + ( _WaterColor + ( ( temp_output_5_0 * _FoamColor ) * _FoamIntensity ) ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			o.Albedo = clampResult49.rgb;
			o.Emission = ( clampResult49 * _EmissionContribute ).rgb;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc tessellate:tessFunction 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17800
93;311;1793;490;-227.6518;532.4686;1.3;True;True
Node;AmplifyShaderEditor.WorldPosInputsNode;2;-1180.437,-276.7101;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;25;-915.2022,82.40833;Inherit;False;FLOAT2;4;0;FLOAT;-1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;3;-911.6357,-237.602;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-830.8931,177.0646;Inherit;False;Property;_WaterScale;Water Scale;2;0;Create;True;0;0;False;0;1;-0.22;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;6;-621.5729,-87.85843;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-572.5043,-391.1424;Inherit;False;2;2;0;FLOAT;0.5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;43;-585.4991,-269.1052;Inherit;False;3;0;FLOAT2;1,1;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;39;-257.7665,-359.6686;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;1;-276.2236,-60.73183;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;20.45054,-292.6459;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;282.5336,-57.1705;Inherit;False;Constant;_Float3;Float 3;11;0;Create;True;0;0;False;0;0.35;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;38;372.0135,-368.9843;Inherit;False;Property;_FoamColor;Foam Color;1;0;Create;True;0;0;False;0;0,0,0,0;0.990566,0.990566,0.990566,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;5;555.9579,-192.7085;Inherit;True;Color Mask;-1;;1;eec747d987850564c95bde0e5a6d1867;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;0.5,0.5,0.5;False;4;FLOAT;0.13;False;5;FLOAT;0.09;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;1180.037,-21.19998;Inherit;False;Property;_FoamIntensity;Foam Intensity;12;0;Create;True;0;0;False;0;1;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;969.9269,-253.4136;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;36;987.2087,-401.5995;Inherit;False;Property;_WaterFresnel;Water Fresnel;9;0;Create;True;0;0;False;0;0;0.07;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;27;780.7873,-652.4219;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;2.09;False;3;FLOAT;2.55;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;11;365.6759,-545.5674;Inherit;False;Property;_WaterColor;Water Color;0;0;Create;True;0;0;False;0;0.004716992,0.9610302,1,0;0.003921568,0.5479257,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;1327.635,-244.4578;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;1330.109,-497.1647;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;17;1473.115,-306.761;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-97.51765,403.0139;Inherit;False;Property;_Displace;Displace;3;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;1785.39,-296.2974;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;45;1839.485,308.5168;Inherit;False;Property;_DisplaceNormal;Displace Normal;10;0;Create;True;0;0;False;0;1,0.9847543,0,1;0,0,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;49;2591.923,-136.1233;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;51;2515.493,193.9137;Inherit;False;Property;_EmissionContribute;Emission Contribute;13;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;324.8871,251.1033;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;32;51.53469,463.3067;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;1997.066,70.57256;Inherit;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;30;1382.204,176.588;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;33;2030.722,553.7294;Inherit;True;Color Mask;-1;;2;eec747d987850564c95bde0e5a6d1867;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;0.5,0.5,0.5;False;4;FLOAT;0.28;False;5;FLOAT;0.41;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;14;967.9883,325.1257;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;20;938.4953,497.5549;Inherit;True;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;15;578.7922,169.8951;Inherit;False;Property;_OpacityMin;Opacity Min;5;0;Create;True;0;0;False;0;0;0.643;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;2637.393,388.4009;Inherit;False;Property;_Tesselation;Tesselation;11;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;1161.905,120.4333;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;23;1391.711,650.7805;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;2180.716,261.1627;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;688.643,483.862;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;22;1180.041,601.463;Inherit;False;Property;_WaterState;Water State;7;0;Create;True;0;0;False;0;1.01;3.24;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;565.949,325.8485;Inherit;False;Property;_OpacityMax;Opacity Max;6;0;Create;True;0;0;False;0;1;0.72;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;920.0077,159.3097;Inherit;False;Property;_WaveOpacity;Wave Opacity;8;0;Create;True;0;0;False;0;0;0.13;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;2729.148,94.90253;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;21;1536.179,550.3331;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.9;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;1185.718,750.6219;Inherit;False;Constant;_Float2;Float 2;8;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2944.398,-57.58625;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;Waterline;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.82;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;7.3;10;25;False;5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.11;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;2;1
WireConnection;3;1;2;3
WireConnection;6;0;3;0
WireConnection;6;2;25;0
WireConnection;40;1;4;0
WireConnection;43;0;3;0
WireConnection;39;0;43;0
WireConnection;39;1;40;0
WireConnection;1;0;6;0
WireConnection;1;1;4;0
WireConnection;41;0;39;0
WireConnection;41;1;1;0
WireConnection;5;1;41;0
WireConnection;5;3;42;0
WireConnection;37;0;5;0
WireConnection;37;1;38;0
WireConnection;47;0;37;0
WireConnection;47;1;48;0
WireConnection;34;0;27;0
WireConnection;34;1;36;0
WireConnection;17;0;11;0
WireConnection;17;1;47;0
WireConnection;28;0;34;0
WireConnection;28;1;17;0
WireConnection;49;0;28;0
WireConnection;7;0;41;0
WireConnection;7;1;8;0
WireConnection;30;0;29;0
WireConnection;30;1;14;0
WireConnection;33;1;21;0
WireConnection;14;0;1;0
WireConnection;14;1;15;0
WireConnection;14;2;16;0
WireConnection;20;0;19;0
WireConnection;29;0;5;0
WireConnection;29;1;31;0
WireConnection;23;0;22;0
WireConnection;23;1;18;0
WireConnection;44;0;7;0
WireConnection;44;1;45;0
WireConnection;50;0;49;0
WireConnection;50;1;51;0
WireConnection;21;0;20;0
WireConnection;21;1;22;0
WireConnection;21;2;23;0
WireConnection;0;0;49;0
WireConnection;0;2;50;0
WireConnection;0;4;10;0
WireConnection;0;11;44;0
WireConnection;0;14;46;0
ASEEND*/
//CHKSM=525E4E0B094B3854C9869B9568F5E10931FE35E4