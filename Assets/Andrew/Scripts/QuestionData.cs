
using UnityEngine;

[CreateAssetMenu]
public class QuestionData : ScriptableObject
{
    public int question_id;
    [TextArea(3, 3)]
    public string questionGr;
    [TextArea(3, 3)]
    public string questionEn;
    [TextArea(3, 3)]
    public string[] answersGr;
    [TextArea(3, 3)]
    public string[] answersEn;
    public bool multipleChoice;
    public bool dontTranslate;
}
