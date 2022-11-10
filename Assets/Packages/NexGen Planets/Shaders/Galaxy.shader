// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.13 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.13;sub:START;pass:START;ps:flbk:NexGen/Planets/Gx,lico:0,lgpr:1,nrmq:1,nrsp:0,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,bsrc:0,bdst:0,culm:2,dpts:2,wrdp:False,dith:0,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:35089,y:32753,varname:node_1,prsc:2|emission-810-OUT,alpha-160-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:33339,y:32611,ptovrint:False,ptlb:Galaxy (RGBA),ptin:_GalaxyRGBA,varname:node_1414,prsc:2,tex:34ad08bd6e1513247861c1da458a6d3f,ntxv:0,isnm:False|UVIN-46-UVOUT;n:type:ShaderForge.SFN_Rotator,id:46,x:33073,y:32529,varname:node_46,prsc:2|UVIN-58-UVOUT,SPD-65-OUT;n:type:ShaderForge.SFN_Slider,id:57,x:31950,y:32629,ptovrint:False,ptlb:Galaxy Rotation Speed,ptin:_GalaxyRotationSpeed,varname:node_1719,prsc:2,min:0,cur:0.2483896,max:1;n:type:ShaderForge.SFN_TexCoord,id:58,x:32863,y:32426,varname:node_58,prsc:2,uv:0;n:type:ShaderForge.SFN_Time,id:64,x:32107,y:32726,varname:node_64,prsc:2;n:type:ShaderForge.SFN_Multiply,id:65,x:32538,y:32631,varname:node_65,prsc:2|A-906-OUT,B-936-OUT;n:type:ShaderForge.SFN_Fresnel,id:159,x:33339,y:33003,varname:node_159,prsc:2;n:type:ShaderForge.SFN_Multiply,id:160,x:33755,y:32831,varname:node_160,prsc:2|A-2-A,B-185-OUT;n:type:ShaderForge.SFN_OneMinus,id:185,x:33502,y:33002,varname:node_185,prsc:2|IN-159-OUT;n:type:ShaderForge.SFN_Multiply,id:210,x:33810,y:32645,varname:node_210,prsc:2|A-2-RGB,B-2-A;n:type:ShaderForge.SFN_Clamp,id:215,x:33906,y:32361,varname:node_215,prsc:2|IN-222-OUT,MIN-218-OUT,MAX-216-OUT;n:type:ShaderForge.SFN_Vector1,id:216,x:33574,y:32557,varname:node_216,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:218,x:33563,y:32474,varname:node_218,prsc:2,v1:0;n:type:ShaderForge.SFN_Time,id:220,x:33245,y:32218,varname:node_220,prsc:2;n:type:ShaderForge.SFN_Sin,id:222,x:33574,y:32316,varname:node_222,prsc:2|IN-226-OUT;n:type:ShaderForge.SFN_Multiply,id:226,x:33574,y:32143,varname:node_226,prsc:2|A-294-OUT,B-220-TSL;n:type:ShaderForge.SFN_Multiply,id:234,x:34406,y:32622,varname:node_234,prsc:2|A-264-OUT,B-210-OUT;n:type:ShaderForge.SFN_Add,id:264,x:34136,y:32498,varname:node_264,prsc:2|A-344-OUT,B-210-OUT;n:type:ShaderForge.SFN_Slider,id:294,x:33187,y:32091,ptovrint:False,ptlb:Flickering Speed,ptin:_FlickeringSpeed,varname:node_3729,prsc:2,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Slider,id:342,x:33754,y:32069,ptovrint:False,ptlb:Flickering Power,ptin:_FlickeringPower,varname:node_1977,prsc:2,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:344,x:34128,y:32183,varname:node_344,prsc:2|A-342-OUT,B-215-OUT;n:type:ShaderForge.SFN_Add,id:362,x:34641,y:32812,varname:node_362,prsc:2|A-234-OUT,B-366-OUT;n:type:ShaderForge.SFN_Tex2d,id:364,x:33339,y:32805,ptovrint:False,ptlb:Dust (RGBA),ptin:_DustRGBA,varname:node_4551,prsc:2,tex:5c9cc8bc66bbf5f49bce5ad19d8787b8,ntxv:0,isnm:False|UVIN-410-UVOUT;n:type:ShaderForge.SFN_Multiply,id:366,x:33744,y:33002,varname:node_366,prsc:2|A-364-RGB,B-443-RGB;n:type:ShaderForge.SFN_Rotator,id:410,x:33091,y:32836,varname:node_410,prsc:2|UVIN-411-UVOUT,SPD-1142-OUT;n:type:ShaderForge.SFN_TexCoord,id:411,x:32861,y:32722,varname:node_411,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:443,x:33398,y:33177,ptovrint:False,ptlb:Dust Mask (RGBA),ptin:_DustMaskRGBA,varname:node_7897,prsc:2,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:808,x:34646,y:32642,ptovrint:False,ptlb:Color Tint,ptin:_ColorTint,varname:node_6260,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:810,x:34896,y:32768,varname:node_810,prsc:2|A-808-RGB,B-362-OUT;n:type:ShaderForge.SFN_Vector1,id:906,x:32296,y:32555,varname:node_906,prsc:2,v1:0.01;n:type:ShaderForge.SFN_Multiply,id:936,x:32315,y:32691,varname:node_936,prsc:2|A-57-OUT,B-64-TSL;n:type:ShaderForge.SFN_Slider,id:1138,x:32050,y:33088,ptovrint:False,ptlb:Dust Rotation Speed,ptin:_DustRotationSpeed,varname:node_4306,prsc:2,min:0,cur:0.6466165,max:1;n:type:ShaderForge.SFN_Time,id:1140,x:32254,y:33281,varname:node_1140,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1142,x:32638,y:33090,varname:node_1142,prsc:2|A-1144-OUT,B-1146-OUT;n:type:ShaderForge.SFN_Vector1,id:1144,x:32396,y:33014,varname:node_1144,prsc:2,v1:0.01;n:type:ShaderForge.SFN_Multiply,id:1146,x:32415,y:33150,varname:node_1146,prsc:2|A-1138-OUT,B-1140-TSL;proporder:808-2-364-443-57-1138-342-294;pass:END;sub:END;*/

