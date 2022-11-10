// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.13 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.13;sub:START;pass:START;ps:flbk:NexGen/Planets/Hal,lico:0,lgpr:1,nrmq:1,nrsp:0,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,rprd:False,enco:True,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:False,dith:0,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:34038,y:33373,varname:node_1,prsc:2|emission-6726-OUT;n:type:ShaderForge.SFN_Fresnel,id:10,x:32283,y:33115,varname:node_10,prsc:2|NRM-2482-OUT;n:type:ShaderForge.SFN_Power,id:11,x:32825,y:33234,varname:node_11,prsc:2|VAL-2481-OUT,EXP-1355-OUT;n:type:ShaderForge.SFN_Vector4,id:1355,x:32530,y:33326,varname:node_1355,prsc:2,v1:4,v2:4,v3:4,v4:4;n:type:ShaderForge.SFN_Multiply,id:1356,x:33309,y:33149,varname:node_1356,prsc:2|A-1357-OUT,B-11-OUT;n:type:ShaderForge.SFN_Slider,id:1357,x:32891,y:33083,ptovrint:False,ptlb:Power,ptin:_Power,varname:node_2305,prsc:2,min:0,cur:1,max:10;n:type:ShaderForge.SFN_LightVector,id:2473,x:32010,y:32864,varname:node_2473,prsc:2;n:type:ShaderForge.SFN_Fresnel,id:2475,x:32298,y:33372,varname:node_2475,prsc:2|NRM-2477-OUT;n:type:ShaderForge.SFN_NormalVector,id:2477,x:32091,y:33372,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:2481,x:32557,y:33050,varname:node_2481,prsc:2,dt:0|A-10-OUT,B-2475-OUT;n:type:ShaderForge.SFN_OneMinus,id:2482,x:32118,y:33009,varname:node_2482,prsc:2|IN-2473-OUT;n:type:ShaderForge.SFN_Color,id:8321,x:33254,y:33429,ptovrint:False,ptlb:Color Tint,ptin:_ColorTint,varname:node_8321,prsc:2,glob:False,c1:0,c2:0.4206896,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:6726,x:33551,y:33326,varname:node_6726,prsc:2|A-1356-OUT,B-8321-RGB;proporder:1357-8321;pass:END;sub:END;*/

Shader "NexGen/Planets/Halo" {
    Properties {
        _Power ("Power", Range(0, 10)) = 1
        _ColorTint ("Color Tint", Color) = (0,0.4206896,1,1)
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform float _Power;
            uniform float4 _ColorTint;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
////// Emissive:
                float3 emissive = ((_Power*pow(dot((1.0-max(0,dot((1.0 - lightDirection), viewDirection))),(1.0-max(0,dot(i.normalDir, viewDirection)))),float4(4,4,4,4)))*_ColorTint.rgb).rgb;
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "NexGen/Planets/Hal"
    CustomEditor "ShaderForgeMaterialInspector"
}
