// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.13 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.13;sub:START;pass:START;ps:flbk:NexGen/Planets/Fog Override/G,lico:1,lgpr:1,nrmq:0,nrsp:0,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,rprd:False,enco:True,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:False,dith:0,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:35433,y:33089,varname:node_1,prsc:2|emission-1564-OUT,custl-588-OUT;n:type:ShaderForge.SFN_Fresnel,id:10,x:33098,y:32804,varname:node_10,prsc:2|NRM-1165-OUT;n:type:ShaderForge.SFN_Power,id:11,x:33395,y:32851,varname:node_11,prsc:2|VAL-10-OUT,EXP-1267-OUT;n:type:ShaderForge.SFN_Time,id:128,x:32685,y:33610,varname:node_128,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:507,x:33575,y:33500,ptovrint:False,ptlb:Map 01,ptin:_Map01,varname:node_142,prsc:2,tex:a8cee7b98fc8e0f499fbf9791d29fe9b,ntxv:0,isnm:False|UVIN-508-UVOUT;n:type:ShaderForge.SFN_Panner,id:508,x:33390,y:33500,varname:node_508,prsc:2,spu:1,spv:1|DIST-509-OUT;n:type:ShaderForge.SFN_Multiply,id:509,x:33110,y:33459,varname:node_509,prsc:2|A-795-OUT,B-552-OUT;n:type:ShaderForge.SFN_Multiply,id:552,x:32901,y:33520,varname:node_552,prsc:2|A-128-T,B-555-OUT;n:type:ShaderForge.SFN_Vector1,id:555,x:32685,y:33485,varname:node_555,prsc:2,v1:0.9;n:type:ShaderForge.SFN_Multiply,id:588,x:34423,y:33593,varname:node_588,prsc:2|A-1446-OUT,B-710-OUT;n:type:ShaderForge.SFN_Slider,id:594,x:33528,y:33690,ptovrint:False,ptlb:Flickering,ptin:_Flickering,varname:node_3347,prsc:2,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Clamp,id:600,x:33606,y:33832,varname:node_600,prsc:2|IN-619-OUT,MIN-601-OUT,MAX-702-OUT;n:type:ShaderForge.SFN_Vector1,id:601,x:33390,y:33863,varname:node_601,prsc:2,v1:0.75;n:type:ShaderForge.SFN_Sin,id:619,x:33390,y:33690,varname:node_619,prsc:2|IN-620-OUT;n:type:ShaderForge.SFN_Multiply,id:620,x:33110,y:33691,varname:node_620,prsc:2|A-621-OUT,B-128-T;n:type:ShaderForge.SFN_Vector1,id:621,x:32901,y:33761,varname:node_621,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Vector1,id:702,x:33390,y:33946,varname:node_702,prsc:2,v1:0.9;n:type:ShaderForge.SFN_Multiply,id:710,x:33873,y:33734,varname:node_710,prsc:2|A-600-OUT,B-594-OUT;n:type:ShaderForge.SFN_ValueProperty,id:786,x:32685,y:33274,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_9404,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:795,x:32901,y:33316,varname:node_795,prsc:2|A-786-OUT,B-800-OUT;n:type:ShaderForge.SFN_Vector1,id:800,x:32685,y:33396,varname:node_800,prsc:2,v1:0.025;n:type:ShaderForge.SFN_NormalVector,id:1165,x:32886,y:32804,prsc:2,pt:False;n:type:ShaderForge.SFN_Vector4,id:1267,x:33098,y:32993,varname:node_1267,prsc:2,v1:4,v2:4,v3:4,v4:10;n:type:ShaderForge.SFN_Color,id:1290,x:33584,y:33113,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_4777,prsc:2,glob:False,c1:1,c2:0.762069,c3:0.25,c4:1;n:type:ShaderForge.SFN_Multiply,id:1339,x:33875,y:32962,varname:node_1339,prsc:2|A-11-OUT,B-1290-RGB;n:type:ShaderForge.SFN_Multiply,id:1374,x:33946,y:33430,varname:node_1374,prsc:2|A-1290-RGB,B-1409-OUT;n:type:ShaderForge.SFN_Add,id:1379,x:34245,y:33228,varname:node_1379,prsc:2|A-1339-OUT,B-1374-OUT;n:type:ShaderForge.SFN_Panner,id:1395,x:33390,y:33279,varname:node_1395,prsc:2,spu:-1,spv:-0.5|DIST-1406-OUT;n:type:ShaderForge.SFN_Tex2d,id:1405,x:33584,y:33305,ptovrint:False,ptlb:Map 02,ptin:_Map02,varname:node_2468,prsc:2,tex:d5939de5f3bd46c41ac2a1126a436277,ntxv:0,isnm:False|UVIN-1395-UVOUT;n:type:ShaderForge.SFN_Multiply,id:1406,x:33119,y:33211,varname:node_1406,prsc:2|A-1407-OUT,B-552-OUT;n:type:ShaderForge.SFN_Multiply,id:1407,x:32910,y:33107,varname:node_1407,prsc:2|A-786-OUT,B-1408-OUT;n:type:ShaderForge.SFN_Vector1,id:1408,x:32654,y:33104,varname:node_1408,prsc:2,v1:0.05;n:type:ShaderForge.SFN_Subtract,id:1409,x:33870,y:33255,varname:node_1409,prsc:2|A-1405-RGB,B-507-RGB;n:type:ShaderForge.SFN_Power,id:1446,x:34169,y:33521,varname:node_1446,prsc:2|VAL-1374-OUT,EXP-1474-OUT;n:type:ShaderForge.SFN_Vector1,id:1474,x:33976,y:33671,varname:node_1474,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Dot,id:1491,x:34446,y:33324,varname:node_1491,prsc:2,dt:0|A-1379-OUT,B-1625-OUT;n:type:ShaderForge.SFN_Multiply,id:1538,x:34616,y:33162,varname:node_1538,prsc:2|A-1491-OUT,B-1290-RGB;n:type:ShaderForge.SFN_Add,id:1564,x:34868,y:33003,varname:node_1564,prsc:2|A-1580-OUT,B-1538-OUT;n:type:ShaderForge.SFN_Multiply,id:1580,x:34651,y:32917,varname:node_1580,prsc:2|A-1290-RGB,B-1625-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1625,x:34232,y:33420,ptovrint:False,ptlb:Power,ptin:_Power,varname:node_8505,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_Slider,id:3032,x:31246,y:32949,ptovrint:False,ptlb:Rotation Speed_copy_copy,ptin:_RotationSpeed_copy_copy,varname:node_765,prsc:2,min:0,cur:0.6466165,max:1;n:type:ShaderForge.SFN_Time,id:3034,x:31450,y:33142,varname:node_3034,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3036,x:31834,y:32951,varname:node_3036,prsc:2|A-3038-OUT,B-3040-OUT;n:type:ShaderForge.SFN_Vector1,id:3038,x:31592,y:32875,varname:node_3038,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:3040,x:31611,y:33011,varname:node_3040,prsc:2|A-3032-OUT,B-3034-TSL;n:type:ShaderForge.SFN_Slider,id:3042,x:31182,y:33013,ptovrint:False,ptlb:Rotation Speed_copy_copy_copy,ptin:_RotationSpeed_copy_copy_copy,varname:node_6495,prsc:2,min:0,cur:0.6466165,max:1;n:type:ShaderForge.SFN_Time,id:3044,x:31386,y:33206,varname:node_3044,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3046,x:31770,y:33015,varname:node_3046,prsc:2|A-3048-OUT,B-3050-OUT;n:type:ShaderForge.SFN_Vector1,id:3048,x:31528,y:32939,varname:node_3048,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:3050,x:31547,y:33075,varname:node_3050,prsc:2|A-3042-OUT,B-3044-TSL;proporder:507-1405-594-786-1290-1625;pass:END;sub:END;*/

