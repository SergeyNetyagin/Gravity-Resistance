// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Shader created with Shader Forge Beta 0.30 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.30;sub:START;pass:START;ps:flbk:NexGen/Planets/Gs,lico:1,lgpr:1,nrmq:0,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:True,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:2,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:30806,y:33089|emission-1564-OUT,custl-588-OUT;n:type:ShaderForge.SFN_Fresnel,id:10,x:33210,y:32804|NRM-1165-OUT;n:type:ShaderForge.SFN_Power,id:11,x:32913,y:32851|VAL-10-OUT,EXP-1267-OUT;n:type:ShaderForge.SFN_Time,id:128,x:33623,y:33610;n:type:ShaderForge.SFN_Tex2d,id:507,x:32733,y:33500,ptlb:Map 01,ptin:_Map01,tex:a8cee7b98fc8e0f499fbf9791d29fe9b,ntxv:0,isnm:False|UVIN-508-UVOUT;n:type:ShaderForge.SFN_Panner,id:508,x:32918,y:33500,spu:1,spv:1|DIST-509-OUT;n:type:ShaderForge.SFN_Multiply,id:509,x:33198,y:33459|A-795-OUT,B-552-OUT;n:type:ShaderForge.SFN_Multiply,id:552,x:33407,y:33520|A-128-T,B-555-OUT;n:type:ShaderForge.SFN_Vector1,id:555,x:33623,y:33485,v1:0.9;n:type:ShaderForge.SFN_Multiply,id:588,x:31885,y:33593|A-1446-OUT,B-710-OUT;n:type:ShaderForge.SFN_Slider,id:594,x:32623,y:33690,ptlb:Flickering,ptin:_Flickering,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Clamp,id:600,x:32702,y:33832|IN-619-OUT,MIN-601-OUT,MAX-702-OUT;n:type:ShaderForge.SFN_Vector1,id:601,x:32918,y:33863,v1:0.75;n:type:ShaderForge.SFN_Sin,id:619,x:32918,y:33690|IN-620-OUT;n:type:ShaderForge.SFN_Multiply,id:620,x:33198,y:33691|A-621-OUT,B-128-T;n:type:ShaderForge.SFN_Vector1,id:621,x:33407,y:33761,v1:0.25;n:type:ShaderForge.SFN_Vector1,id:702,x:32918,y:33946,v1:0.9;n:type:ShaderForge.SFN_Multiply,id:710,x:32435,y:33734|A-600-OUT,B-594-OUT;n:type:ShaderForge.SFN_ValueProperty,id:786,x:33623,y:33274,ptlb:Speed,ptin:_Speed,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:795,x:33407,y:33316|A-786-OUT,B-800-OUT;n:type:ShaderForge.SFN_Vector1,id:800,x:33623,y:33396,v1:0.025;n:type:ShaderForge.SFN_NormalVector,id:1165,x:33422,y:32804,pt:False;n:type:ShaderForge.SFN_Vector4,id:1267,x:33210,y:32993,v1:4,v2:4,v3:4,v4:10;n:type:ShaderForge.SFN_Color,id:1290,x:32724,y:33113,ptlb:Color,ptin:_Color,glob:False,c1:1,c2:0.762069,c3:0.25,c4:1;n:type:ShaderForge.SFN_Multiply,id:1339,x:32433,y:32962|A-11-OUT,B-1290-RGB;n:type:ShaderForge.SFN_Multiply,id:1374,x:32362,y:33430|A-1290-RGB,B-1409-OUT;n:type:ShaderForge.SFN_Add,id:1379,x:32063,y:33228|A-1339-OUT,B-1374-OUT;n:type:ShaderForge.SFN_Panner,id:1395,x:32918,y:33279,spu:-1,spv:-0.5|DIST-1406-OUT;n:type:ShaderForge.SFN_Tex2d,id:1405,x:32724,y:33305,ptlb:Map 02,ptin:_Map02,tex:d5939de5f3bd46c41ac2a1126a436277,ntxv:0,isnm:False|UVIN-1395-UVOUT;n:type:ShaderForge.SFN_Multiply,id:1406,x:33189,y:33211|A-1407-OUT,B-552-OUT;n:type:ShaderForge.SFN_Multiply,id:1407,x:33398,y:33107|A-786-OUT,B-1408-OUT;n:type:ShaderForge.SFN_Vector1,id:1408,x:33654,y:33104,v1:0.05;n:type:ShaderForge.SFN_Subtract,id:1409,x:32438,y:33255|A-1405-RGB,B-507-RGB;n:type:ShaderForge.SFN_Power,id:1446,x:32139,y:33521|VAL-1374-OUT,EXP-1474-OUT;n:type:ShaderForge.SFN_Vector1,id:1474,x:32332,y:33671,v1:0.1;n:type:ShaderForge.SFN_Dot,id:1491,x:31862,y:33324,dt:0|A-1379-OUT,B-1625-OUT;n:type:ShaderForge.SFN_Multiply,id:1538,x:31692,y:33162|A-1491-OUT,B-1290-RGB;n:type:ShaderForge.SFN_Add,id:1564,x:31440,y:33003|A-1580-OUT,B-1538-OUT;n:type:ShaderForge.SFN_Multiply,id:1580,x:31657,y:32917|A-1290-RGB,B-1625-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1625,x:32076,y:33420,ptlb:Power,ptin:_Power,glob:False,v1:0;proporder:507-1405-594-786-1290-1625;pass:END;sub:END;*/

Shader "NexGen/Planets/Gas" {
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
            Name "ForwardBase"
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
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
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
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalDirection =  i.normalDir;
////// Lighting:
////// Emissive:
                float4 node_128 = _Time + _TimeEditor;
                float node_552 = (node_128.g*0.9);
                float2 node_3305 = i.uv0;
                float2 node_1395 = (node_3305.rg+((_Speed*0.05)*node_552)*float2(-1,-0.5));
                float2 node_508 = (node_3305.rg+((_Speed*0.025)*node_552)*float2(1,1));
                float3 node_1374 = (_Color.rgb*(tex2D(_Map02,TRANSFORM_TEX(node_1395, _Map02)).rgb-tex2D(_Map01,TRANSFORM_TEX(node_508, _Map01)).rgb));
                float3 emissive = ((_Color.rgb*_Power)+(dot(((pow((1.0-max(0,dot(i.normalDir, viewDirection))),float4(4,4,4,10))*_Color.rgb)+node_1374),_Power)*_Color.rgb));
                float3 finalColor = emissive + (pow(node_1374,0.1)*(clamp(sin((0.25*node_128.g)),0.75,0.9)*_Flickering));
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "NexGen/Planets/Gs"
    CustomEditor "ShaderForgeMaterialInspector"
}
