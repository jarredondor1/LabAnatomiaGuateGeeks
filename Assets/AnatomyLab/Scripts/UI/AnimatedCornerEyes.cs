using UnityEngine;
using UnityEngine.UI;

namespace GuateGeeks.AnatomyLab.UI {
// Usa GeekEyes.png del escritorio como firma viva de cada tarjeta.
public sealed class AnimatedCornerEyes:MonoBehaviour {
 RectTransform image;
 Material eyesMaterial;
 Transform viewer;
 float nextBlink,blinkStart=-1,nextLook;
 Vector2 gaze,saccade;

 public static AnimatedCornerEyes Create(Transform parent,float x,float y,float width){
  var texture=Resources.Load<Texture2D>("Branding/GeekEyes");if(texture==null)return null;
  var frame=LabPanel.Rect("Ojos GuateGeeks",parent,x,y,width,width*texture.height/texture.width);
  var raw=frame.gameObject.AddComponent<RawImage>();raw.texture=texture;raw.raycastTarget=false;
  var eyes=frame.gameObject.AddComponent<AnimatedCornerEyes>();eyes.image=frame;eyes.viewer=Camera.main!=null?Camera.main.transform:null;
  var shader=Resources.Load<Shader>("GeekEyesAnimated");if(shader!=null){eyes.eyesMaterial=new Material(shader);raw.material=eyes.eyesMaterial;}
  eyes.nextBlink=Time.unscaledTime+Random.Range(1.5f,4f);return eyes;
 }
 void Update(){
  if(image==null)return;
  float now=Time.unscaledTime;
  if(now>=nextLook){saccade=Random.insideUnitCircle*.16f;nextLook=now+Random.Range(.6f,1.8f);}
  Vector2 look=Vector2.zero;
  if(viewer!=null){var local=image.InverseTransformPoint(viewer.position);look=new Vector2(local.x,local.y)/Mathf.Max(100,Mathf.Abs(local.z)*.5f);}
  gaze=Vector2.Lerp(gaze,Vector2.ClampMagnitude(look+saccade,1),1-Mathf.Exp(-8*Time.unscaledDeltaTime));
  if(eyesMaterial!=null)eyesMaterial.SetVector("_Gaze",new Vector4(gaze.x,gaze.y,0,0));
  if(blinkStart<0&&now>=nextBlink)blinkStart=now;
  float open=1;
  if(blinkStart>=0){float phase=(now-blinkStart)/.18f;if(phase>=1){blinkStart=-1;nextBlink=now+Random.Range(2.5f,6f);}else open=1-.93f*Mathf.Sin(phase*Mathf.PI);}
  image.localScale=new Vector3(1,open,1);
 }
 void OnDestroy(){if(eyesMaterial!=null)Destroy(eyesMaterial);}
}
}
