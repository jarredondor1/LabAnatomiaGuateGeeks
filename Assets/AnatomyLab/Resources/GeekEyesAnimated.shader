Shader "GuateGeeks/AnimatedEyesUI" {
 Properties {
  _MainTex ("Ojos GuateGeeks", 2D) = "white" {}
  _Color ("Tinte", Color) = (1,1,1,1)
  _Gaze ("Mirada", Vector) = (0,0,0,0)
 }
 SubShader {
  Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
  Pass {
   Blend SrcAlpha OneMinusSrcAlpha
   ZWrite Off
   Cull Off
   HLSLPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #pragma multi_compile_instancing
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
   TEXTURE2D(_MainTex);SAMPLER(sampler_MainTex);
   CBUFFER_START(UnityPerMaterial)
   half4 _Color;float4 _Gaze;
   CBUFFER_END
   struct A { float4 positionOS:POSITION; float2 uv:TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID };
   struct V { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; UNITY_VERTEX_OUTPUT_STEREO };
   V vert(A a){V o;UNITY_SETUP_INSTANCE_ID(a);UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);o.positionCS=TransformObjectToHClip(a.positionOS.xyz);o.uv=a.uv;return o;}
   float Disc(float2 p,float2 centre,float radius){return 1-smoothstep(radius-1.5,radius+1.5,length(p-centre));}
   half4 frag(V i):SV_Target {
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
    float2 p=float2(i.uv.x*512,(1-i.uv.y)*282);
    half4 result=SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv)*_Color;
    float whites=max(Disc(p,float2(118,172),97),Disc(p,float2(396,115),97));
    result.rgb=lerp(result.rgb,half3(1,1,1),whites);result.a=max(result.a,whites);
    float2 movement=_Gaze.xy*34;
    float2 left=float2(140,162)+movement,right=float2(371,115)+movement;
    float pupils=max(Disc(p,left,36),Disc(p,right,36));
    result.rgb=lerp(result.rgb,half3(.015,.02,.025),pupils);result.a=max(result.a,pupils);
    float highlight=max(Disc(p,left+float2(11,-14),9),Disc(p,right+float2(11,-14),9));
    result.rgb=lerp(result.rgb,half3(1,1,1),highlight);result.a=max(result.a,highlight);
    return result;
   }
   ENDHLSL
  }
 }
}
