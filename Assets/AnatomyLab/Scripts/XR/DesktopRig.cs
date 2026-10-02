using UnityEngine;
using UnityEngine.InputSystem.XR;
using Unity.XR.CoreUtils;
namespace GuateGeeks.AnatomyLab.XR {
public sealed class DesktopRig:MonoBehaviour {
 void Awake(){
#if UNITY_EDITOR || UNITY_STANDALONE
 foreach(var pose in GetComponentsInChildren<TrackedPoseDriver>())pose.enabled=false;
 var origin=GetComponent<XROrigin>();origin.Camera.transform.localPosition=new Vector3(0,1.45f,-.8f);origin.Camera.transform.localRotation=Quaternion.Euler(10,0,0);
#endif
 }
}
}