Shader "NexGen/Planets/Galaxy" {
    Properties {
        _ColorTint ("Color Tint", Color) = (1,1,1,1)
        _GalaxyRGBA ("Galaxy (RGBA)", 2D) = "white" {}
        _DustRGBA ("Dust (RGBA)", 2D) = "white" {}
        _DustMaskRGBA ("Dust Mask (RGBA)", 2D) = "white" {}
        _GalaxyRotationSpeed ("Galaxy Rotation Speed", Range(0, 1)) = 0.2483896
        _DustRotationSpeed ("Dust Rotation Speed", Range(0, 1)) = 0.6466165
        _FlickeringPower ("Flickering Power", Range(0, 1)) = 0
        _FlickeringSpeed ("Flickering Speed", Range(0, 10)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
            Cull Off
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
            uniform sampler2D _GalaxyRGBA; uniform float4 _GalaxyRGBA_ST;
            uniform float _GalaxyRotationSpeed;
            uniform float _FlickeringSpeed;
            uniform float _FlickeringPower;
            uniform sampler2D _DustRGBA; uniform float4 _DustRGBA_ST;
            uniform sampler2D _DustMaskRGBA; uniform float4 _DustMaskRGBA_ST;
            uniform float4 _ColorTint;
            uniform float _DustRotationSpeed;
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
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
////// Lighting:
////// Emissive:
                float4 node_220 = _Time + _TimeEditor;
                float4 node_3313 = _Time + _TimeEditor;
                float4 node_64 = _Time + _TimeEditor;
                float node_46_ang = node_3313.g;
                float node_46_spd = (0.01*(_GalaxyRotationSpeed*node_64.r));
                float node_46_cos = cos(node_46_spd*node_46_ang);
                float node_46_sin = sin(node_46_spd*node_46_ang);
                float2 node_46_piv = float2(0.5,0.5);
                float2 node_46 = (mul(i.uv0-node_46_piv,float2x2( node_46_cos, -node_46_sin, node_46_sin, node_46_cos))+node_46_piv);
                float4 _GalaxyRGBA_var = tex2D(_GalaxyRGBA,TRANSFORM_TEX(node_46, _GalaxyRGBA));
                float3 node_210 = (_GalaxyRGBA_var.rgb*_GalaxyRGBA_var.a);
                float4 node_1140 = _Time + _TimeEditor;
                float node_410_ang = node_3313.g;
                float node_410_spd = (0.01*(_DustRotationSpeed*node_1140.r));
                float node_410_cos = cos(node_410_spd*node_410_ang);
                float node_410_sin = sin(node_410_spd*node_410_ang);
                float2 node_410_piv = float2(0.5,0.5);
                float2 node_410 = (mul(i.uv0-node_410_piv,float2x2( node_410_cos, -node_410_sin, node_410_sin, node_410_cos))+node_410_piv);
                float4 _DustRGBA_var = tex2D(_DustRGBA,TRANSFORM_TEX(node_410, _DustRGBA));
                float4 _DustMaskRGBA_var = tex2D(_DustMaskRGBA,TRANSFORM_TEX(i.uv0, _DustMaskRGBA));
                float3 emissive = (_ColorTint.rgb*((((_FlickeringPower*clamp(sin((_FlickeringSpeed*node_220.r)),0.0,1.0))+node_210)*node_210)+(_DustRGBA_var.rgb*_DustMaskRGBA_var.rgb)));
                float3 finalColor = emissive;
                return fixed4(finalColor,(_GalaxyRGBA_var.a*(1.0 - (1.0-max(0,dot(normalDirection, viewDirection))))));
            }
            ENDCG
        }
    }
    FallBack "NexGen/Planets/Gx"
    CustomEditor "ShaderForgeMaterialInspector"
}