Shader "NexGen/Planets/Fog Override/Gas" {
    Properties {
        _Map01 ("Map 01", 2D) = "white" {}
        _Map02 ("Map 02", 2D) = "white" {}
        _Flickering ("Flickering", Range(0, 1)) = 1
        _Speed ("Speed", Float ) = 1
        _Color ("Color", Color) = (1,0.762069,0.25,1)
        _Power ("Power", Float ) = 0
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
            uniform float4 _TimeEditor;
            uniform sampler2D _Map01; uniform float4 _Map01_ST;
            uniform float _Flickering;
            uniform float _Speed;
            uniform float4 _Color;
            uniform sampler2D _Map02; uniform float4 _Map02_ST;
            uniform float _Power;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 node_128 = _Time + _TimeEditor;
                float node_552 = (node_128.g*0.9);
                float2 node_1395 = (i.uv0+((_Speed*0.05)*node_552)*float2(-1,-0.5));
                float4 _Map02_var = tex2D(_Map02,TRANSFORM_TEX(node_1395, _Map02));
                float2 node_508 = (i.uv0+((_Speed*0.025)*node_552)*float2(1,1));
                float4 _Map01_var = tex2D(_Map01,TRANSFORM_TEX(node_508, _Map01));
                float3 node_1374 = (_Color.rgb*(_Map02_var.rgb-_Map01_var.rgb));
                float3 emissive = ((_Color.rgb*_Power)+(dot(((pow((1.0-max(0,dot(i.normalDir, viewDirection))),float4(4,4,4,10))*_Color.rgb)+node_1374),_Power)*_Color.rgb));
                float3 finalColor = emissive + (pow(node_1374,0.1)*(clamp(sin((0.25*node_128.g)),0.75,0.9)*_Flickering));
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "NexGen/Planets/Fog Override/G"
    CustomEditor "ShaderForgeMaterialInspector"
}
