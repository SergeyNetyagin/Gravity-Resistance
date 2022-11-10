// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.13 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.13;sub:START;pass:START;ps:flbk:NexGen/Planets/St,lico:0,lgpr:1,nrmq:0,nrsp:0,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,rprd:False,enco:True,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:True,dith:0,ufog:False,aust:False,igpj:True,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:1,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:34160,y:33669,varname:node_1,prsc:2|diff-1616-OUT,diffpow-1616-OUT,spec-2531-OUT,gloss-1488-OUT,normal-1383-RGB,emission-1648-OUT;n:type:ShaderForge.SFN_Fresnel,id:10,x:32748,y:33257,varname:node_10,prsc:2|NRM-1622-OUT;n:type:ShaderForge.SFN_Tex2d,id:1371,x:32748,y:33432,ptovrint:False,ptlb:Diffuse (RGB) Specular (A),ptin:_DiffuseRGBSpecularA,varname:node_9487,prsc:2,tex:926ab811b2a96c443846db02bff4d8e7,ntxv:0,isnm:False|UVIN-1739-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:1383,x:32748,y:33614,ptovrint:False,ptlb:Normal Map,ptin:_NormalMap,varname:node_4510,prsc:2,tex:d985cb5859ea9924ca2a6d60fe0199a5,ntxv:3,isnm:True|UVIN-1739-UVOUT;n:type:ShaderForge.SFN_Lerp,id:1394,x:32941,y:33316,varname:node_1394,prsc:2|A-1371-A,B-10-OUT,T-1395-OUT;n:type:ShaderForge.SFN_Vector1,id:1395,x:32547,y:33390,varname:node_1395,prsc:2,v1:0.7;n:type:ShaderForge.SFN_Multiply,id:1420,x:33127,y:33316,varname:node_1420,prsc:2|A-1371-A,B-1394-OUT;n:type:ShaderForge.SFN_Multiply,id:1426,x:33326,y:33267,varname:node_1426,prsc:2|A-1427-OUT,B-1420-OUT;n:type:ShaderForge.SFN_Slider,id:1427,x:32970,y:33159,ptovrint:False,ptlb:Specular Level,ptin:_SpecularLevel,varname:node_1540,prsc:2,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:1488,x:33169,y:33469,ptovrint:False,ptlb:Glossiness,ptin:_Glossiness,varname:node_6515,prsc:2,min:0.2,cur:0.2,max:0.4;n:type:ShaderForge.SFN_Tex2d,id:1588,x:32748,y:33816,ptovrint:False,ptlb:Clouds,ptin:_Clouds,varname:node_9436,prsc:2,tex:23075e732097310458d9b8b09445673e,ntxv:0,isnm:False|UVIN-2181-UVOUT;n:type:ShaderForge.SFN_Add,id:1616,x:33585,y:33671,varname:node_1616,prsc:2|A-1371-RGB,B-2231-OUT;n:type:ShaderForge.SFN_LightVector,id:1622,x:32410,y:33250,varname:node_1622,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:1638,x:32767,y:34160,ptovrint:False,ptlb:Nightlights,ptin:_Nightlights,varname:node_3689,prsc:2,tex:f9c06cf8bea79fe4c957866cb429e7b0,ntxv:0,isnm:False|UVIN-1739-UVOUT;n:type:ShaderForge.SFN_LightVector,id:1645,x:32950,y:33864,varname:node_1645,prsc:2;n:type:ShaderForge.SFN_Dot,id:1647,x:33530,y:33838,varname:node_1647,prsc:2,dt:0|A-7307-OUT,B-2400-OUT;n:type:ShaderForge.SFN_Multiply,id:1648,x:33589,y:34026,varname:node_1648,prsc:2|A-1647-OUT,B-1638-RGB,C-5183-OUT;n:type:ShaderForge.SFN_Panner,id:1739,x:32279,y:33585,varname:node_1739,prsc:2,spu:1,spv:0|DIST-2101-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2084,x:31640,y:33481,ptovrint:False,ptlb:Rotation,ptin:_Rotation,varname:node_6310,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Time,id:2100,x:31831,y:33701,varname:node_2100,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2101,x:32106,y:33585,varname:node_2101,prsc:2|A-2114-OUT,B-2100-T;n:type:ShaderForge.SFN_Vector1,id:2113,x:31640,y:33587,varname:node_2113,prsc:2,v1:0.025;n:type:ShaderForge.SFN_Multiply,id:2114,x:31831,y:33508,varname:node_2114,prsc:2|A-2084-OUT,B-2113-OUT;n:type:ShaderForge.SFN_Panner,id:2181,x:32279,y:33793,varname:node_2181,prsc:2,spu:0.8,spv:0|DIST-2184-OUT;n:type:ShaderForge.SFN_Multiply,id:2184,x:32106,y:33792,varname:node_2184,prsc:2|A-2114-OUT,B-2100-T;n:type:ShaderForge.SFN_Multiply,id:2231,x:33260,y:33761,varname:node_2231,prsc:2|A-2232-OUT,B-1588-RGB;n:type:ShaderForge.SFN_Slider,id:2232,x:32905,y:33720,ptovrint:False,ptlb:Clouds Opacity,ptin:_CloudsOpacity,varname:node_1879,prsc:2,min:0,cur:1.863597,max:3;n:type:ShaderForge.SFN_Vector3,id:2400,x:32950,y:34007,varname:node_2400,prsc:2,v1:0.5,v2:0.5,v3:0;n:type:ShaderForge.SFN_Slider,id:2436,x:29865,y:32885,ptovrint:False,ptlb:Rotation Speed_copy_copy,ptin:_RotationSpeed_copy_copy,varname:node_3572,prsc:2,min:0,cur:0.6466165,max:1;n:type:ShaderForge.SFN_Time,id:2438,x:30069,y:33078,varname:node_2438,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2440,x:30453,y:32887,varname:node_2440,prsc:2|A-2442-OUT,B-2444-OUT;n:type:ShaderForge.SFN_Vector1,id:2442,x:30211,y:32811,varname:node_2442,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:2444,x:30230,y:32947,varname:node_2444,prsc:2|A-2436-OUT,B-2438-TSL;n:type:ShaderForge.SFN_Multiply,id:2531,x:33857,y:33464,varname:node_2531,prsc:2|A-1426-OUT,B-2537-OUT;n:type:ShaderForge.SFN_OneMinus,id:2537,x:33585,y:33531,varname:node_2537,prsc:2|IN-2231-OUT;n:type:ShaderForge.SFN_Add,id:5183,x:33381,y:34167,varname:node_5183,prsc:2|A-1647-OUT,B-1638-RGB;n:type:ShaderForge.SFN_HalfVector,id:5679,x:33008,y:34223,varname:node_5679,prsc:2;n:type:ShaderForge.SFN_Add,id:7307,x:33300,y:33883,varname:node_7307,prsc:2|A-1645-OUT,B-5679-OUT;proporder:1371-1383-1427-1488-1588-1638-2084-2232;pass:END;sub:END;*/

