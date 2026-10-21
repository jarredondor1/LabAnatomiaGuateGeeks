using UnityEngine;
using GuateGeeks.AnatomyLab.Core;

namespace GuateGeeks.AnatomyLab.UI {
// Conserva las señales anatómicas encima del escenario inmersivo.
public sealed class AnatomyStageVisuals:MonoBehaviour {
 public LabRuntime Lab;
 Material cyan,muted;
 LineRenderer marker;
 readonly Vector3[] outline=new Vector3[5];

 public void Initialize(LabRuntime lab){
  Lab=lab;
  gameObject.AddComponent<AnatomyStarkEnvironment>().Initialize(lab);
  var table=GameObject.Find("Mesa holográfica GuateGeeks");
  if(table!=null)table.AddComponent<AnatomyProjectionTable>().Initialize(lab);
  cyan=Material(AnatomyHoloTheme.Cyan*.78f);
  muted=Material(AnatomyHoloTheme.Grid*.75f);
  var center=lab.Body.position+new Vector3(0,1.05f,1.4f);
  for(int k=0;k<2;k++){
   var arc=Line("Arco de diagnóstico",145,.003f,k==0?cyan:muted);
   for(int i=0;i<145;i++){
    float a=(18+i)*Mathf.Deg2Rad;
    arc.SetPosition(i,center+new Vector3(Mathf.Cos(a)*(1.25f+k*.08f),Mathf.Sin(a)*(1.25f+k*.08f),0));
   }
  }
  for(int i=0;i<49;i++){
   float a=(18+i*3f)*Mathf.Deg2Rad;
   var radial=new Vector3(Mathf.Cos(a),Mathf.Sin(a),0);
   var mark=Line("Marca de escaneo",2,.003f,muted);
   mark.SetPosition(0,center+radial*1.36f);mark.SetPosition(1,center+radial*(i%4==0?1.45f:1.39f));
  }
  marker=Line("Indicador de selección",5,.002f,cyan);marker.enabled=false;
 }
 static Material Material(Color color){var shader=Resources.Load<Shader>("LabUnlit");if(shader==null)throw new System.InvalidOperationException("Falta el shader LabUnlit de los indicadores.");var m=new Material(shader);m.SetColor("_Color",color);return m;}
 LineRenderer Line(string name,int count,float width,Material material){
  var go=new GameObject(name);go.transform.SetParent(transform,false);
  var line=go.AddComponent<LineRenderer>();line.sharedMaterial=material;line.positionCount=count;line.startWidth=line.endWidth=width;return line;
 }
 void LateUpdate(){
  if(marker==null||Lab==null)return;
  var selected=Lab.Selection.Selected;marker.enabled=selected!=null&&selected.Object!=null&&selected.Object.activeInHierarchy;
  if(!marker.enabled)return;
  var bounds=selected.Renderer.bounds;var camera=Lab.ViewCamera.transform;float radius=Mathf.Max(.045f,bounds.extents.magnitude*.75f);
  outline[0]=bounds.center-camera.right*radius+camera.up*radius;
  outline[1]=bounds.center+camera.right*radius+camera.up*radius;
  outline[2]=bounds.center+camera.right*radius-camera.up*radius;
  outline[3]=bounds.center-camera.right*radius-camera.up*radius;
  outline[4]=outline[0];marker.SetPositions(outline);
 }
 void OnDestroy(){if(cyan!=null)Destroy(cyan);if(muted!=null)Destroy(muted);}
}
}
