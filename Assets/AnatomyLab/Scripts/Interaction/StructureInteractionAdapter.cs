using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using GuateGeeks.AnatomyLab.Core;
using GuateGeeks.AnatomyLab.XR;
namespace GuateGeeks.AnatomyLab.Interaction {
public sealed class StructureInteractionAdapter : MonoBehaviour {
 public StructureState State; public Transform Body; public float Tolerance=.10f, AngleTolerance=35;
 public XRGrabInteractable Grab {get;private set;} public event Action<StructureInteractionAdapter,bool> Released;
 public bool CanSnap=>Vector3.Distance(transform.position,Body.TransformPoint(State.Data.originalPosition))<Tolerance && Quaternion.Angle(transform.rotation,Body.rotation*State.Data.originalRotation)<AngleTolerance;
 public void Initialize(StructureState state,Transform body){
 State=state;Body=body;state.Extracted=true;
 var collider=gameObject.AddComponent<BoxCollider>();collider.center=state.Mesh.bounds.center;collider.size=Vector3.Max(state.Mesh.bounds.size,Vector3.one*.018f);
 var rb=gameObject.AddComponent<Rigidbody>();rb.useGravity=false;rb.isKinematic=true;
 Grab=gameObject.AddComponent<XRGrabInteractable>();Grab.movementType=XRBaseInteractable.MovementType.Kinematic;Grab.throwOnDetach=false;Grab.retainTransformParent=true;
 Grab.selectEntered.AddListener(_=>HapticFeedbackService.Pulse());
 Grab.selectExited.AddListener(_=>Released?.Invoke(this,CanSnap));
 }
 public void DisableInteraction(){if(Grab!=null){Grab.selectExited.RemoveAllListeners();Grab.enabled=false;Destroy(Grab);}var c=GetComponent<Collider>();if(c!=null){c.enabled=false;Destroy(c);}var r=GetComponent<Rigidbody>();if(r!=null)Destroy(r);}
}
}
