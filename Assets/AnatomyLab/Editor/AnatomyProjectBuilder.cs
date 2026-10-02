using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Unity.XR.CoreUtils;
using GuateGeeks.AnatomyLab.Core;
using GuateGeeks.AnatomyLab.UI;
using GuateGeeks.AnatomyLab.XR;
using GuateGeeks.AnatomyLab.Performance;
public static class AnatomyProjectBuilder {
 const string Root="Assets/AnatomyLab/";
 [MenuItem("GuateGeeks/Anatomy Lab/Configure Project and Scenes")]
 public static void Configure(){
 PlayerSettings.companyName="GuateGeeks";PlayerSettings.productName="GuateGeeks Anatomy Lab VR";PlayerSettings.bundleVersion="0.1.0";
 PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,"com.guategeeks.anatomylabvr");
 PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel32;PlayerSettings.Android.targetSdkVersion=AndroidSdkVersions.AndroidApiLevel34;
 PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android,false);PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,new[]{GraphicsDeviceType.Vulkan,GraphicsDeviceType.OpenGLES3});
 PlayerSettings.colorSpace=ColorSpace.Linear;PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;PlayerSettings.runInBackground=true;PlayerSettings.Android.forceInternetPermission=false;
 var player=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);player.FindProperty("activeInputHandler").intValue=1;player.ApplyModifiedPropertiesWithoutUndo();
 ConfigureRendering();ConfigureXR();
 var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);new GameObject("Bootstrap").AddComponent<Bootstrap>();EditorSceneManager.SaveScene(scene,Root+"Scenes/01_Bootstrap.unity");
 CreateLab(false);CreateLab(true);
 EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Root+"Scenes/01_Bootstrap.unity",true),new EditorBuildSettingsScene(Root+"Scenes/02_AnatomyLab.unity",true),new EditorBuildSettingsScene(Root+"Scenes/03_PerformanceTest.unity",true)};
 EditorSceneManager.OpenScene(Root+"Scenes/02_AnatomyLab.unity");AssetDatabase.SaveAssets();Debug.Log("[AnatomyLab] PASS setup complete.");
 }
 static void ConfigureRendering(){
 var renderer=AssetDatabase.LoadAssetAtPath<UniversalRendererData>(Root+"Art/Materials/AnatomyRenderer.asset");if(renderer==null){renderer=ScriptableObject.CreateInstance<UniversalRendererData>();AssetDatabase.CreateAsset(renderer,Root+"Art/Materials/AnatomyRenderer.asset");}
 var qualities=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/QualitySettings.asset")[0]);var levels=qualities.FindProperty("m_QualitySettings");levels.arraySize=3;string[] names={"PERFORMANCE","BALANCED","QUALITY"};
 for(int i=0;i<3;i++){
 var path=Root+"Art/Materials/"+names[i]+".asset";var asset=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);if(asset==null){asset=UniversalRenderPipelineAsset.Create(renderer);AssetDatabase.CreateAsset(asset,path);}asset.renderScale=i==0?.8f:i==1?1f:1.15f;asset.msaaSampleCount=i==2?4:2;asset.supportsCameraDepthTexture=false;asset.supportsCameraOpaqueTexture=false;asset.supportsHDR=false;asset.shadowDistance=0;asset.useSRPBatcher=true;
 var entry=levels.GetArrayElementAtIndex(i);entry.FindPropertyRelative("name").stringValue=names[i];entry.FindPropertyRelative("customRenderPipeline").objectReferenceValue=asset;entry.FindPropertyRelative("vSyncCount").intValue=0;EditorUtility.SetDirty(asset);
 if(i==1)GraphicsSettings.defaultRenderPipeline=asset;
 }qualities.ApplyModifiedPropertiesWithoutUndo();QualitySettings.SetQualityLevel(1);QualitySettings.vSyncCount=0;
 }
 static void ConfigureXR(){
 Directory.CreateDirectory(Root+"ScriptableObjects/XR");string path=Root+"ScriptableObjects/XR/General.asset";var settings=AssetDatabase.LoadAssetAtPath<XRGeneralSettingsPerBuildTarget>(path);if(settings==null){settings=ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();AssetDatabase.CreateAsset(settings,path);}if(!settings.HasManagerSettingsForBuildTarget(BuildTargetGroup.Android))settings.CreateDefaultManagerSettingsForBuildTarget(BuildTargetGroup.Android);
 EditorBuildSettings.AddConfigObject(XRGeneralSettings.settingsKey,settings,true);var manager=settings.ManagerSettingsForBuildTarget(BuildTargetGroup.Android);if(!XRPackageMetadataStore.AssignLoader(manager,typeof(OpenXRLoader).FullName,BuildTargetGroup.Android))throw new Exception("[XR] Loader assignment failed");manager.automaticLoading=true;manager.automaticRunning=true;EditorUtility.SetDirty(manager);
 var xr=OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);xr.renderMode=OpenXRSettings.RenderMode.SinglePassInstanced;var quest=xr.GetFeature<MetaQuestFeature>();var touch=xr.GetFeature<OculusTouchControllerProfile>();quest.enabled=true;touch.enabled=true;EditorUtility.SetDirty(quest);EditorUtility.SetDirty(touch);EditorUtility.SetDirty(xr);
 }
 static Material Material(string name,Color color,bool transparent=false){string path=Root+"Art/Materials/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(m==null){m=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(m,path);}m.SetColor("_BaseColor",color);m.SetFloat("_Smoothness",.32f);m.enableInstancing=true;if(transparent){m.SetFloat("_Surface",1);m.SetFloat("_SrcBlend",(float)BlendMode.SrcAlpha);m.SetFloat("_DstBlend",(float)BlendMode.OneMinusSrcAlpha);m.SetFloat("_ZWrite",0);m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");m.renderQueue=3000;}return m;}
 static GameObject Primitive(string name,PrimitiveType type,Vector3 pos,Vector3 scale,Material material){var o=GameObject.CreatePrimitive(type);o.name=name;o.transform.position=pos;o.transform.localScale=scale;o.GetComponent<Renderer>().sharedMaterial=material;UnityEngine.Object.DestroyImmediate(o.GetComponent<Collider>());return o;}
 static void CreateLab(bool performance){
 var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
 RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.32f,.38f,.46f);
 var dark=Material("Lab graphite",new Color(.006f,.010f,.018f));var platform=Material("Platform",new Color(.015f,.04f,.055f));var cyan=Material("Cyan accents",new Color(.04f,.65f,.73f));var opaque=Material("Anatomy shared",Color.white);var transparent=Material("Anatomy transparent",new Color(1,1,1,.24f),true);
 Primitive("Laboratory floor",PrimitiveType.Cube,new Vector3(0,-.05f,2),new Vector3(12,.1f,12),dark);
 Primitive("Central platform",PrimitiveType.Cylinder,new Vector3(.25f,.04f,2.1f),new Vector3(1.25f,.04f,1.25f),platform);
 Primitive("Platform luminous edge",PrimitiveType.Cylinder,new Vector3(.25f,.015f,2.1f),new Vector3(1.29f,.009f,1.29f),cyan);
 for(int i=-5;i<=5;i++){Primitive("Floor seam",PrimitiveType.Cube,new Vector3(i,0,2),new Vector3(.006f,.002f,12),platform);Primitive("Floor seam",PrimitiveType.Cube,new Vector3(0,0,i+2),new Vector3(12,.002f,.006f),platform);}
 var l=new GameObject("Key light").AddComponent<Light>();l.type=LightType.Directional;l.intensity=1.6f;l.shadows=LightShadows.None;l.transform.rotation=Quaternion.Euler(35,-35,0);
 var fill=new GameObject("Fill light").AddComponent<Light>();fill.type=LightType.Directional;fill.color=new Color(.4f,.73f,1);fill.intensity=.7f;fill.transform.rotation=Quaternion.Euler(30,150,0);
 var root=new GameObject("XR Origin");var origin=root.AddComponent<XROrigin>();origin.RequestedTrackingOriginMode=XROrigin.TrackingOriginMode.Floor;var offset=new GameObject("Camera Offset");offset.transform.SetParent(root.transform,false);origin.CameraFloorOffsetObject=offset;
 var cameraObject=new GameObject("Main Camera");cameraObject.tag="MainCamera";cameraObject.transform.SetParent(offset.transform,false);var camera=cameraObject.AddComponent<Camera>();camera.nearClipPlane=.03f;camera.farClipPlane=30;camera.backgroundColor=new Color(.012f,.022f,.042f);camera.clearFlags=CameraClearFlags.SolidColor;cameraObject.AddComponent<AudioListener>();origin.Camera=camera;Pose(cameraObject,"<XRHMD>","centerEyePosition","centerEyeRotation");root.AddComponent<DesktopRig>();
 var manager=new GameObject("XR Interaction Manager").AddComponent<XRInteractionManager>();
 var left=Hand(offset.transform,"Left",manager,cyan);var right=Hand(offset.transform,"Right",manager,cyan);
 var lab=new GameObject("Anatomy Services").AddComponent<LabRuntime>();lab.ViewCamera=camera;lab.Body=new GameObject("Anatomy Body · 1.75m").transform;lab.Body.position=new Vector3(.25f,.08f,2.1f);lab.Opaque=opaque;lab.Transparent=transparent;
 var eventSystem=new GameObject("Event System",typeof(EventSystem),typeof(XRUIInputModule));
 var canvasObject=new GameObject("Holographic console",typeof(RectTransform));var canvas=canvasObject.AddComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;canvas.worldCamera=camera;canvasObject.AddComponent<TrackedDeviceGraphicRaycaster>();canvasObject.AddComponent<GraphicRaycaster>();var rt=(RectTransform)canvas.transform;rt.sizeDelta=new Vector2(940,940);rt.position=new Vector3(-.83f,1.26f,1.42f);rt.localScale=Vector3.one*.001f;rt.rotation=Quaternion.Euler(0,-18,0);canvasObject.AddComponent<Image>().color=new Color(.018f,.045f,.069f,.96f);
 var panel=canvasObject.AddComponent<LabPanel>();panel.Lab=lab;
 Text header=Text("GUATEGEEKS   /   ANATOMY LAB",rt,28,-24,880,44,26);header.color=new Color(.25f,.88f,.93f);
 Text("VR  ·  BODY EXPLORER  /  OFFLINE",rt,28,-69,880,26,17);
 string[] labels={"ANATOMÍA","SISTEMAS","BUSCAR","ESTRUCTURA","EXPLORAR","APRENDIZAJE","AJUSTES"};for(int i=0;i<labels.Length;i++){var page=labels[i];LabPanel.CreateButton(page,rt,22,-120-i*64,208,54,()=>panel.Open(page));}
 panel.Content=LabPanel.Rect("Page content",rt,255,-120,655,680);
 panel.Detail=Text("Apunta para identificar",rt,28,-830,880,30,22);panel.Status=Text("Cargando…",rt,28,-867,880,30,17);panel.Tutorial=Text("",rt,28,-907,880,26,18);
 var quick=LabPanel.Rect("Quick panel",rt,950,-50,230,440);quick.gameObject.AddComponent<Image>().color=new Color(.02f,.05f,.08f,.96f);panel.QuickPanel=quick.gameObject;string[] ids={"skeleton","muscular","digestive","nervous","arterial","venous"};for(int i=0;i<ids.Length;i++){var id=ids[i];LabPanel.CreateButton(id,quick,10,-10-i*48,210,40,()=>{if(lab.Ready)lab.Systems.ToggleSystem(id);});}LabPanel.CreateButton("Exploded 50%",quick,10,-305,210,40,()=>lab.SetExplosion(.5f));LabPanel.CreateButton("Reset",quick,10,-353,210,40,lab.ResetBody);quick.gameObject.SetActive(false);
 var input=root.AddComponent<LabInput>();input.Lab=lab;input.Panel=panel;input.Rig=root.transform;input.LeftRay=left;input.RightRay=right;
 var hud=new GameObject("Development metrics").AddComponent<PerformanceHUD>();hud.Lab=lab;hud.Label=Text("",rt,950,-520,440,105,17);
 if(performance){var harness=lab.gameObject.AddComponent<PerformanceScenario>();harness.Lab=lab;}
 EditorSceneManager.SaveScene(scene,Root+"Scenes/"+(performance?"03_PerformanceTest":"02_AnatomyLab")+".unity");
 }
 static Text Text(string content,Transform parent,float x,float y,float width,float height,int size){var rt=LabPanel.Rect("Text",parent,x,y,width,height);var text=rt.gameObject.AddComponent<Text>();text.text=content;text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");text.fontSize=size;text.color=new Color(.72f,.85f,.89f);text.raycastTarget=false;return text;}
 static void Pose(GameObject go,string device,string position,string rotation){var pose=go.AddComponent<TrackedPoseDriver>();pose.positionInput=new InputActionProperty(new InputAction("Position",InputActionType.Value,device+"/"+position));pose.rotationInput=new InputActionProperty(new InputAction("Rotation",InputActionType.Value,device+"/"+rotation));pose.trackingStateInput=new InputActionProperty(new InputAction("Tracking",InputActionType.Value,device+"/trackingState"));}
 static XRRayInteractor Hand(Transform parent,string side,XRInteractionManager manager,Material material){
 string device="<XRController>{"+side+"Hand}";var o=new GameObject(side+" Ray");o.transform.SetParent(parent,false);Pose(o,device,"pointerPosition","pointerRotation");var ray=o.AddComponent<XRRayInteractor>();ray.interactionManager=manager;ray.maxRaycastDistance=5;ray.enableUIInteraction=true;
 ray.selectInput=new XRInputButtonReader(side+" select",inputSourceMode:XRInputButtonReader.InputSourceMode.InputAction){inputActionPerformed=new InputAction(side+" select",InputActionType.Button,device+"/{TriggerButton}")};ray.uiPressInput=new XRInputButtonReader(side+" UI",inputSourceMode:XRInputButtonReader.InputSourceMode.InputAction){inputActionPerformed=new InputAction(side+" UI",InputActionType.Button,device+"/{TriggerButton}")};var line=o.AddComponent<LineRenderer>();line.sharedMaterial=material;line.startWidth=line.endWidth=.0025f;var visual=o.AddComponent<XRInteractorLineVisual>();visual.lineWidth=.0025f;
 var direct=new GameObject(side+" Direct");direct.transform.SetParent(parent,false);Pose(direct,device,"devicePosition","deviceRotation");var col=direct.AddComponent<SphereCollider>();col.radius=.07f;col.isTrigger=true;var d=direct.AddComponent<XRDirectInteractor>();d.interactionManager=manager;d.selectInput=new XRInputButtonReader(side+" grip",inputSourceMode:XRInputButtonReader.InputSourceMode.InputAction){inputActionPerformed=new InputAction(side+" grip",InputActionType.Button,device+"/{GripButton}")};return ray;
 }
 [MenuItem("GuateGeeks/Anatomy Lab/Build Quest APK")]
 public static void BuildQuest(){AnatomyDatasetImporter.Validate();AnatomyDatasetImporter.Audit();Directory.CreateDirectory("Builds");var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=Array.ConvertAll(EditorBuildSettings.scenes,s=>s.path),locationPathName="Builds/GuateGeeksAnatomyLabVR.apk",target=BuildTarget.Android,options=BuildOptions.Development});File.WriteAllText("Builds/build-result.txt",report.summary.result+"\nBytes: "+report.summary.totalSize+"\nTime: "+report.summary.totalTime);if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new Exception("APK build failed");}
}
