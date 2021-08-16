using System.Collections;
using System.Collections.Generic;
using TextSpeech;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using TMPro;


public class QuizUIManager : MonoBehaviour
{
    public static QuizUIManager instance;

    public GameObject StartQuizUI;
    public GameObject AnswerUI;
    public GameObject ContinueUI;
    public TMP_Text resultText;

    const string LANG_CODE = "en-US";
    
    
    string userAnswer;
    private static string selectedObj;
    private static bool StartBool=true;
    private static bool AnswerBool = false;
    private static bool ContinueBool = false;

    private static int totalQuestion = 0;
    private static int correctAnswers = 0;
    private static int pigCount = 0;
    private static int pigCorrect = 0;
    private static int carCount = 0;
    private static int carCorrect = 0;
    private static int boatCount = 0;
    private static int boatCorrect = 0;
    private static int sheepCount = 0;
    private static int sheepCorrect = 0;


    private void Start()
    {
                
        Setup(LANG_CODE);

        

        SpeechToText.instance.onResultCallback = OnFinalSpeechResult;
        TextToSpeech.instance.onStartCallBack = OnSpeakStart;
        TextToSpeech.instance.onDoneCallback = OnSpeakStop;
        StartQuizUI.SetActive(StartBool);
        ContinueUI.SetActive(ContinueBool);
        AnswerUI.SetActive(AnswerBool);

        Debug.Log(selectedObj);
    }
    
    public void StartListening()
    {
        SpeechToText.instance.StartRecording();
        Debug.Log("Start Talking");

        
    }
    
    void OnFinalSpeechResult(string result)
    {
        
        userAnswer = result;
        testResult();

    }

    public void StartQuiz()
    {
        Quiz();
        StartBool = false;
        AnswerBool = true;
        
    }

    public void testResult()
    {
        
        
        if (userAnswer.ToLower().Contains(selectedObj))
        {
            correctAnswers += 1;
            resultText.text = "Correct Answer";

            if (selectedObj.Equals("car"))
            {
                carCorrect += 1;
            }
            else if (selectedObj.Equals("boat"))
            {
                boatCorrect += 1;
            }
            else if (selectedObj.Equals("sheep"))
            {
                sheepCorrect += 1;
            }
            else if (selectedObj.Equals("pig"))
            {
                pigCorrect += 1;
            }

        }
        else
        {
            resultText.text = "Wrong Answer\n" + "Correct Answer: " + selectedObj;
        }
        AnswerBool = false;
        ContinueBool = true;

        ContinueUI.SetActive(ContinueBool);
        AnswerUI.SetActive(AnswerBool);

    }

    public void Quiz()
    {
        totalQuestion += 1;
        int i = Random.Range(0, 15);
        switch (i)
        {
            case 0:
                SceneManager.LoadScene(sceneName: "QuizCarRealLife");
                selectedObj = "car";
                carCount += 1;
                break;
            case 1:
                SceneManager.LoadScene(sceneName: "QuizPigRealLife");
                selectedObj = "pig";
                pigCount += 1;
                break;
            case 2:
                SceneManager.LoadScene(sceneName: "QuizBoatRealLife");
                selectedObj = "boat";
                boatCount += 1;
                break;
            case 3:
                SceneManager.LoadScene(sceneName: "QuizSheepRealLife");
                selectedObj = "sheep";
                sheepCount += 1;
                break;
            case 4:
                SceneManager.LoadScene(sceneName: "QuizCarRealLife");
                selectedObj = "car";
                carCount += 1;
                break;
            case 5:
                SceneManager.LoadScene(sceneName: "QuizPigRealLife");
                selectedObj = "pig";
                pigCount += 1;
                break;
            case 6:
                SceneManager.LoadScene(sceneName: "QuizBoatRealLife");
                selectedObj = "boat";
                boatCount += 1;
                break;
            case 7:
                SceneManager.LoadScene(sceneName: "QuizSheepRealLife");
                selectedObj = "sheep";
                sheepCount += 1;
                break;
            case 8:
                SceneManager.LoadScene(sceneName: "QuizCarRealLife");
                selectedObj = "car";
                carCount += 1;
                break;
            case 9:
                SceneManager.LoadScene(sceneName: "QuizPigRealLife");
                selectedObj = "pig";
                pigCount += 1;
                break;
            case 10:
                SceneManager.LoadScene(sceneName: "QuizBoatRealLife");
                selectedObj = "boat";
                boatCount += 1;
                break;
            case 11:
                SceneManager.LoadScene(sceneName: "QuizSheepRealLife");
                selectedObj = "sheep";
                sheepCount += 1;
                break;
            case 12:
                SceneManager.LoadScene(sceneName: "QuizCarRealLife");
                selectedObj = "car";
                carCount += 1;
                break;
            case 13:
                SceneManager.LoadScene(sceneName: "QuizPigRealLife");
                selectedObj = "pig";
                pigCount += 1;
                break;
            case 14:
                SceneManager.LoadScene(sceneName: "QuizBoatRealLife");
                selectedObj = "boat";
                boatCount += 1;
                break;
            case 15:
                SceneManager.LoadScene(sceneName: "QuizSheepRealLife");
                selectedObj = "sheep";
                sheepCount += 1;
                break;
        }
        
        
    }

    public void ContinueQuiz()
    {
        AnswerBool = true;
        ContinueBool = false;
        Quiz();
    }

    public void EndQuiz()
    {
        AuthManager.authManagerSingleton.SaveQuizData(totalQuestion,correctAnswers,carCount,carCorrect,pigCount,pigCorrect,boatCount,boatCorrect,sheepCount,sheepCorrect);
        ContinueBool = false;
        StartBool = true;
        totalQuestion = 0;
        correctAnswers = 0;
        pigCount = 0;
        pigCorrect = 0;
        carCount = 0;
        carCorrect = 0;
        boatCount = 0;
        boatCorrect = 0;
        sheepCount = 0;
        sheepCorrect = 0;
        SceneManager.LoadScene(sceneName: "Menu");
    }

    void Setup(string code)
    {
        TextToSpeech.instance.Setting(code, 1, 1);
        SpeechToText.instance.Setting(code);
    }

    void OnSpeakStart()
    {
        Debug.Log("Talking started");
    }

    void OnSpeakStop()
    {
        Debug.Log("Talking stopped");
    }


}
