using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using GuateGeeks.AnatomyLab.Core;
using GuateGeeks.AnatomyLab.UI;
namespace GuateGeeks.AnatomyLab.XR {
public sealed class LabInput : MonoBehaviour {
 public LabRuntime Lab;public LabPanel Panel;public XRRayInteractor LeftRay,RightRay;public Transform Rig;public bool DeviceMode;
 InputAction leftTrigger,rightTrigger,menu,turn;float nextPick,turnLatch;bool dragging;
 void OnEnable(){leftTrigger=Action("Left select","<XRController>{LeftHand}/{TriggerButton}");rightTrigger=Action("Right select","<XRController>{RightHand}/{TriggerButton}");menu=Action("Quick menu","<XRController>{LeftHand}/{PrimaryButton}");turn=new InputAction("Snap turn",InputActionType.Value,"<XRController>{RightHand}/{Primary2DAxis}");turn.Enable();}
 static InputAction Action(string name,string binding){var a=new InputAction(name,InputActionType.Button,binding);a.Enable();return a;}
 void OnDisable(){leftTrigger?.Dispose();rightTrigger?.Dispose();menu?.Dispose();turn?.Dispose();}
 void Update(){if(!Lab.Ready)return;
 DeviceMode=XRSettings.isDeviceActive;
 if(menu.WasPressedThisFrame())Panel.ToggleQuick();
 if(DeviceMode){float x=turn.ReadValue<Vector2>().x;if(Mathf.Abs(x)>.7f&&turnLatch==0){Rig.RotateAround(Lab.ViewCamera.transform.position,Vector3.up,Mathf.Sign(x)*30);turnLatch=1;}if(Mathf.Abs(x)<.3f)turnLatch=0;
 var interactor=Lab.LeftHanded?LeftRay:RightRay;var trigger=Lab.LeftHanded?leftTrigger:rightTrigger;
 if(interactor.TryGetCurrentUIRaycastResult(out var uiHit)&&uiHit.gameObject!=null)return;
 var ray=new Ray(interactor.transform.position,interactor.transform.forward);if(Time.unscaledTime>nextPick){Lab.Selection.Hover(Lab.Pick(ray));nextPick=Time.unscaledTime+.065f;}if(trigger.WasPressedThisFrame())Lab.Selection.Select(Lab.Pick(ray));return;}
 var mouse=Mouse.current;var kb=Keyboard.current;if(mouse==null||kb==null)return;
 if(Panel.SearchActive)return;
 if(kb.spaceKey.wasPressedThisFrame)Panel.ToggleQuick();if(kb.rKey.wasPressedThisFrame)Lab.ResetBody();if(kb.eKey.wasPressedThisFrame)Lab.ExtractSelected();if(kb.tKey.wasPressedThisFrame)Lab.ReturnSelected();
 if(mouse.rightButton.isPressed)Lab.Rotate(-mouse.delta.ReadValue().x*.15f);
 if(mouse.scroll.ReadValue().y!=0)Lab.Scale(mouse.scroll.ReadValue().y>0?1.03f:.97f);
 var rayDesktop=Lab.ViewCamera.ScreenPointToRay(mouse.position.ReadValue());
 if(kb.gKey.wasPressedThisFrame&&Lab.Selection.Selected!=null&&Lab.Selection.Selected.Extracted)dragging=!dragging;
 if(dragging&&Lab.Selection.Selected!=null&&Lab.Selection.Selected.Extracted){var s=Lab.Selection.Selected;s.Object.transform.position=rayDesktop.GetPoint(.9f);if(kb.qKey.isPressed)s.Object.transform.Rotate(Vector3.up,-60*Time.deltaTime);if(kb.eKey.isPressed)s.Object.transform.Rotate(Vector3.up,60*Time.deltaTime);}
 bool over=EventSystem.current!=null&&EventSystem.current.IsPointerOverGameObject();if(over)return;
 if(Time.unscaledTime>nextPick){Lab.Selection.Hover(Lab.Pick(rayDesktop));nextPick=Time.unscaledTime+.065f;}if(mouse.leftButton.wasPressedThisFrame)Lab.Selection.Select(Lab.Pick(rayDesktop));
 }
}
}
