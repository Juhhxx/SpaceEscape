using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestion", menuName = "Question")]
public class QuestionSO : ScriptableObject
{
    public Option[] options;
    public bool isInputBased;
    public string correctAnswer; // <- New field

    public string hintField;

    [TextArea] public string missionDescription;
}
