using UnityEngine;

namespace GuateGeeks.AnatomyLab.UI {
// La marca del cristal mira a quien examina el cuerpo y parpadea suavemente.
public sealed class AnatomyTableEyes:MonoBehaviour {
 [SerializeField] Transform leftContent,leftPupil,rightContent,rightPupil;
 float nextBlink,blinkStart=-1,nextLook;
 Vector2 saccade;

 public void SetEye(bool left,Transform content,Transform pupil){
  if(left){leftContent=content;leftPupil=pupil;}else{rightContent=content;rightPupil=pupil;}
 }
 void OnEnable(){nextBlink=Time.unscaledTime+Random.Range(1.5f,4f);nextLook=Time.unscaledTime;}
 void LateUpdate(){
  float now=Time.unscaledTime;if(now>=nextLook){saccade=Random.insideUnitCircle*.012f;nextLook=now+Random.Range(.6f,2.2f);}
  var camera=Camera.main;var offset=Vector2.zero;
  if(camera!=null){var local=transform.InverseTransformPoint(camera.transform.position);offset=Vector2.ClampMagnitude(new Vector2(local.x,local.z)*.012f,.045f);}
  var look=offset+saccade;Move(leftPupil,look);Move(rightPupil,look);
  if(blinkStart<0&&now>=nextBlink)blinkStart=now;
  float open=1;
  if(blinkStart>=0){float phase=(now-blinkStart)/.17f;if(phase>=1){blinkStart=-1;nextBlink=now+Random.Range(2.5f,6f);}else open=1-.91f*Mathf.Sin(phase*Mathf.PI);}
  if(leftContent!=null)leftContent.localScale=new Vector3(1,1,open);
  if(rightContent!=null)rightContent.localScale=new Vector3(1,1,open);
 }
 static void Move(Transform pupil,Vector2 look){if(pupil==null)return;var target=new Vector3(look.x,pupil.localPosition.y,look.y);pupil.localPosition=Vector3.Lerp(pupil.localPosition,target,1-Mathf.Exp(-12f*Time.unscaledDeltaTime));}
}
}
