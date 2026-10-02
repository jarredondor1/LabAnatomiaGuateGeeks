using UnityEngine;
using UnityEngine.XR;
namespace GuateGeeks.AnatomyLab.XR {
public static class HapticFeedbackService {
 public static void Pulse(float strength=.18f,float duration=.04f){foreach(var node in new[]{XRNode.LeftHand,XRNode.RightHand}){var d=InputDevices.GetDeviceAtXRNode(node);if(d.TryGetHapticCapabilities(out var cap)&&cap.supportsImpulse)d.SendHapticImpulse(0,strength,duration);}}
}
}
