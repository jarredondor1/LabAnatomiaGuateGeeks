using UnityEngine;

namespace GuateGeeks.AnatomyLab.Interaction {
// Controls one extracted structure with the Quest pointer, leaving the anatomy body anchored.
public sealed class StructureRayManipulator {
 public StructureInteractionAdapter Held {get;private set;}
 float distance, scaleFactor, twoHandDistance, twoHandScale, yaw;
 bool twoHandActive;
 Vector3 pointerOffset;
 Quaternion rotationOffset;

 public bool Begin(StructureInteractionAdapter adapter,Ray pointer,Quaternion handRotation){
  if(adapter==null||adapter.State==null||!adapter.State.Extracted)return false;
  if(Held!=null)End();
  Held=adapter;adapter.BeginManualGrab();var part=adapter.transform;
  var center=part.TransformPoint(adapter.State.Mesh.bounds.center);
  distance=Mathf.Clamp(Vector3.Dot(center-pointer.origin,pointer.direction),.3f,4.5f);
  pointerOffset=Quaternion.Inverse(handRotation)*(center-pointer.GetPoint(distance));
  rotationOffset=Quaternion.Inverse(handRotation)*part.rotation;
  var original=adapter.State.Data.originalScale.x;
  scaleFactor=Mathf.Clamp(Mathf.Abs(original)>0.00001f?part.localScale.x/original:1f,.5f,6f);
  yaw=0;twoHandActive=false;
  return true;
 }

 public void Step(Ray pointer,Quaternion handRotation,Vector2 stick,bool enlarge,bool reduce,bool secondGrip,Vector3 leftHand,Vector3 rightHand,float deltaTime){
  if(Held==null||!Held.State.Extracted){Held=null;return;}
  var seconds=Mathf.Clamp(deltaTime,0,.1f);
  distance=Mathf.Clamp(distance+stick.y*1.15f*seconds,.3f,4.5f);
  yaw+=stick.x*85f*seconds;
  if(enlarge!=reduce)scaleFactor=Mathf.Clamp(scaleFactor*Mathf.Exp((enlarge?1:-1)*1.35f*seconds),.5f,6f);
  if(secondGrip){
   var separation=Vector3.Distance(leftHand,rightHand);
   if(!twoHandActive){twoHandActive=true;twoHandDistance=Mathf.Max(separation,.08f);twoHandScale=scaleFactor;}
   else scaleFactor=Mathf.Clamp(twoHandScale*separation/twoHandDistance,.5f,6f);
  }else twoHandActive=false;
  pointerOffset=Vector3.Lerp(pointerOffset,Vector3.zero,1-Mathf.Exp(-12f*seconds));
  var part=Held.transform;
  part.localScale=Held.State.Data.originalScale*scaleFactor;
  part.rotation=Quaternion.AngleAxis(yaw,Vector3.up)*handRotation*rotationOffset;
  var center=pointer.GetPoint(distance)+handRotation*pointerOffset;
  part.position=center-part.rotation*Vector3.Scale(Held.State.Mesh.bounds.center,part.lossyScale);
 }

 public void End(){
  var adapter=Held;Held=null;twoHandActive=false;
  if(adapter!=null){adapter.EndManualGrab();if(adapter.State.Extracted)adapter.ReleaseDesktop();}
 }
}
}
