// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "fiber_face_tile"
{
	Properties
	{
		_FresnelScale("Fresnel Scale", Float) = 1
		_EmissionMultiplier("Emission Multiplier", Float) = 1
		_FresnelPower("Fresnel Power", Float) = 6
		_Fresnel_color("Fresnel_color", Color) = (0.3773585,0.3773585,0.3773585,0)
		_CellScale("Cell Scale", Range( 0 , 5)) = 4.088258
		_CellSharpness("Cell Sharpness", Range( 0.01 , 1)) = 0.01
		_FresnelAmount("Fresnel Amount", Range( 0 , 1)) = 0
		_CellAmount("Cell Amount", Range( 0 , 1)) = 1
		_SkinColor("Skin Color", Color) = (1,0,0,0)
		_Emission_Color_contribute("Emission_Color_contribute", Range( 0 , 1)) = 1
		_face_tile_test("face_tile_test", 2D) = "white" {}
		_OffsetX("Offset X", Int) = 1
		_OffsetY("Offset Y", Int) = 1
		_TileCount("Tile Count", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 5.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			float3 worldPos;
		};

		uniform float4 _SkinColor;
		uniform sampler2D _face_tile_test;
		uniform float _TileCount;
		uniform int _OffsetX;
		uniform int _OffsetY;
		uniform float _Emission_Color_contribute;
		uniform float _CellSharpness;
		uniform float _CellScale;
		uniform float _CellAmount;
		uniform float4 _Fresnel_color;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		uniform float _EmissionMultiplier;
		uniform float _FresnelAmount;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 Albedo110 = _SkinColor;
			float clampResult204 = clamp( _TileCount , 1.0 , 16.0 );
			float TileValue188 = ( 1.0 / clampResult204 );
			float2 temp_cast_0 = (TileValue188).xx;
			float2 appendResult215 = (float2(( _OffsetX * TileValue188 ) , ( _OffsetY * TileValue188 )));
			float2 uv_TexCoord213 = i.uv_texcoord * temp_cast_0 + appendResult215;
			float4 tex2DNode121 = tex2D( _face_tile_test, uv_TexCoord213 );
			float4 Diffuse54 = ( ( Albedo110 * ( 1.0 - tex2DNode121.a ) ) + ( tex2DNode121 * tex2DNode121.a ) );
			o.Albedo = Diffuse54.rgb;
			float4 temp_output_106_0 = ( Diffuse54 * _Emission_Color_contribute );
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult74 = dot( ase_worldNormal , ase_worldlightDir );
			float smoothstepResult81 = smoothstep( 1.0 , ( 1.0 - _CellSharpness ) , ( dotResult74 * _CellScale ));
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV32 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode32 = ( 0.0 + _FresnelScale * pow( 1.0 - fresnelNdotV32, _FresnelPower ) );
			float smoothstepResult44 = smoothstep( 0.01 , 0.11 , fresnelNode32);
			float4 temp_output_34_0 = ( _Fresnel_color * smoothstepResult44 );
			float4 Emission57 = ( temp_output_34_0 * _EmissionMultiplier );
			float4 temp_output_88_0 = ( ( ( 1.0 - smoothstepResult81 ) * _CellAmount ) + ( Emission57 * _FresnelAmount ) );
			float4 temp_cast_2 = (1.0).xxxx;
			float4 lerpResult99 = lerp( ( temp_output_88_0 + Emission57 ) , temp_cast_2 , temp_output_88_0);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			o.Emission = ( ( temp_output_106_0 + lerpResult99 ) * ase_lightColor ).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
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
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
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
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
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
				surfIN.uv_texcoord = IN.customPack1.xy;
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
117;127;1620;859;4687.407;3490.675;7.509429;True;True
Node;AmplifyShaderEditor.CommentaryNode;205;-1520.745,-2311.84;Inherit;False;738;308.6858;Comment;4;148;204;188;222;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;222;-1497.037,-2195.884;Inherit;False;Property;_TileCount;Tile Count;16;0;Create;True;0;0;False;0;1;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;204;-1326.745,-2162.154;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;16;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;221;-1498.037,-2270.884;Inherit;False;Constant;_Float1;Float 1;16;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;148;-1158.554,-2261.84;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;188;-1006.745,-2258.154;Inherit;False;TileValue;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;200;-1408.728,-1450.064;Inherit;False;188;TileValue;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;223;-1435.818,-1286.694;Inherit;False;188;TileValue;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;224;-1420.83,-1366.044;Inherit;False;Property;_OffsetY;Offset Y;15;0;Create;True;0;0;False;0;1;3;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;219;-1393.74,-1529.414;Inherit;False;Property;_OffsetX;Offset X;14;0;Create;True;0;0;False;0;1;-6;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-1403.869,-36.49871;Inherit;False;Property;_FresnelScale;Fresnel Scale;0;0;Create;True;0;0;False;0;1;2.61;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-1406.165,39.08712;Inherit;False;Property;_FresnelPower;Fresnel Power;2;0;Create;True;0;0;False;0;6;6.81;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;225;-1171.985,-1283.253;Inherit;False;2;2;0;INT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;32;-1157.396,-74.66151;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;15.16;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;218;-1144.895,-1446.623;Inherit;False;2;2;0;INT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;45;-923.7664,-309.7269;Inherit;False;Property;_Fresnel_color;Fresnel_color;3;0;Create;True;0;0;False;0;0.3773585,0.3773585,0.3773585,0;0.1886792,0.1886792,0.1886792,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;44;-806.7316,22.31692;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.01;False;2;FLOAT;0.11;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;82;1236.94,-826.953;Inherit;True;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;62;1297.642,-1057.901;Inherit;True;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;215;-928.7261,-1415.558;Inherit;False;FLOAT2;4;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-194.9485,32.87444;Inherit;True;Property;_EmissionMultiplier;Emission Multiplier;1;0;Create;True;0;0;False;0;1;1.06;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;95;-1636.602,-477.8796;Inherit;False;Property;_SkinColor;Skin Color;8;0;Create;True;0;0;False;0;1,0,0,0;0.9339623,0.6542633,0.4449537,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-578.3167,-74.71841;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;74;1540.768,-792.0029;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;84;1448.094,-538.0249;Inherit;False;Property;_CellSharpness;Cell Sharpness;5;0;Create;True;0;0;False;0;0.01;0.01;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;213;-685.7261,-1338.558;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;83;1603.351,-411.0466;Inherit;False;Constant;_Float0;Float 0;12;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;1254.297,-650.2227;Inherit;False;Property;_CellScale;Cell Scale;4;0;Create;True;0;0;False;0;4.088258;2.31;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;110;-1358.629,-519.5924;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;85;1889.95,-563.5461;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;121;-336,-1360;Inherit;True;Property;_face_tile_test;face_tile_test;11;0;Create;True;0;0;False;0;-1;a6801e0b535e33c46965d5aef22e88fc;a6801e0b535e33c46965d5aef22e88fc;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;1893.368,-844.3062;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;271.0287,-69.60459;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;57;540.5894,-67.19176;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;81;2296.006,-787.6517;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.36;False;2;FLOAT;0.47;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;226;29.7842,-771.9061;Inherit;True;110;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;179;68.93378,-1036.17;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;120;2507.088,-750.0876;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;176;315.9337,-1314.37;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;91;2279.227,-249.5705;Inherit;False;Property;_FresnelAmount;Fresnel Amount;6;0;Create;True;0;0;False;0;0;0.075;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;56;2139.59,-447.3733;Inherit;True;57;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;180;449.6338,-737.1698;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;92;2308.069,-573.6096;Inherit;False;Property;_CellAmount;Cell Amount;7;0;Create;True;0;0;False;0;1;0.07;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;2634.487,-711.4379;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;2650.511,-382.9327;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;122;698.3568,-1064.518;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;54;1000.373,-1121.804;Inherit;True;Diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;3295.219,-320.4683;Inherit;False;57;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;88;2862.725,-610.4011;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;101;2925.728,-227.5339;Inherit;False;Constant;_Float1;Float 1;11;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;3320.969,-978.1247;Inherit;True;54;Diffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;105;3421.438,-568.6997;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;108;3292.974,-730.91;Inherit;False;Property;_Emission_Color_contribute;Emission_Color_contribute;10;0;Create;True;0;0;False;0;1;0.643;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;99;3781.755,-664.6686;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;3656.065,-867.0294;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;117;4435.377,-966.989;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;111;4421.575,-854.9538;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;114;-1023.685,-707.5808;Inherit;False;113;Emission_Multiplier;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;4438.052,-470.142;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;187;-979.2,-1083.2;Inherit;False;2;0;INT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;145;-757.7,-978.9;Inherit;True;FLOAT2;4;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;46;-122.5806,-278.6184;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;4689.481,-865.3475;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;112;4304.296,-1520.991;Inherit;True;54;Diffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.IntNode;186;-1283.2,-891.2;Inherit;False;Property;_OfsetY;Ofset Y;12;0;Create;True;0;0;False;0;1;1;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;3798.284,-388.8921;Inherit;False;Property;_Smoothness;Smoothness;9;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;119;1880.487,-324.9879;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;113;3676.01,-1091.317;Inherit;False;Emission_Multiplier;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;115;-770.7383,-692.1241;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;185;-1283.2,-1083.2;Inherit;False;Property;_OfsetX;Ofset X;13;0;Create;True;0;0;False;0;1;1;0;1;INT;0
Node;AmplifyShaderEditor.GetLocalVarNode;227;-847.2334,-457.4613;Inherit;False;54;Diffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;189;-1318.2,-982.2;Inherit;False;188;TileValue;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;190;-979.2,-891.2;Inherit;False;2;0;INT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;116;-574.4365,-447.9059;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;33;-402.0706,-447.2412;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;183;-374.4988,-988.4241;Inherit;False;Ofset;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;100;2947.847,-357.6456;Inherit;False;Constant;_Float0;Float 0;11;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;4957.161,-1405.174;Float;False;True;-1;7;ASEMaterialInspector;0;0;Standard;fiber_face_tile;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;204;0;222;0
WireConnection;148;0;221;0
WireConnection;148;1;204;0
WireConnection;188;0;148;0
WireConnection;225;0;224;0
WireConnection;225;1;223;0
WireConnection;32;2;39;0
WireConnection;32;3;36;0
WireConnection;218;0;219;0
WireConnection;218;1;200;0
WireConnection;44;0;32;0
WireConnection;215;0;218;0
WireConnection;215;1;225;0
WireConnection;34;0;45;0
WireConnection;34;1;44;0
WireConnection;74;0;62;0
WireConnection;74;1;82;0
WireConnection;213;0;200;0
WireConnection;213;1;215;0
WireConnection;110;0;95;0
WireConnection;85;0;83;0
WireConnection;85;1;84;0
WireConnection;121;1;213;0
WireConnection;86;0;74;0
WireConnection;86;1;87;0
WireConnection;48;0;34;0
WireConnection;48;1;38;0
WireConnection;57;0;48;0
WireConnection;81;0;86;0
WireConnection;81;1;83;0
WireConnection;81;2;85;0
WireConnection;179;0;121;4
WireConnection;120;0;81;0
WireConnection;176;0;121;0
WireConnection;176;1;121;4
WireConnection;180;0;226;0
WireConnection;180;1;179;0
WireConnection;89;0;120;0
WireConnection;89;1;92;0
WireConnection;90;0;56;0
WireConnection;90;1;91;0
WireConnection;122;0;180;0
WireConnection;122;1;176;0
WireConnection;54;0;122;0
WireConnection;88;0;89;0
WireConnection;88;1;90;0
WireConnection;105;0;88;0
WireConnection;105;1;104;0
WireConnection;99;0;105;0
WireConnection;99;1;101;0
WireConnection;99;2;88;0
WireConnection;106;0;55;0
WireConnection;106;1;108;0
WireConnection;111;0;106;0
WireConnection;111;1;99;0
WireConnection;102;0;99;0
WireConnection;102;1;106;0
WireConnection;187;0;185;0
WireConnection;187;1;189;0
WireConnection;145;0;187;0
WireConnection;145;1;190;0
WireConnection;46;0;33;0
WireConnection;46;1;34;0
WireConnection;118;0;111;0
WireConnection;118;1;117;0
WireConnection;113;0;108;0
WireConnection;115;0;114;0
WireConnection;190;0;186;0
WireConnection;190;1;189;0
WireConnection;116;0;227;0
WireConnection;116;1;115;0
WireConnection;33;0;34;0
WireConnection;33;1;116;0
WireConnection;183;0;145;0
WireConnection;0;0;112;0
WireConnection;0;2;118;0
ASEEND*/
//CHKSM=17004002CF10630F9222FB47FE40D43FEB1AE826