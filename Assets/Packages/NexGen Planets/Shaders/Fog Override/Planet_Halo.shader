// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Shader created with Shader Forge Beta 0.30 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.30;sub:START;pass:START;ps:flbk:NexGen/Planets/Fog Override/H,lico:0,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:True,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:2,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:False,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:31781,y:33373|emission-1365-OUT,custl-1000-OUT,alpha-4-OUT,olcol-3-OUT;n:type:ShaderForge.SFN_LightVector,id:2,x:33063,y:33528;n:type:ShaderForge.SFN_Vector3,id:3,x:33063,y:33673,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Dot,id:4,x:32847,y:33528,dt:0|A-2-OUT,B-3-OUT;n:type:ShaderForge.SFN_Fresnel,id:10,x:33605,y:33115|NRM-2482-OUT;n:type:ShaderForge.SFN_Power,id:11,x:33063,y:33234|VAL-2481-OUT,EXP-1355-OUT;n:type:ShaderForge.SFN_Multiply,id:13,x:32852,y:33274|A-11-OUT,B-1354-RGB;n:type:ShaderForge.SFN_Multiply,id:1000,x:32520,y:33425|A-13-OUT,B-4-OUT;n:type:ShaderForge.SFN_Color,id:1354,x:33063,y:33373,ptlb:Color Tint,ptin:_ColorTint,glob:False,c1:0,c2:0.006896496,c3:1,c4:1;n:type:ShaderForge.SFN_Vector4,id:1355,x:33358,y:33326,v1:4,v2:4,v3:4,v4:4;n:type:ShaderForge.SFN_Multiply,id:1356,x:32579,y:33149|A-1357-OUT,B-11-OUT;n:type:ShaderForge.SFN_Slider,id:1357,x:32840,y:33083,ptlb:Power,ptin:_Power,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Multiply,id:1359,x:32329,y:33263|A-1356-OUT,B-1354-RGB;n:type:ShaderForge.SFN_Clamp,id:1360,x:32319,y:33705|IN-1363-OUT,MIN-1361-OUT,MAX-1362-OUT;n:type:ShaderForge.SFN_Vector1,id:1361,x:32520,y:33749,v1:0.1;n:type:ShaderForge.SFN_Vector1,id:1362,x:32520,y:33843,v1:1;n:type:ShaderForge.SFN_Sin,id:1363,x:32520,y:33593|IN-1366-OUT;n:type:ShaderForge.SFN_Time,id:1364,x:32939,y:33940;n:type:ShaderForge.SFN_Multiply,id:1365,x:32102,y:33376|A-1359-OUT,B-1360-OUT;n:type:ShaderForge.SFN_Multiply,id:1366,x:32764,y:33799|A-1370-OUT,B-1364-T;n:type:ShaderForge.SFN_ValueProperty,id:1368,x:33188,y:33799,ptlb:Flickering Speed,ptin:_FlickeringSpeed,glob:False,v1:1;n:type:ShaderForge.SFN_Vector1,id:1369,x:33188,y:33895,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:1370,x:32939,y:33777|A-1368-OUT,B-1369-OUT;n:type:ShaderForge.SFN_LightVector,id:2473,x:33878,y:32864;n:type:ShaderForge.SFN_Fresnel,id:2475,x:33590,y:33372|NRM-2477-OUT;n:type:ShaderForge.SFN_NormalVector,id:2477,x:33797,y:33372,pt:False;n:type:ShaderForge.SFN_Dot,id:2481,x:33331,y:33050,dt:0|A-10-OUT,B-2475-OUT;n:type:ShaderForge.SFN_OneMinus,id:2482,x:33770,y:33009|IN-2473-OUT;proporder:1354-1357-1368;pass:END;sub:END;*/

Shader "NexGen/Planets/Fog Override/Halo" {
    Properties {
        _ColorTint ("Color Tint", Color) = (0,0.006896496,1,1)
        _Power ("Power", Range(0, 10)) = 1
        _FlickeringSpeed ("Flickering Speed", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _ColorTint;
            uniform float _Power;
            uniform float _FlickeringSpeed;
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
                VertexOutput o;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
////// Emissive:
                float4 node_11 = pow(dot((1.0-max(0,dot((1.0 - lightDirection), viewDirection))),(1.0-max(0,dot(i.normalDir, viewDirection)))),float4(4,4,4,4));
                float4 node_1364 = _Time + _TimeEditor;
                float3 emissive = (((_Power*node_11)*_ColorTint.rgb)*clamp(sin(((_FlickeringSpeed*0.1)*node_1364.g)),0.1,1.0)).rgb;
                float3 node_3 = float3(0,0,1);
                float node_4 = dot(lightDirection,node_3);
                float3 finalColor = emissive + ((node_11*_ColorTint.rgb)*node_4).rgb;
/// Final Color:
                return fixed4(finalColor,node_4);
            }
            ENDCG
        }
    }
    FallBack "NexGen/Planets/Fog Override/H"
    CustomEditor "ShaderForgeMaterialInspector"
}
