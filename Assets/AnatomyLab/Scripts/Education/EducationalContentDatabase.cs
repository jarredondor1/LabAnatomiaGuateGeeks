using System;
using UnityEngine;
namespace GuateGeeks.AnatomyLab.Education {
[Serializable] public class EducationalEntry {public string id,title,function,location,source;public string[] related;}
[Serializable] public class EducationalContentDatabase {
 public EducationalEntry[] entries=Array.Empty<EducationalEntry>();
 public static EducationalContentDatabase Load(){var text=Resources.Load<TextAsset>("Anatomy/education");return text==null?new EducationalContentDatabase():JsonUtility.FromJson<EducationalContentDatabase>(text.text);}
 public EducationalEntry Find(string id){foreach(var e in entries)if(e.id==id)return e;return null;}
}
[Serializable] public class LearningStatistics {public int questions,correct,incorrect,correctPlacements,wrongPlacements;public float elapsed,completionTime;public float Percentage=>questions==0?0:100f*correct/questions;}
public sealed class QuizManager {
 public LearningStatistics Statistics=new LearningStatistics(); public string TargetId {get;private set;} public bool Active=>TargetId!=null;
 public void Ask(string id){TargetId=id;}
 public bool Answer(string id){if(!Active)return false; bool correct=id==TargetId;Statistics.questions++;if(correct){Statistics.correct++;TargetId=null;}else Statistics.incorrect++;Save();return correct;}
 public void Save()=>PlayerPrefs.SetString("anatomy.statistics",JsonUtility.ToJson(Statistics));
 public void Load(){var saved=PlayerPrefs.GetString("anatomy.statistics","");if(saved.Length>0)Statistics=JsonUtility.FromJson<LearningStatistics>(saved);}
}
}
