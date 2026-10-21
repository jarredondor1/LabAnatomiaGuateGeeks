using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using GuateGeeks.AnatomyLab.Core;

namespace GuateGeeks.AnatomyLab.UI {
// Adapta el cielo, la retícula y el techo del AWS VR Architect al centro de la mesa anatómica.
public sealed class AnatomyStarkEnvironment : MonoBehaviour {
 static readonly Vector2 ArchStart=new Vector2(6.8f,4.9f),ArchControl=new Vector2(5.1f,6.35f),ArchEnd=new Vector2(2.3f,5.65f);
 readonly List<Material> materials=new List<Material>();
 readonly List<Mesh> meshes=new List<Mesh>();
 readonly List<Transform> gyros=new List<Transform>();
 Transform root,lens;
 Vector3 center,lensScale;

 public void Initialize(LabRuntime lab){
  center=new Vector3(lab.Body.position.x,0,lab.Body.position.z);
  root=new GameObject("Sala envolvente GuateGeeks").transform;root.SetParent(transform,false);
  RenderSettings.skybox=null;RenderSettings.fog=false;
  lab.ViewCamera.farClipPlane=40;
  ConfigureFloor();BuildSky();BuildArchitecture();BuildCanopy();BuildProjector();
 }
 Material Mat(Shader shader,Color color){
  if(shader==null)shader=Resources.Load<Shader>("LabUnlit");
  if(shader==null)throw new System.InvalidOperationException("Falta el shader LabUnlit del laboratorio.");
  var m=new Material(shader);m.color=color;m.SetColor("_Color",color);if(m.HasProperty("_BaseColor"))m.SetColor("_BaseColor",color);materials.Add(m);return m;
 }
 Material Unlit(Color color)=>Mat(null,color);
 GameObject Shape(string name,PrimitiveType type,Vector3 position,Vector3 size,Material material){
  var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(root,false);go.transform.localPosition=position;go.transform.localScale=size;
  go.GetComponent<Renderer>().sharedMaterial=material;var collider=go.GetComponent<Collider>();if(collider!=null)Destroy(collider);return go;
 }
 LineRenderer Path(string name,Vector3[] points,float width,Material material,bool loop=false){
  var go=new GameObject(name);go.transform.SetParent(root,false);var line=go.AddComponent<LineRenderer>();line.useWorldSpace=false;line.sharedMaterial=material;line.positionCount=points.Length;line.SetPositions(points);line.startWidth=line.endWidth=width;line.loop=loop;line.numCornerVertices=2;return line;
 }
 Vector3[] Circle(Vector3 origin,float radius,int count,float height=0){
  var points=new Vector3[count];for(int i=0;i<count;i++){float a=i*Mathf.PI*2/count;points[i]=origin+new Vector3(Mathf.Sin(a)*radius,height,Mathf.Cos(a)*radius);}return points;
 }
 static Vector2 Arch(float t){float u=1-t;return u*u*ArchStart+2*u*t*ArchControl+t*t*ArchEnd;}

