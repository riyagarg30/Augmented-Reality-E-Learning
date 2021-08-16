using System.Collections;
using System.Collections.Generic;
using TextSpeech;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
public class VoiceController : MonoBehaviour
{
    const string LANG_CODE = "en-US";
    //string obj = "car";

    [SerializeField]
    Text uiText;

    

    private void Start()
    {
        Setup(LANG_CODE);
#if UNITY_ANDROID
    SpeechToText.instance.onPartialResultsCallback = OnPartialSpeechResult;
#endif
        SpeechToText.instance.onResultCallback = OnFinalSpeechResult;
        TextToSpeech.instance.onStartCallBack = OnSpeakStart;
        TextToSpeech.instance.onDoneCallback = OnSpeakStop;

        
    }

    #region TextToSpeech

    public void StartSpeaking(string message)
    {
        TextToSpeech.instance.StartSpeak(message);
        Debug.Log("Talking started");
        Debug.Log(message);
    }

    public void StopSpeaking()
    {
        TextToSpeech.instance.StopSpeak();
    }

    void OnSpeakStart()
    {
        Debug.Log("Talking started");
    }

    void OnSpeakStop()
    {
        Debug.Log("Talking stopped");
    }

    #endregion

    #region SpeechToText

    public void StartListening()
    {
        SpeechToText.instance.StartRecording();
        Debug.Log("Start Talking");
    }

    public void StopListening()
    {
        SpeechToText.instance.StopRecording();
        Debug.Log("Stopped Listening");
    }

    void OnFinalSpeechResult(string result)
    {
        uiText.text = result;
        ChangeScene(result);

    }

    void OnPartialSpeechResult(string result)
    {
        
        uiText.text = result;
        
        //Scene scene = SceneManager.GetActiveScene();
        //string scene_name = "VoiceCommandSearch";
        /*if (scene.Equals(scene_name))
        {
            ChangeScene(result);
        }*/
    }

    #endregion


    public void ChangeScene(string result)
    {
        if (result.ToLower().Contains("car"))
        {
            SceneManager.LoadScene(sceneName: "CarRealLife");
        }

        if (result.ToLower().Contains("cloud"))
        {
            //SceneManager.LoadScene(sceneName: "CloudRealLife");
        }

        if (result.ToLower().Contains("plane"))
        {
            //SceneManager.LoadScene(sceneName: "AirplaneRealLife");
        }



        if (result.ToLower().Contains("boat"))
        {
            SceneManager.LoadScene(sceneName: "BoatRealLife");
        }

        if (result.ToLower().Contains("pig"))
        {
            SceneManager.LoadScene(sceneName: "PigRealLife");
        }

        if (result.ToLower().Contains("sheep"))
        {
            SceneManager.LoadScene(sceneName: "SheepRealLife");
        }


    }

    void Setup(string code)
    {
        TextToSpeech.instance.Setting(code, 1, 1);
        SpeechToText.instance.Setting(code);
    }
}
