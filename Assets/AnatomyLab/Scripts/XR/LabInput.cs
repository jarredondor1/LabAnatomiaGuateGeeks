using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using GuateGeeks.AnatomyLab.Core;
using GuateGeeks.AnatomyLab.Interaction;
using GuateGeeks.AnatomyLab.UI;
namespace GuateGeeks.AnatomyLab.XR {
public sealed class LabInput : MonoBehaviour {
 public LabRuntime Lab;public LabPanel Panel;public XRRayInteractor LeftRay,RightRay;public Transform Rig;public bool DeviceMode;
 public ExplorationWorkbench Workbench;public DesktopControllerSimulator Controllers;
 public SpatialLabUI Spatial;
 InputAction leftTrigger,rightTrigger,leftGrip,rightGrip,menu,turn,move,growPiece,shrinkPiece;float nextPick,turnLatch;
 SpatialPanelHandle leftPanel,rightPanel;readonly StructureRayManipulator piece=new StructureRayManipulator();
 readonly System.Collections.Generic.List<RaycastResult> uiHits=new System.Collections.Generic.List<RaycastResult>();
 void OnEnable(){leftTrigger=Action("Left select","<XRController>{LeftHand}/{TriggerButton}");rightTrigger=Action("Right select","<XRController>{RightHand}/{TriggerButton}");leftGrip=Action("Left grip","<XRController>{LeftHand}/{GripButton}");rightGrip=Action("Right grip","<XRController>{RightHand}/{GripButton}");menu=Action("Quick menu","<XRController>{LeftHand}/{PrimaryButton}");growPiece=Action("Enlarge part","<XRController>{RightHand}/{PrimaryButton}");shrinkPiece=Action("Reduce part","<XRController>{RightHand}/{SecondaryButton}");turn=new InputAction("Snap turn",InputActionType.Value,"<XRController>{RightHand}/{Primary2DAxis}");turn.Enable();move=new InputAction("Walk",InputActionType.Value,"<XRController>{LeftHand}/{Primary2DAxis}");move.Enable();}
 static InputAction Action(string name,string binding){var a=new InputAction(name,InputActionType.Button,binding);a.Enable();return a;}
 void OnDisable(){leftTrigger?.Dispose();rightTrigger?.Dispose();leftGrip?.Dispose();rightGrip?.Dispose();menu?.Dispose();turn?.Dispose();move?.Dispose();growPiece?.Dispose();shrinkPiece?.Dispose();}
 static Ray Aim(XRRayInteractor ray)=>new Ray(ray.transform.position,ray.transform.forward);
 SpatialPanelHandle HitHandle(XRRayInteractor ray){return ray.TryGetCurrentUIRaycastResult(out var hit)&&hit.gameObject!=null?hit.gameObject.GetComponentInParent<SpatialPanelHandle>():null;}
 public bool TryBeginPieceGrab(Ray ray,Quaternion handRotation){
  if(piece.Held!=null||Lab==null||!Lab.Ready)return false;
  var target=Lab.Pick(ray);if(target==null)return false;
  var wasExtracted=target.Extracted;var position=target.Object.transform.position;var rotation=target.Object.transform.rotation;
  Lab.Selection.Select(target);var adapter=Lab.Extract(target);if(adapter==null)return false;
  if(!wasExtracted)adapter.transform.SetPositionAndRotation(position,rotation);
  return piece.Begin(adapter,ray,handRotation);
 }
 public void EndPieceGrab(){piece.End();}
 void MoveRig(){var axes=move.ReadValue<Vector2>();if(axes.sqrMagnitude<.02f)return;var head=Lab.ViewCamera.transform;var forward=Vector3.ProjectOnPlane(head.forward,Vector3.up).normalized;var right=Vector3.ProjectOnPlane(head.right,Vector3.up).normalized;var delta=(right*axes.x+forward*axes.y)*Time.deltaTime*1.45f;var target=Rig.position+delta;target.x=Mathf.Clamp(target.x,-5.5f,5.5f);target.z=Mathf.Clamp(target.z,-5.5f,7.5f);Rig.position=target;}
 void HandleXRGrabs(){
  if(leftGrip.WasPressedThisFrame()&&piece.Held==null){leftPanel=HitHandle(LeftRay);if(leftPanel!=null&&!leftPanel.BeginGrab(this,Aim(LeftRay),LeftRay.transform.rotation))leftPanel=null;}
  if(rightGrip.WasPressedThisFrame()){rightPanel=HitHandle(RightRay);if(rightPanel!=null&&!rightPanel.BeginGrab(rightGrip,Aim(RightRay),RightRay.transform.rotation))rightPanel=null;
   if(rightPanel==null&&!RightRay.TryGetCurrentUIRaycastResult(out _))TryBeginPieceGrab(Aim(RightRay),RightRay.transform.rotation);
  }
  if(leftPanel!=null){if(leftGrip.IsPressed()&&piece.Held==null)leftPanel.MoveGrab(this,Aim(LeftRay),LeftRay.transform.rotation);else{leftPanel.EndGrab(this);leftPanel=null;}}
  if(rightPanel!=null){if(rightGrip.IsPressed())rightPanel.MoveGrab(rightGrip,Aim(RightRay),RightRay.transform.rotation);else{rightPanel.EndGrab(rightGrip);rightPanel=null;}}
  if(piece.Held!=null){if(rightGrip.IsPressed())piece.Step(Aim(RightRay),RightRay.transform.rotation,turn.ReadValue<Vector2>(),growPiece.IsPressed(),shrinkPiece.IsPressed(),leftGrip.IsPressed(),LeftRay.transform.position,RightRay.transform.position,Time.deltaTime);else EndPieceGrab();}
 }
 void Update(){if(!Lab.Ready)return;
 DeviceMode=XRSettings.isDeviceActive;
 if(menu.WasPressedThisFrame()){if(Spatial!=null)Spatial.CatalogCard.SetActive(!Spatial.CatalogCard.activeSelf);else Panel.ToggleQuick();}
 if(DeviceMode){if(piece.Held==null||!leftGrip.IsPressed())MoveRig();HandleXRGrabs();float x=turn.ReadValue<Vector2>().x;if(piece.Held==null&&Mathf.Abs(x)>.7f&&turnLatch==0){Rig.RotateAround(Lab.ViewCamera.transform.position,Vector3.up,Mathf.Sign(x)*30);turnLatch=1;}if(Mathf.Abs(x)<.3f)turnLatch=0;
 var interactor=Lab.LeftHanded?LeftRay:RightRay;var trigger=Lab.LeftHanded?leftTrigger:rightTrigger;
 if((LeftRay.TryGetCurrentUIRaycastResult(out var leftUI)&&leftUI.gameObject!=null)||(RightRay.TryGetCurrentUIRaycastResult(out var rightUI)&&rightUI.gameObject!=null))return;
 var ray=new Ray(interactor.transform.position,interactor.transform.forward);if(Time.unscaledTime>nextPick){Lab.Selection.Hover(Lab.Pick(ray));nextPick=Time.unscaledTime+.065f;}if(trigger.WasPressedThisFrame())Lab.Selection.Select(Lab.Pick(ray));return;}
 var mouse=Mouse.current;var kb=Keyboard.current;if(mouse==null||kb==null)return;
 if(Workbench!=null&&Workbench.IsTyping){if(kb.escapeKey.wasPressedThisFrame)Workbench.SearchField.DeactivateInputField();return;}
 if(Panel.SearchActive&&Workbench==null)return;
 bool over=false;if(EventSystem.current!=null){uiHits.Clear();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=mouse.position.ReadValue()},uiHits);over=uiHits.Count>0;}
 if(Workbench!=null){if(kb.iKey.wasPressedThisFrame)Workbench.ShowDetails();if(kb.escapeKey.wasPressedThisFrame)Workbench.CloseDetails();}
 if(Workbench!=null&&Workbench.IsDetailsOpen)over=true;
 if(Controllers!=null){

 if(kb.rKey.wasPressedThisFrame){Controllers.Release();Lab.ResetBody();}
 if(kb.eKey.wasPressedThisFrame)Controllers.TakeSelected();if(kb.gKey.wasPressedThisFrame)Workbench.ToggleGrip();if(kb.tKey.wasPressedThisFrame)Workbench.Return();
 if(Controllers.Holding){if(!over)Controllers.Distance=Mathf.Clamp(Controllers.Distance+mouse.scroll.ReadValue().y*.0015f,.35f,4.5f);if(kb.qKey.isPressed)Controllers.RotateHeld(-70*Time.deltaTime);if(kb.cKey.isPressed)Controllers.RotateHeld(70*Time.deltaTime);}
 else if(!over&&mouse.scroll.ReadValue().y!=0)Lab.Scale(mouse.scroll.ReadValue().y>0?1.03f:.97f);
 }else{if(kb.eKey.wasPressedThisFrame)Lab.ExtractSelected();if(kb.tKey.wasPressedThisFrame)Lab.ReturnSelected();if(kb.rKey.wasPressedThisFrame)Lab.ResetBody();}
 if(!over&&mouse.rightButton.isPressed)Lab.Rotate(-mouse.delta.ReadValue().x*.15f);
 if(over){Lab.Selection.Hover(null);return;}
 if(Controllers!=null&&Controllers.Holding&&mouse.leftButton.wasPressedThisFrame){Controllers.Release();return;}
 var rayDesktop=Lab.ViewCamera.ScreenPointToRay(mouse.position.ReadValue());
 if(Time.unscaledTime>nextPick){Lab.Selection.Hover(Lab.Pick(rayDesktop));nextPick=Time.unscaledTime+.065f;}if(mouse.leftButton.wasPressedThisFrame&&(Controllers==null||!Controllers.Holding))Lab.Selection.Select(Lab.Pick(rayDesktop));

 }
}
}
