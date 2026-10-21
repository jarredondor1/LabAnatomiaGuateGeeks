using UnityEngine;
namespace GuateGeeks.AnatomyLab.ExplodedView {
public sealed class ExplodedViewController {
 public float Factor {get;private set;}
 public void SetFactor(float value){Factor=Mathf.Clamp01(value);}
 public static Vector3 Position(Vector3 original,Vector3 center,float factor) {
 var direction=original-center; if(direction.sqrMagnitude<0.000001f)direction=Vector3.up;
 return original+direction.normalized*Mathf.Clamp01(factor)*0.65f;
 }
 // Mantiene cada pieza y sus dimensiones dentro del volumen sobre la mesa.
 public static Vector3 Position(Vector3 original,Vector3 center,float factor,Bounds bounds,float radius,float ceiling){
  float amount=Mathf.Clamp01(factor);if(amount<=0)return original;
  var extent=bounds.extents;
  // Una malla que representa el cuerpo entero es referencia, no una pieza para dispersar.
  if(extent.y>.6f)return original;
  var side=new Vector2(original.x-center.x,original.z-center.z);
  if(side.sqrMagnitude<.0004f){float angle=original.y*8.4f;side=new Vector2(Mathf.Sin(angle),Mathf.Cos(angle));}
  side.Normalize();
  var proposed=original+new Vector3(side.x*.36f,Mathf.Sign(original.y-center.y)*.14f,side.y*.36f)*amount;
  var horizontal=new Vector2(proposed.x-center.x,proposed.z-center.z);
  float footprint=new Vector2(extent.x,extent.z).magnitude;
  float allowed=Mathf.Max(0,radius-footprint);
  if(horizontal.magnitude>allowed){horizontal=horizontal.normalized*allowed;proposed.x=center.x+horizontal.x;proposed.z=center.z+horizontal.y;}
  float lower=Mathf.Max(original.y,extent.y-.015f);
  float upper=Mathf.Max(original.y,ceiling-extent.y);
  proposed.y=Mathf.Clamp(proposed.y,lower,upper);
  return proposed;
 }
}
}
