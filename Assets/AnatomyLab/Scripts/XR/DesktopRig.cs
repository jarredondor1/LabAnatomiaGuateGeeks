using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;
namespace GuateGeeks.AnatomyLab.XR {
public sealed class DesktopRig:MonoBehaviour {
 public void ZoomToward(Vector3 target,float factor){
#if UNITY_EDITOR || UNITY_STANDALONE
 if(UnityEngine.XR.XRSettings.isDeviceActive||factor<=0)return;
 var camera=GetComponent<XROrigin>().Camera.transform;
 var offset=Vector3.ProjectOnPlane(target-camera.position,Vector3.up);
 var distance=offset.magnitude;if(distance<.001f)return;
 var desired=Mathf.Clamp(distance/factor,1.65f,6f);
 transform.position+=offset.normalized*(distance-desired);
#endif
 }
 void Awake(){
#if UNITY_ANDROID && !UNITY_EDITOR
 // El seguimiento del visor reemplaza la posición local de la cámara al iniciar.
 // Colocar el origen aquí conserva la distancia a la mesa que se ve en Unity.
 transform.position=new Vector3(0,0,-2.1f);
#endif
#if UNITY_EDITOR || UNITY_STANDALONE
 foreach(var pose in GetComponentsInChildren<TrackedPoseDriver>())pose.enabled=false;
 var origin=GetComponent<XROrigin>();origin.Camera.transform.localPosition=new Vector3(.15f,1.82f,-2.1f);origin.Camera.transform.localRotation=Quaternion.Euler(11,0,0);
#endif
 }
 void Update(){
#if UNITY_EDITOR || UNITY_STANDALONE
 if(UnityEngine.XR.XRSettings.isDeviceActive||Keyboard.current==null)return;
 var kb=Keyboard.current;var axis=new Vector2((kb.dKey.isPressed?1:0)-(kb.aKey.isPressed?1:0),(kb.wKey.isPressed?1:0)-(kb.sKey.isPressed?1:0));
 if(axis.sqrMagnitude>.01f){float speed=kb.leftShiftKey.isPressed?3.2f:1.55f;var cam=GetComponent<XROrigin>().Camera.transform;var forward=Vector3.ProjectOnPlane(cam.forward,Vector3.up).normalized;var right=Vector3.ProjectOnPlane(cam.right,Vector3.up).normalized;transform.position+=(right*axis.x+forward*axis.y).normalized*speed*Time.deltaTime;}
#endif
 }
}
}
