using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Android;
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
 PlayerSettings.companyName="GuateGeeks";PlayerSettings.productName="GuateGeeks Anatomía VR";PlayerSettings.bundleVersion="0.1.0";
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
 [MenuItem("GuateGeeks/Anatomy Lab/Regenerar escenas del laboratorio")]
 public static void RebuildLabScenes(){CreateLab(false);CreateLab(true);EditorSceneManager.OpenScene(Root+"Scenes/02_AnatomyLab.unity");AssetDatabase.SaveAssets();}
 static void CreateProjectionTable(Material dark,Material platform,Material cyan){
 var table=new GameObject("Mesa holográfica GuateGeeks");var center=new Vector3(.25f,0,2.1f);
 GameObject Part(string name,PrimitiveType type,Vector3 offset,Vector3 size,Material material){var part=Primitive(name,type,center+offset,size,material);part.transform.SetParent(table.transform,true);return part;}
 Part("Pedestal blindado",PrimitiveType.Cylinder,new Vector3(0,.31f,0),new Vector3(2.7f,.31f,2.7f),dark);
 Part("Faldón metálico",PrimitiveType.Cylinder,new Vector3(0,.54f,0),new Vector3(3.25f,.11f,3.25f),platform);
 Part("Consola de proyección",PrimitiveType.Cylinder,new Vector3(0,.67f,0),new Vector3(3.85f,.045f,3.85f),platform);
 Part("Bisel metálico",PrimitiveType.Cylinder,new Vector3(0,.705f,0),new Vector3(3.74f,.012f,3.74f),Material("Bisel titanio",new Color(.173f,.243f,.294f)));
 Part("Superficie de proyección",PrimitiveType.Cylinder,new Vector3(0,.72f,0),new Vector3(3.62f,.008f,3.62f),Material("Cristal de proyección",new Color(.027f,.075f,.114f)));
 var leaf=Material("Luz quetzal",new Color(.20f,.73f,.57f));var etched=Material("Grabado de mesa",new Color(.055f,.26f,.31f));
 LineRenderer Line(string name,Vector3[] points,float width,Material material,bool loop=false){var go=new GameObject(name);go.transform.SetParent(table.transform,false);var line=go.AddComponent<LineRenderer>();line.useWorldSpace=false;line.loop=loop;line.positionCount=points.Length;line.SetPositions(points);line.sharedMaterial=material;line.startWidth=line.endWidth=width;line.numCornerVertices=2;return line;}
 Vector3[] Circle(float radius,float height,int count){var points=new Vector3[count];for(int i=0;i<count;i++){float a=i*Mathf.PI*2/count;points[i]=center+new Vector3(Mathf.Sin(a)*radius,height,Mathf.Cos(a)*radius);}return points;}
 Line("Borde quetzal",Circle(1.94f,.672f,128),.05f,leaf,true);
 Line("Anillo cian del cristal",Circle(1.82f,.736f,128),.014f,cyan,true);
 Line("Borde inferior verde",Circle(1.99f,.585f,128),.02f,leaf,true);
 foreach(var radius in new[]{1.81f,1.70f,1.20f,.48f,.42f})Line("Grabado circular",Circle(radius,.741f,radius>1?96:48),.0025f,etched,true);
 for(int i=0;i<72;i++){float a=i*Mathf.PI/36;var d=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));Line("Marca radial "+i,new[]{center+d*(i%6==0?1.55f:1.62f)+Vector3.up*.742f,center+d*1.67f+Vector3.up*.742f},i%6==0?.0035f:.002f,i%6==0?cyan:etched);}
 for(int i=0;i<8;i++){float a=i*Mathf.PI/4;var d=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));var side=new Vector3(-d.z,0,d.x);Line("Circuito del núcleo "+i,new[]{center+d*.55f+Vector3.up*.742f,center+d*.9f+Vector3.up*.742f,center+d*1.1f+side*.16f+Vector3.up*.742f,center+d*1.4f+side*.16f+Vector3.up*.742f},.003f,etched);}
 var scanner=new GameObject("Escáner del perímetro");scanner.transform.SetParent(table.transform,false);scanner.transform.position=center+Vector3.up*.746f;scanner.AddComponent<AnatomyTableScanner>();
 for(int i=0;i<3;i++){var points=new Vector3[32];for(int j=0;j<points.Length;j++){float a=(i*120f+j*85f/(points.Length-1))*Mathf.Deg2Rad;points[j]=new Vector3(Mathf.Sin(a)*1.76f,0,Mathf.Cos(a)*1.76f);}var arc=new GameObject("Segmento de escaneo");arc.transform.SetParent(scanner.transform,false);var line=arc.AddComponent<LineRenderer>();line.useWorldSpace=false;line.positionCount=points.Length;line.SetPositions(points);line.sharedMaterial=cyan;line.startWidth=line.endWidth=.004f;}
 var logo=Part("Logotipo GuateGeeks",PrimitiveType.Quad,new Vector3(0,.737f,0),new Vector3(2.25f*2048f/1856f,2.25f*1408f/1856f,1),dark);
 logo.transform.rotation=Quaternion.Euler(90f,0,0);
 var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"Resources/Branding/GuateGeeksSDF.png");if(texture==null)throw new Exception("No se encontró el logotipo luminoso de GuateGeeks");
 var logoMaterial=Material("GuateGeeks logo",Color.white);logoMaterial.shader=Shader.Find("GuateGeeks/Logo");if(logoMaterial.shader==null)throw new Exception("No se encontró el shader del logotipo de GuateGeeks");logoMaterial.SetTexture("_MainTex",texture);
 var logoRenderer=logo.GetComponent<Renderer>();logoRenderer.sharedMaterial=logoMaterial;logoRenderer.shadowCastingMode=ShadowCastingMode.Off;logoRenderer.receiveShadows=false;
 var eyes=new GameObject("Ojos GuateGeeks de la mesa");eyes.transform.SetParent(table.transform,false);eyes.transform.position=center+new Vector3(.083f,.744f,-.073f);
 var eyeAnimation=eyes.AddComponent<AnatomyTableEyes>();
 Material ink=Material("Tinta ojos",new Color(.02f,.025f,.03f));Material white=Material("Blanco ojos",Color.white);
 GameObject Disc(string name,Transform parent,Vector3 at,float radius,Material material){var go=Primitive(name,PrimitiveType.Cylinder,Vector3.zero,new Vector3(radius*2,.003f,radius*2),material);go.transform.SetParent(parent,false);go.transform.localPosition=at;return go;}
 Transform Eye(string name,float x,float z){var group=new GameObject(name).transform;group.SetParent(eyes.transform,false);group.localPosition=new Vector3(x,0,z);Disc("Marco",group,Vector3.zero,.232f,ink);var content=new GameObject("Párpado").transform;content.SetParent(group,false);Disc("Esclerótica",content,new Vector3(0,.003f,0),.208f,white);var pupil=Disc("Pupila",content,new Vector3(0,.007f,0),.076f,ink).transform;Disc("Reflejo",pupil,new Vector3(.020f,.003f,.021f),.022f,white);eyeAnimation.SetEye(name=="Ojo izquierdo",content,pupil);return group;}
 Eye("Ojo izquierdo",-.30f,-.061f);Eye("Ojo derecho",.30f,.061f);
 // La cara superior queda casi al ras del suelo; el pedestal permanece enterrado.
 table.transform.position=Vector3.down*.74f;
 }
 static void CreateLab(bool performance){
 var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
 RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.32f,.38f,.46f);
 var dark=Material("Lab graphite",new Color(.006f,.010f,.018f));var platform=Material("Platform",new Color(.015f,.04f,.055f));var cyan=Material("Cyan accents",new Color(.04f,.65f,.73f));var opaque=Material("Anatomy shared",Color.white);var transparent=Material("Anatomy transparent",new Color(1,1,1,.24f),true);
 Primitive("Laboratory floor",PrimitiveType.Cube,new Vector3(0,-.05f,2),new Vector3(24,.1f,24),dark);
 CreateProjectionTable(dark,platform,cyan);
 for(int i=-10;i<=10;i++){Primitive("Floor seam",PrimitiveType.Cube,new Vector3(i,0,2),new Vector3(.006f,.002f,24),platform);Primitive("Floor seam",PrimitiveType.Cube,new Vector3(0,0,i+2),new Vector3(24,.002f,.006f),platform);}
 var l=new GameObject("Key light").AddComponent<Light>();l.type=LightType.Directional;l.intensity=1.6f;l.shadows=LightShadows.None;l.transform.rotation=Quaternion.Euler(35,-35,0);
 var fill=new GameObject("Fill light").AddComponent<Light>();fill.type=LightType.Directional;fill.color=new Color(.4f,.73f,1);fill.intensity=.7f;fill.transform.rotation=Quaternion.Euler(30,150,0);
 var root=new GameObject("XR Origin");var origin=root.AddComponent<XROrigin>();origin.RequestedTrackingOriginMode=XROrigin.TrackingOriginMode.Floor;var offset=new GameObject("Camera Offset");offset.transform.SetParent(root.transform,false);origin.CameraFloorOffsetObject=offset;
 var cameraObject=new GameObject("Main Camera");cameraObject.tag="MainCamera";cameraObject.transform.SetParent(offset.transform,false);cameraObject.transform.localPosition=new Vector3(.15f,1.82f,-2.1f);cameraObject.transform.localRotation=Quaternion.Euler(11,0,0);var camera=cameraObject.AddComponent<Camera>();camera.nearClipPlane=.03f;camera.farClipPlane=30;camera.backgroundColor=new Color(.012f,.022f,.042f);camera.clearFlags=CameraClearFlags.SolidColor;cameraObject.AddComponent<AudioListener>();origin.Camera=camera;Pose(cameraObject,"<XRHMD>","centerEyePosition","centerEyeRotation");root.AddComponent<DesktopRig>();
 var manager=new GameObject("XR Interaction Manager").AddComponent<XRInteractionManager>();
 var left=Hand(offset.transform,"Left",manager,cyan);var right=Hand(offset.transform,"Right",manager,cyan);
 var lab=new GameObject("Anatomy Services").AddComponent<LabRuntime>();lab.ViewCamera=camera;lab.Body=new GameObject("Anatomy Body · estatura personalizable").transform;lab.Body.position=new Vector3(.25f,0,2.1f);lab.Opaque=opaque;lab.Transparent=transparent;
 var eventSystem=new GameObject("Event System",typeof(EventSystem),typeof(XRUIInputModule));
 var canvasObject=new GameObject("Holographic console",typeof(RectTransform));var canvas=canvasObject.AddComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;canvas.worldCamera=camera;canvasObject.AddComponent<TrackedDeviceGraphicRaycaster>();canvasObject.AddComponent<GraphicRaycaster>();var rt=(RectTransform)canvas.transform;rt.sizeDelta=new Vector2(940,940);rt.position=new Vector3(-.83f,1.26f,1.42f);rt.localScale=Vector3.one*.001f;rt.rotation=Quaternion.Euler(0,-18,0);canvasObject.AddComponent<Image>().color=Color.clear;AnatomyHoloTheme.Decorate(rt,true);
 var panel=canvasObject.AddComponent<LabPanel>();panel.Lab=lab;
 Text header=Text("GUATEGEEKS / LABORATORIO DE ANATOMÍA",rt,28,-24,880,44,26);header.color=AnatomyHoloTheme.Cyan;
 Text("EXPLORADOR ANATÓMICO / SIN CONEXIÓN",rt,28,-69,880,26,17);
 string[] labels={"ANATOMÍA","SISTEMAS","BUSCAR","ESTRUCTURA","EXPLORAR","APRENDIZAJE","AJUSTES"};for(int i=0;i<labels.Length;i++){var page=labels[i];LabPanel.CreateButton(page,rt,22,-120-i*64,208,54,()=>panel.Open(page));}
 panel.Content=LabPanel.Rect("Page content",rt,255,-120,655,680);
 panel.Detail=Text("Apunta para identificar",rt,28,-830,880,30,22);panel.Status=Text("Cargando…",rt,28,-867,880,30,17);panel.Tutorial=Text("",rt,28,-907,880,26,18);
 var quick=LabPanel.Rect("Quick panel",rt,950,-50,230,440);quick.gameObject.AddComponent<Image>().color=Color.clear;AnatomyHoloTheme.Decorate(quick,true);panel.QuickPanel=quick.gameObject;string[] ids={"skeleton","muscular","digestive","nervous","arterial","venous"};for(int i=0;i<ids.Length;i++){var id=ids[i];LabPanel.CreateButton(id,quick,10,-10-i*48,210,40,()=>{if(lab.Ready)lab.Systems.ToggleSystem(id);});}LabPanel.CreateButton("Separar al 50 %",quick,10,-305,210,40,()=>lab.SetExplosion(.5f));LabPanel.CreateButton("Restablecer",quick,10,-353,210,40,lab.ResetBody);quick.gameObject.SetActive(false);
 var input=root.AddComponent<LabInput>();input.Lab=lab;input.Panel=panel;input.Rig=root.transform;input.LeftRay=left;input.RightRay=right;
 var hud=new GameObject("Development metrics").AddComponent<PerformanceHUD>();hud.Lab=lab;hud.Label=Text("",rt,950,-520,440,105,17);
 if(performance){var harness=lab.gameObject.AddComponent<PerformanceScenario>();harness.Lab=lab;}
 EditorSceneManager.SaveScene(scene,Root+"Scenes/"+(performance?"03_PerformanceTest":"02_AnatomyLab")+".unity");
 }
 static Text Text(string content,Transform parent,float x,float y,float width,float height,int size){var rt=LabPanel.Rect("Text",parent,x,y,width,height);var text=rt.gameObject.AddComponent<Text>();text.text=content;text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");text.fontSize=size;text.color=AnatomyHoloTheme.Muted;text.raycastTarget=false;return text;}
 static void Pose(GameObject go,string device,string position,string rotation){var pose=go.AddComponent<TrackedPoseDriver>();pose.positionInput=new InputActionProperty(new InputAction("Position",InputActionType.Value,device+"/"+position));pose.rotationInput=new InputActionProperty(new InputAction("Rotation",InputActionType.Value,device+"/"+rotation));pose.trackingStateInput=new InputActionProperty(new InputAction("Tracking",InputActionType.Value,device+"/trackingState"));}
 static XRRayInteractor Hand(Transform parent,string side,XRInteractionManager manager,Material material){
 string device="<XRController>{"+side+"Hand}";var o=new GameObject(side+" Ray");o.transform.SetParent(parent,false);Pose(o,device,"pointerPosition","pointerRotation");var ray=o.AddComponent<XRRayInteractor>();ray.interactionManager=manager;ray.maxRaycastDistance=5;ray.enableUIInteraction=true;
 ray.selectInput=new XRInputButtonReader(side+" select",inputSourceMode:XRInputButtonReader.InputSourceMode.InputAction){inputActionPerformed=new InputAction(side+" select",InputActionType.Button,device+"/{TriggerButton}")};ray.uiPressInput=new XRInputButtonReader(side+" UI",inputSourceMode:XRInputButtonReader.InputSourceMode.InputAction){inputActionPerformed=new InputAction(side+" UI",InputActionType.Button,device+"/{TriggerButton}")};var line=o.AddComponent<LineRenderer>();line.sharedMaterial=material;line.startWidth=line.endWidth=.0025f;var visual=o.AddComponent<XRInteractorLineVisual>();visual.lineWidth=.0025f;
 var direct=new GameObject(side+" Direct");direct.transform.SetParent(parent,false);Pose(direct,device,"devicePosition","deviceRotation");var col=direct.AddComponent<SphereCollider>();col.radius=.07f;col.isTrigger=true;var d=direct.AddComponent<XRDirectInteractor>();d.interactionManager=manager;d.selectInput=new XRInputButtonReader(side+" grip",inputSourceMode:XRInputButtonReader.InputSourceMode.InputAction){inputActionPerformed=new InputAction(side+" grip",InputActionType.Button,device+"/{GripButton}")};return ray;
 }
 [MenuItem("GuateGeeks/Anatomy Lab/Build Quest APK")]
 public static void BuildQuest(){ConfigureQuestIcon();AnatomyDatasetImporter.Validate();AnatomyDatasetImporter.Audit();Directory.CreateDirectory("Builds");var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=Array.ConvertAll(EditorBuildSettings.scenes,s=>s.path),locationPathName="Builds/GuateGeeksAnatomyLabVR.apk",target=BuildTarget.Android,options=BuildOptions.Development});File.WriteAllText("Builds/build-result.txt",report.summary.result+"\nBytes: "+report.summary.totalSize+"\nTime: "+report.summary.totalTime);if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new Exception("APK build failed");}

 [MenuItem("GuateGeeks/Anatomy Lab/Configurar icono de Quest")]
 public static void ConfigureQuestIcon(){
 const string iconPath=Root+"Art/UI/guategeeks.png";
 var icon=AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
 if(icon==null)throw new Exception("No se encontró el icono de GuateGeeks: "+iconPath);
 PlayerSettings.productName="GuateGeeks Anatomía VR";
 var kind=AndroidPlatformIconKind.Adaptive;
 var slots=PlayerSettings.GetPlatformIcons(NamedBuildTarget.Android,kind);
 if(slots.Length==0)throw new Exception("No hay espacios de icono adaptable para Android");
 foreach(var slot in slots)slot.SetTextures(new[]{icon,icon});
 PlayerSettings.SetPlatformIcons(NamedBuildTarget.Android,kind,slots);
 var configured=PlayerSettings.GetPlatformIcons(NamedBuildTarget.Android,kind);
 if(configured.Length!=slots.Length||configured[0].GetTexture(0)!=icon||configured[0].GetTexture(1)!=icon)
 throw new Exception("Unity no conservó el icono adaptable de GuateGeeks");
 EditorUtility.SetDirty(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
 AssetDatabase.SaveAssets();
 Debug.Log("[AnatomyLab] Icono de Quest configurado con "+iconPath);
 }
}
