// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Shader created with Shader Forge Beta 0.30 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.30;sub:START;pass:START;ps:flbk:NexGen/Planets/Fog Override/G,lico:0,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:2,bsrc:0,bdst:0,culm:2,dpts:2,wrdp:False,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:31534,y:32753|emission-810-OUT,alpha-160-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:33353,y:32611,ptlb:Galaxy (RGBA),ptin:_GalaxyRGBA,tex:34ad08bd6e1513247861c1da458a6d3f,ntxv:0,isnm:False|UVIN-46-UVOUT;n:type:ShaderForge.SFN_Rotator,id:46,x:33619,y:32529|UVIN-58-UVOUT,SPD-65-OUT;n:type:ShaderForge.SFN_Slider,id:57,x:34585,y:32629,ptlb:Galaxy Rotation Speed,ptin:_GalaxyRotationSpeed,min:0,cur:0.6466165,max:1;n:type:ShaderForge.SFN_TexCoord,id:58,x:33829,y:32426,uv:0;n:type:ShaderForge.SFN_Time,id:64,x:34538,y:32822;n:type:ShaderForge.SFN_Multiply,id:65,x:34154,y:32631|A-906-OUT,B-936-OUT;n:type:ShaderForge.SFN_Fresnel,id:159,x:33353,y:33003;n:type:ShaderForge.SFN_Multiply,id:160,x:32937,y:32831|A-2-A,B-185-OUT;n:type:ShaderForge.SFN_OneMinus,id:185,x:33190,y:33002|IN-159-OUT;n:type:ShaderForge.SFN_Multiply,id:210,x:32882,y:32645|A-2-RGB,B-2-A;n:type:ShaderForge.SFN_Clamp,id:215,x:32786,y:32361|IN-222-OUT,MIN-218-OUT,MAX-216-OUT;n:type:ShaderForge.SFN_Vector1,id:216,x:33118,y:32557,v1:1;n:type:ShaderForge.SFN_Vector1,id:218,x:33129,y:32474,v1:0;n:type:ShaderForge.SFN_Time,id:220,x:33447,y:32218;n:type:ShaderForge.SFN_Sin,id:222,x:33118,y:32316|IN-226-OUT;n:type:ShaderForge.SFN_Multiply,id:226,x:33118,y:32143|A-294-OUT,B-220-TSL;n:type:ShaderForge.SFN_Multiply,id:234,x:32286,y:32622|A-264-OUT,B-210-OUT;n:type:ShaderForge.SFN_Add,id:264,x:32556,y:32498|A-344-OUT,B-210-OUT;n:type:ShaderForge.SFN_Slider,id:294,x:33348,y:32091,ptlb:Flickering Speed,ptin:_FlickeringSpeed,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Slider,id:342,x:32781,y:32069,ptlb:Flickering Power,ptin:_FlickeringPower,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:344,x:32564,y:32183|A-342-OUT,B-215-OUT;n:type:ShaderForge.SFN_Add,id:362,x:32051,y:32812|A-234-OUT,B-366-OUT;n:type:ShaderForge.SFN_Tex2d,id:364,x:33353,y:32805,ptlb:Dust (RGBA),ptin:_DustRGBA,tex:5c9cc8bc66bbf5f49bce5ad19d8787b8,ntxv:0,isnm:False|UVIN-410-UVOUT;n:type:ShaderForge.SFN_Multiply,id:366,x:32948,y:33002|A-364-RGB,B-443-RGB;n:type:ShaderForge.SFN_Rotator,id:410,x:33601,y:32836|UVIN-411-UVOUT,SPD-1142-OUT;n:type:ShaderForge.SFN_TexCoord,id:411,x:33831,y:32722,uv:0;n:type:ShaderForge.SFN_Tex2d,id:443,x:33294,y:33177,ptlb:Dust Mask (RGBA),ptin:_DustMaskRGBA,tex:95ef4804fe0be4c999ddaa383536cde8,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:808,x:32046,y:32642,ptlb:Color Tint,ptin:_ColorTint,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:810,x:31796,y:32768|A-808-RGB,B-362-OUT;n:type:ShaderForge.SFN_Vector1,id:906,x:34396,y:32555,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:936,x:34377,y:32691|A-57-OUT,B-64-TSL;n:type:ShaderForge.SFN_Slider,id:1138,x:34485,y:33088,ptlb:Dust Rotation Speed,ptin:_DustRotationSpeed,min:0,cur:0.6466165,max:1;n:type:ShaderForge.SFN_Time,id:1140,x:34438,y:33281;n:type:ShaderForge.SFN_Multiply,id:1142,x:34054,y:33090|A-1144-OUT,B-1146-OUT;n:type:ShaderForge.SFN_Vector1,id:1144,x:34296,y:33014,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:1146,x:34277,y:33150|A-1138-OUT,B-1140-TSL;proporder:808-2-364-443-57-1138-342-294;pass:END;sub:END;*/

Shader "NexGen/Planets/Fog Override/Galaxy" {
    Properties {
        _ColorTint ("Color Tint", Color) = (1,1,1,1)
        _GalaxyRGBA ("Galaxy (RGBA)", 2D) = "white" {}
        _DustRGBA ("Dust (RGBA)", 2D) = "white" {}
        _DustMaskRGBA ("Dust Mask (RGBA)", 2D) = "white" {}
        _GalaxyRotationSpeed ("Galaxy Rotation Speed", Range(0, 1)) = 0.6466165
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
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
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
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
////// Lighting:
////// Emissive:
                float4 node_220 = _Time + _TimeEditor;
                float4 node_3136 = _Time + _TimeEditor;
                float4 node_64 = _Time + _TimeEditor;
                float node_46_ang = node_3136.g;
                float node_46_spd = (0.1*(_GalaxyRotationSpeed*node_64.r));
                float node_46_cos = cos(node_46_spd*node_46_ang);
                float node_46_sin = sin(node_46_spd*node_46_ang);
                float2 node_46_piv = float2(0.5,0.5);
                float2 node_46 = (mul(i.uv0.rg-node_46_piv,float2x2( node_46_cos, -node_46_sin, node_46_sin, node_46_cos))+node_46_piv);
                float4 node_2 = tex2D(_GalaxyRGBA,TRANSFORM_TEX(node_46, _GalaxyRGBA));
                float3 node_210 = (node_2.rgb*node_2.a);
                float4 node_1140 = _Time + _TimeEditor;
                float node_410_ang = node_3136.g;
                float node_410_spd = (0.1*(_DustRotationSpeed*node_1140.r));
                float node_410_cos = cos(node_410_spd*node_410_ang);
                float node_410_sin = sin(node_410_spd*node_410_ang);
                float2 node_410_piv = float2(0.5,0.5);
                float2 node_410 = (mul(i.uv0.rg-node_410_piv,float2x2( node_410_cos, -node_410_sin, node_410_sin, node_410_cos))+node_410_piv);
                float2 node_3137 = i.uv0;
                float3 emissive = (_ColorTint.rgb*((((_FlickeringPower*clamp(sin((_FlickeringSpeed*node_220.r)),0.0,1.0))+node_210)*node_210)+(tex2D(_DustRGBA,TRANSFORM_TEX(node_410, _DustRGBA)).rgb*tex2D(_DustMaskRGBA,TRANSFORM_TEX(node_3137.rg, _DustMaskRGBA)).rgb)));
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,(node_2.a*(1.0 - (1.0-max(0,dot(normalDirection, viewDirection))))));
            }
            ENDCG
        }
    }
    FallBack "NexGen/Planets/Fog Override/G"
    CustomEditor "ShaderForgeMaterialInspector"
}