 void ConfigureFloor(){
  var shader=Resources.Load<Shader>("LabGrid");var floorMat=Mat(shader,new Color(.012f,.024f,.04f));
  foreach(var renderer in FindObjectsByType<MeshRenderer>()){
   if(renderer.name=="Laboratory floor"){
    renderer.transform.position=new Vector3(0,-.08f,center.z);renderer.transform.localScale=new Vector3(30,.12f,30);renderer.sharedMaterial=floorMat;
   }else if(renderer.name=="Floor seam")renderer.enabled=false;
  }
 }
 void BuildSky(){
  var panorama=Mat(Resources.Load<Shader>("LabPanorama"),Color.white);
  var sky=Shape("Horizonte de Guatemala · cielo panorámico",PrimitiveType.Sphere,center+Vector3.up*1.2f,Vector3.one*19.2f,panorama);
  var renderer=sky.GetComponent<Renderer>();renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
 }
 void BuildArchitecture(){
  var graphite=Unlit(new Color(.08f,.13f,.17f));var steel=Unlit(new Color(.13f,.20f,.24f));
  var cyan=Unlit(new Color(.08f,.62f,.72f));var green=Unlit(new Color(.23f,.75f,.58f));var amber=Unlit(new Color(.96f,.53f,.17f));
  Path("Anillo del laboratorio",Circle(center,4.8f,128,.025f),.012f,green,true);
  Path("Halo del techo",Circle(center,4.8f,128,4.1f),.008f,cyan,true);
  Path("Corona del proyector",Circle(center,2.22f,96,5.59f),.024f,green,true);
  for(int i=0;i<16;i++){
   float a=i*Mathf.PI*2/16;var radial=new Vector3(Mathf.Sin(a),0,Mathf.Cos(a));var p=center+radial*6.8f;
   var rib=Shape("Costilla de titanio "+(i+1),PrimitiveType.Cube,p+Vector3.up*2.45f,new Vector3(.16f,4.9f,.26f),graphite);rib.transform.localRotation=Quaternion.Euler(0,i*22.5f,0);
   Shape("Base estructural",PrimitiveType.Cube,p+Vector3.up*.14f,new Vector3(.42f,.28f,.50f),steel).transform.localRotation=rib.transform.localRotation;
   Path("Tira de energía",new[]{p+Vector3.up*.3f,p+Vector3.up*4.7f},.012f,i%2==0?cyan:green);
   var arch=new Vector3[17];for(int k=0;k<arch.Length;k++){var q=Arch(k/16f);arch[k]=center+radial*q.x+Vector3.up*q.y;}
   Path("Arco estructural",arch,.042f,graphite);var light=new Vector3[arch.Length];for(int k=0;k<arch.Length;k++)light[k]=arch[k]+Vector3.down*.05f;
   Path("Arco luminoso",light,.012f,i%2==0?cyan:green);
   Path("Marca ámbar",new[]{p+Vector3.up*.1f,p+Vector3.up*.36f},.028f,amber);
  }
 }
 void BuildCanopy(){
  const int around=128,overArch=16,inward=6,along=overArch+inward;const float lift=.14f;
  var profile=new Vector2[along+1];for(int k=0;k<=overArch;k++)profile[k]=Arch(k/(float)overArch)+Vector2.up*lift;
  for(int k=1;k<=inward;k++)profile[overArch+k]=Vector2.Lerp(ArchEnd+Vector2.up*lift,new Vector2(1.09f,5.85f),k/(float)inward);
  var vertices=new Vector3[(along+1)*(around+1)];var uvs=new Vector2[vertices.Length];var triangles=new int[along*around*6];
  for(int k=0;k<=along;k++)for(int i=0;i<=around;i++){
   float a=i*Mathf.PI*2/around;int v=k*(around+1)+i;
   vertices[v]=center+new Vector3(Mathf.Sin(a)*profile[k].x,profile[k].y,Mathf.Cos(a)*profile[k].x);
   uvs[v]=new Vector2(i/(float)around,k/(float)along);
  }
  int n=0;for(int k=0;k<along;k++)for(int i=0;i<around;i++){
   int v=k*(around+1)+i,w=v+around+1;triangles[n++]=v;triangles[n++]=w;triangles[n++]=v+1;triangles[n++]=v+1;triangles[n++]=w;triangles[n++]=w+1;
  }
  var mesh=new Mesh{name="Malla hexagonal del techo",vertices=vertices,uv=uvs,triangles=triangles};mesh.RecalculateBounds();meshes.Add(mesh);
  var go=new GameObject("Techo hexagonal holográfico",typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(root,false);go.GetComponent<MeshFilter>().sharedMesh=mesh;
  var material=Mat(Resources.Load<Shader>("LabCanopy"),new Color(.13f,.68f,.7f));material.SetVector("_Cells",new Vector4(72,13,0,0));
  var renderer=go.GetComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
 }
 void BuildProjector(){
  var c=center+Vector3.up*5.65f;var steel=Unlit(new Color(.12f,.18f,.22f));var cyan=Unlit(new Color(.15f,.73f,.79f));var amber=Unlit(new Color(.95f,.55f,.21f));
  Shape("Núcleo del proyector",PrimitiveType.Cylinder,c+Vector3.up*.2f,new Vector3(2.1f,.16f,2.1f),steel);
  Shape("Collar del proyector",PrimitiveType.Cylinder,c,new Vector3(1.5f,.10f,1.5f),steel);
  for(int i=0;i<8;i++){
   float a=(i*45+22.5f)*Mathf.Deg2Rad;var radial=new Vector3(Mathf.Sin(a),0,Mathf.Cos(a));
   var inner=c+radial*1.0f+Vector3.up*.18f;var outer=c+radial*2.3f;var strut=Shape("Brazo del proyector",PrimitiveType.Cube,(inner+outer)*.5f,new Vector3(.07f,.07f,(outer-inner).magnitude),steel);
   strut.transform.rotation=Quaternion.LookRotation(outer-inner,Vector3.up);
   Path("Alimentación luminosa",new[]{inner+Vector3.down*.05f,outer+Vector3.down*.05f},.012f,cyan);
  }
  lens=Shape("Lente del proyector",PrimitiveType.Sphere,c+Vector3.down*.1f,new Vector3(.5f,.13f,.5f),cyan).transform;lensScale=lens.localScale;
  for(int i=0;i<3;i++){
   var pivot=new GameObject("Anillo giroscópico "+(i+1)).transform;pivot.SetParent(root,false);pivot.localPosition=c+Vector3.down*(.22f+i*.08f);
   var line=Path("Órbita del proyector",Circle(Vector3.zero,1.16f+i*.16f,96),i==0?.013f:.008f,i==1?amber:cyan,true);
   line.transform.SetParent(pivot,false);gyros.Add(pivot);
  }
  BuildShaft(c+Vector3.down*.12f);
 }
 void BuildShaft(Vector3 top){
  const int segments=48;const float topRadius=.24f,bottomRadius=1.75f,bottomY=.76f;
  var vertices=new Vector3[(segments+1)*2];var uvs=new Vector2[vertices.Length];var triangles=new int[segments*6];
  for(int i=0;i<=segments;i++){
   float a=i*Mathf.PI*2/segments;var radial=new Vector3(Mathf.Sin(a),0,Mathf.Cos(a));
   vertices[i*2]=top+radial*topRadius;vertices[i*2+1]=new Vector3(top.x,bottomY,top.z)+radial*bottomRadius;
   uvs[i*2]=new Vector2(i/(float)segments,0);uvs[i*2+1]=new Vector2(i/(float)segments,1);
   if(i==segments)break;int v=i*2,k=i*6;triangles[k]=v;triangles[k+1]=v+1;triangles[k+2]=v+2;triangles[k+3]=v+2;triangles[k+4]=v+1;triangles[k+5]=v+3;
  }
  var mesh=new Mesh{name="Haz de proyección",vertices=vertices,uv=uvs,triangles=triangles};mesh.RecalculateBounds();meshes.Add(mesh);
  var go=new GameObject("Haz de proyección",typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(root,false);go.GetComponent<MeshFilter>().sharedMesh=mesh;
  var renderer=go.GetComponent<MeshRenderer>();renderer.sharedMaterial=Mat(Resources.Load<Shader>("LabShaft"),new Color(.03f,.22f,.25f));renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
 }
 void Update(){
  float t=Time.unscaledTime;Shader.SetGlobalFloat("_LabAnimationTime",t);
  for(int i=0;i<gyros.Count;i++)if(gyros[i]!=null)gyros[i].localRotation=Quaternion.Euler(5+i*3,t*(i%2==0?9:-6),i*4);
  if(lens!=null)lens.localScale=Vector3.Scale(lensScale,new Vector3(1+.04f*Mathf.Sin(t*1.3f),1+.18f*Mathf.Sin(t*1.3f),1+.04f*Mathf.Sin(t*1.3f)));
 }
 void OnDestroy(){foreach(var material in materials)if(material!=null)Destroy(material);foreach(var mesh in meshes)if(mesh!=null)Destroy(mesh);}
}
}
