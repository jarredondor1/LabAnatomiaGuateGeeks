using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using GuateGeeks.AnatomyLab.Core;
using GuateGeeks.AnatomyLab.Interaction;
namespace GuateGeeks.AnatomyLab.XR {
// Mouse manipulation shares the XRI grab lifecycle, without controller meshes or rays.
public sealed class DesktopControllerSimulator : MonoBehaviour {
 public LabRuntime Lab; public bool LeftHand; public float Distance=1.1f;
 public bool Holding=>held!=null&&held.Grab!=null&&held.Grab.isSelected;
 StructureInteractionAdapter held; XRDirectInteractor left,right; XRInteractionManager manager;

 public Transform ActiveHand=>(LeftHand?left:right).transform;
 public void Initialize(LabRuntime lab){Lab=lab;manager=Object.FindAnyObjectByType<XRInteractionManager>();
 foreach(var hand in transform.root.GetComponentsInChildren<XRDirectInteractor>()){if(hand.name.StartsWith("Left"))left=hand;else right=hand;}
 if(left==null||right==null){enabled=false;return;}
 foreach(var hand in new[]{left,right}){hand.selectInput.inputSourceMode=XRInputButtonReader.InputSourceMode.ManualValue;hand.selectInput.manualPerformed=false;}
 }
 bool PointerOverUI(){var events=UnityEngine.EventSystems.EventSystem.current;if(events==null)return false;var hits=new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();events.RaycastAll(new UnityEngine.EventSystems.PointerEventData(events){position=Mouse.current.position.ReadValue()},hits);return hits.Count>0;}
 public void ToggleHand(){if(Holding)Release();LeftHand=!LeftHand;}
 public void TakeSelected(){if(Lab.Selection.Selected==null)return;if(Holding)Release();held=Lab.Extract(Lab.Selection.Selected);var hand=LeftHand?left:right;hand.transform.SetPositionAndRotation(held.transform.position,held.transform.rotation);Distance=Mathf.Clamp(Vector3.Distance(Lab.ViewCamera.transform.position,held.transform.position),.45f,3.5f);hand.selectInput.manualPerformed=true;hand.selectInput.manualValue=1;manager.SelectEnter((IXRSelectInteractor)hand,(IXRSelectInteractable)held.Grab);}
 public void Release(){if(held==null)return;var hand=LeftHand?left:right;hand.selectInput.manualPerformed=false;hand.selectInput.manualValue=0;if(held.Grab!=null&&held.Grab.isSelected)manager.SelectExit((IXRSelectInteractor)hand,(IXRSelectInteractable)held.Grab);held=null;}
 public void RotateHeld(float degrees){if(Holding)ActiveHand.Rotate(Lab.ViewCamera.transform.up,degrees,Space.World);}
 void Update(){if(!Lab.Ready||left==null)return;var cam=Lab.ViewCamera.transform;var mouse=Mouse.current;
 if(!Holding){left.selectInput.manualPerformed=false;right.selectInput.manualPerformed=false;}
 var passive=LeftHand?right:left;passive.transform.SetPositionAndRotation(cam.TransformPoint(new Vector3(LeftHand?.26f:-.26f,-.25f,.65f)),cam.rotation);
 if(!Holding)ActiveHand.SetPositionAndRotation(cam.TransformPoint(new Vector3(LeftHand?-.26f:.26f,-.25f,.65f)),cam.rotation);
 if(mouse!=null&&Holding&&!PointerOverUI()){var ray=Lab.ViewCamera.ScreenPointToRay(mouse.position.ReadValue());ActiveHand.position=ray.GetPoint(Distance);}
 }
}
}