Shader "NexGen/Planets/Standard" {
    Properties {
        _DiffuseRGBSpecularA ("Diffuse (RGB) Specular (A)", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _SpecularLevel ("Specular Level", Range(0, 1)) = 1
        _Glossiness ("Glossiness", Range(0.2, 0.4)) = 0.2
        _Clouds ("Clouds", 2D) = "white" {}
        _Nightlights ("Nightlights", 2D) = "white" {}
        _Rotation ("Rotation", Float ) = 1
        _CloudsOpacity ("Clouds Opacity", Range(0, 3)) = 1.863597
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _DiffuseRGBSpecularA; uniform float4 _DiffuseRGBSpecularA_ST;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform float _SpecularLevel;
            uniform float _Glossiness;
            uniform sampler2D _Clouds; uniform float4 _Clouds_ST;
            uniform sampler2D _Nightlights; uniform float4 _Nightlights_ST;
            uniform float _Rotation;
            uniform float _CloudsOpacity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float node_2114 = (_Rotation*0.025);
                float4 node_2100 = _Time + _TimeEditor;
                float2 node_1739 = (i.uv0+(node_2114*node_2100.g)*float2(1,0));
                float3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(node_1739, _NormalMap)));
                float3 normalLocal = _NormalMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Glossiness;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float4 _DiffuseRGBSpecularA_var = tex2D(_DiffuseRGBSpecularA,TRANSFORM_TEX(node_1739, _DiffuseRGBSpecularA));
                float2 node_2181 = (i.uv0+(node_2114*node_2100.g)*float2(0.8,0));
                float4 _Clouds_var = tex2D(_Clouds,TRANSFORM_TEX(node_2181, _Clouds));
                float3 node_2231 = (_CloudsOpacity*_Clouds_var.rgb);
                float3 specularColor = ((_SpecularLevel*(_DiffuseRGBSpecularA_var.a*lerp(_DiffuseRGBSpecularA_var.a,(1.0-max(0,dot(lightDirection, viewDirection))),0.7)))*(1.0 - node_2231));
                float specularMonochrome = max( max(specularColor.r, specularColor.g), specularColor.b);
                float normTerm = (specPow + 8.0 ) / (8.0 * Pi);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow)*normTerm*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 node_1616 = (_DiffuseRGBSpecularA_var.rgb+node_2231);
                float3 directDiffuse = pow(max( 0.0, NdotL), node_1616) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuseColor = node_1616;
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float node_1647 = dot((lightDirection+halfDirection),float3(0.5,0.5,0));
                float4 _Nightlights_var = tex2D(_Nightlights,TRANSFORM_TEX(node_1739, _Nightlights));
                float3 emissive = (node_1647*_Nightlights_var.rgb*(node_1647+_Nightlights_var.rgb));
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "NexGen/Planets/St"
    CustomEditor "ShaderForgeMaterialInspector"
}
