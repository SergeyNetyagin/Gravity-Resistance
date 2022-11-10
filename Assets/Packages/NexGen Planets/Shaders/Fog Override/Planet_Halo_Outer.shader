// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.13 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.13;sub:START;pass:START;ps:flbk:NexGen/Planets/Fog Override/HO,lico:0,lgpr:1,nrmq:1,nrsp:0,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,rprd:False,enco:True,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:False,dith:0,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:34074,y:33234,varname:node_1,prsc:2|emission-1359-OUT;n:type:ShaderForge.SFN_Fresnel,id:10,x:32012,y:32937,varname:node_10,prsc:2|NRM-2482-OUT;n:type:ShaderForge.SFN_Power,id:11,x:32843,y:33164,varname:node_11,prsc:2|VAL-2489-OUT,EXP-1355-OUT;n:type:ShaderForge.SFN_Vector4,id:1355,x:32618,y:33243,varname:node_1355,prsc:2,v1:5,v2:5,v3:5,v4:5;n:type:ShaderForge.SFN_Multiply,id:1356,x:33256,y:33066,varname:node_1356,prsc:2|A-1357-OUT,B-11-OUT;n:type:ShaderForge.SFN_Slider,id:1357,x:32843,y:32942,ptovrint:False,ptlb:Power,ptin:_Power,varname:node_9088,prsc:2,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Multiply,id:1359,x:33728,y:32943,varname:node_1359,prsc:2|A-8983-RGB,B-1356-OUT;n:type:ShaderForge.SFN_LightVector,id:2473,x:31617,y:32845,varname:node_2473,prsc:2;n:type:ShaderForge.SFN_Fresnel,id:2475,x:32012,y:33095,varname:node_2475,prsc:2|NRM-2477-OUT;n:type:ShaderForge.SFN_NormalVector,id:2477,x:31812,y:33095,prsc:2,pt:True;n:type:ShaderForge.SFN_Dot,id:2481,x:32191,y:33000,varname:node_2481,prsc:2,dt:0|A-10-OUT,B-2475-OUT;n:type:ShaderForge.SFN_OneMinus,id:2482,x:31812,y:32886,varname:node_2482,prsc:2|IN-2473-OUT;n:type:ShaderForge.SFN_OneMinus,id:2486,x:32396,y:33030,varname:node_2486,prsc:2|IN-2481-OUT;n:type:ShaderForge.SFN_Blend,id:2489,x:32618,y:33030,varname:node_2489,prsc:2,blmd:18,clmp:True|SRC-2486-OUT,DST-2486-OUT;n:type:ShaderForge.SFN_Color,id:8983,x:33415,y:32858,ptovrint:False,ptlb:Color Tint,ptin:_ColorTint,varname:node_8983,prsc:2,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;proporder:1357-8983;pass:END;sub:END;*/

Shader "NexGen/Planets/Fog Override/Halo Outer" {
    Properties {
        _Power ("Power", Range(0, 15)) = 1
        _ColorTint ("Color Tint", Color) = (0.5,0.5,0.5,1)
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
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
                float node_2486 = (1.0 - dot((1.0-max(0,dot((1.0 - lightDirection), viewDirection))),(1.0-max(0,dot(normalDirection, viewDirection)))));
                float3 emissive = (_ColorTint.rgb*(_Power*pow(saturate((0.5 - 2.0*(node_2486-0.5)*(node_2486-0.5))),float4(5,5,5,5)))).rgb;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "NexGen/Planets/Fog Override/HO"
    CustomEditor "ShaderForgeMaterialInspector"
}
