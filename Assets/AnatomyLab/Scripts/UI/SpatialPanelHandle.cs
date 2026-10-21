using UnityEngine;
using UnityEngine.EventSystems;

namespace GuateGeeks.AnatomyLab.UI {
// El asa deja libres los botones de la tarjeta y admite ratón o agarre XR.
public sealed class SpatialPanelHandle : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler {
 public RectTransform Panel;
 Camera view;
 Vector3 pointerOffset, grabOffset;
 Quaternion grabRotation, initialAim;
 float grabDistance;
 object owner;
 readonly Vector3[] candidateCorners=new Vector3[4];
 bool stageResolved, hasStage;
 Vector2 stageCenter;
 float stageRadius, stageTop;
 const float FloorClearance=.03f;

 public void OnPointerDown(PointerEventData e){if(Panel==null)return;view=e.pressEventCamera??Camera.main;var ray=view.ScreenPointToRay(e.position);var plane=new Plane(Panel.forward,Panel.position);if(plane.Raycast(ray,out var distance))pointerOffset=Panel.position-ray.GetPoint(distance);}
 public void OnDrag(PointerEventData e){if(Panel==null||view==null||owner!=null)return;var ray=view.ScreenPointToRay(e.position);var plane=new Plane(Panel.forward,Panel.position);if(plane.Raycast(ray,out var distance))SetSafePose(ray.GetPoint(distance)+pointerOffset,Panel.rotation);}
 public void OnEndDrag(PointerEventData e){view=null;}

 public bool BeginGrab(object hand,Ray ray,Quaternion aim){
  if(Panel==null||owner!=null||hand==null)return false;
  var plane=new Plane(Panel.forward,Panel.position);if(!plane.Raycast(ray,out grabDistance))return false;
  owner=hand;grabDistance=Mathf.Clamp(grabDistance,.25f,5f);grabOffset=Panel.position-ray.GetPoint(grabDistance);grabRotation=Panel.rotation;initialAim=aim;return true;
 }
 public void MoveGrab(object hand,Ray ray,Quaternion aim){
  if(owner==null||!ReferenceEquals(owner,hand))return;
  var delta=aim*Quaternion.Inverse(initialAim);SetSafePose(ray.GetPoint(grabDistance)+delta*grabOffset,delta*grabRotation);
 }
 public void EndGrab(object hand){if(owner!=null&&ReferenceEquals(owner,hand))owner=null;}
 void OnDisable(){owner=null;}

 void SetSafePose(Vector3 target,Quaternion rotation){
  ResolveStage();
  var start=Panel.position;var startRotation=Panel.rotation;
  var steps=Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(Vector3.Distance(start,target)/.08f,Quaternion.Angle(startRotation,rotation)/5f)),1,256);
  var accepted=start;var acceptedRotation=startRotation;
  for(int i=1;i<=steps;i++){
   float t=(float)i/steps;var candidate=Vector3.Lerp(start,target,t);var candidateRotation=Quaternion.Slerp(startRotation,rotation,t);
   FillCorners(candidate,candidateRotation);
   float minimumY=Mathf.Min(candidateCorners[0].y,candidateCorners[1].y,candidateCorners[2].y,candidateCorners[3].y);
   if(minimumY<FloorClearance){candidate.y+=FloorClearance-minimumY;FillCorners(candidate,candidateRotation);}
   if(IntersectsStage(candidateCorners))break;
   accepted=candidate;acceptedRotation=candidateRotation;
  }
  Panel.SetPositionAndRotation(accepted,acceptedRotation);
 }

 void FillCorners(Vector3 position,Quaternion rotation){
  var rect=Panel.rect;var scale=Panel.lossyScale;
  candidateCorners[0]=position+rotation*new Vector3(rect.xMin*scale.x,rect.yMin*scale.y,0);
  candidateCorners[1]=position+rotation*new Vector3(rect.xMin*scale.x,rect.yMax*scale.y,0);
  candidateCorners[2]=position+rotation*new Vector3(rect.xMax*scale.x,rect.yMax*scale.y,0);
  candidateCorners[3]=position+rotation*new Vector3(rect.xMax*scale.x,rect.yMin*scale.y,0);
 }

 void ResolveStage(){
  if(stageResolved)return;stageResolved=true;
  var table=GameObject.Find("Mesa holográfica GuateGeeks");if(table==null)return;
  var bezel=table.transform.Find("Bisel metálico");var surface=table.transform.Find("Superficie de proyección");
  if(bezel==null||surface==null)return;
  var bezelRenderer=bezel.GetComponent<Renderer>();var surfaceRenderer=surface.GetComponent<Renderer>();
  if(bezelRenderer==null||surfaceRenderer==null)return;
  var bounds=bezelRenderer.bounds;stageCenter=new Vector2(bounds.center.x,bounds.center.z);
  stageRadius=Mathf.Max(bounds.extents.x,bounds.extents.z)+.035f;
  stageTop=surfaceRenderer.bounds.max.y+.03f;
  hasStage=true;
 }

 bool IntersectsStage(Vector3[] corners){
  if(!hasStage)return false;
  float minY=float.MaxValue,maxY=float.MinValue;
  for(int i=0;i<4;i++){minY=Mathf.Min(minY,corners[i].y);maxY=Mathf.Max(maxY,corners[i].y);}
  if(minY>stageTop||maxY<0)return false;
  float radiusSquared=stageRadius*stageRadius;
  for(int i=0;i<4;i++){
   var a=new Vector2(corners[i].x,corners[i].z);var b=new Vector2(corners[(i+1)%4].x,corners[(i+1)%4].z);
   var edge=b-a;float lengthSquared=edge.sqrMagnitude;
   float t=lengthSquared>0?Mathf.Clamp01(Vector2.Dot(stageCenter-a,edge)/lengthSquared):0;
   if((a+edge*t-stageCenter).sqrMagnitude<radiusSquared)return true;
  }
  // La tarjeta puede cubrir la mesa aun cuando ninguna de sus esquinas la toque.
  bool positive=false,negative=false,nonDegenerate=false;
  for(int i=0;i<4;i++){
   var a=new Vector2(corners[i].x,corners[i].z);var b=new Vector2(corners[(i+1)%4].x,corners[(i+1)%4].z);
   float cross=(b.x-a.x)*(stageCenter.y-a.y)-(b.y-a.y)*(stageCenter.x-a.x);
   if(cross>0.000001f){positive=true;nonDegenerate=true;}else if(cross<-.000001f){negative=true;nonDegenerate=true;}
  }
  return nonDegenerate&&!(positive&&negative);
 }
}
}
