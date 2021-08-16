using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScoreElement : MonoBehaviour
{

    public TMP_Text quizNumber;
    public TMP_Text totalQuestions;
    public TMP_Text correctQuestions;
    public TMP_Text pigQuestions;
    public TMP_Text pigQuestionsCorrect;
    public TMP_Text carQuestions;
    public TMP_Text carQuestionsCorrect;
    public TMP_Text boatQuestions;
    public TMP_Text boatQuestionsCorrect;
    public TMP_Text sheepQuestions;
    public TMP_Text sheepQuestionsCorrect;


    public void NewScoreElement(string quizNumber1, string totalQuestions1, string correctQuestions1, string pigQuestions1, string pigQuestionsCorrect1, string carQuestions1, string carQuestionsCorrect1, string boatQuestions1, string boatQuestionsCorrect1, string sheepQuestions1, string sheepQuestionsCorrect1)
    {
        quizNumber.text=quizNumber1;
        totalQuestions.text = totalQuestions1;
        correctQuestions.text = correctQuestions1;
        pigQuestions.text = pigQuestions1;
        pigQuestionsCorrect.text = pigQuestionsCorrect1;
        carQuestions.text = carQuestions1;
        carQuestionsCorrect.text = carQuestionsCorrect1;
        boatQuestions.text = boatQuestions1;
        boatQuestionsCorrect.text = boatQuestionsCorrect1;
        sheepQuestions.text = sheepQuestions1;
        sheepQuestionsCorrect.text = sheepQuestions1;


}


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
