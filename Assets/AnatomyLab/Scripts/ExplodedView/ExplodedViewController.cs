using UnityEngine;
namespace GuateGeeks.AnatomyLab.ExplodedView {
public sealed class ExplodedViewController {
 public float Factor {get;private set;}
 public void SetFactor(float value){Factor=Mathf.Clamp01(value);}
 public static Vector3 Position(Vector3 original,Vector3 center,float factor) {
 var direction=original-center; if(direction.sqrMagnitude<0.000001f)direction=Vector3.up;
 return original+direction.normalized*Mathf.Clamp01(factor)*0.65f;
 }
}
}
