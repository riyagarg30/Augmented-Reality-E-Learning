using System.Collections;
using System.Collections.Generic;
using TextSpeech;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.SceneManagement;


public class VoiceControllerMemo : MonoBehaviour
{
    const string LANG_CODE = "en-US";
    //string obj = "car";

    [SerializeField]
    Text uiText;

    string memoText = "";

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

    public void StartSpeaking()
    {
        TextToSpeech.instance.StartSpeak(uiText.text);
        Debug.Log("Talking started");
        Debug.Log(uiText);
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
        memoText = memoText + result;
        uiText.text = memoText;

        


    }

    void OnPartialSpeechResult(string result)
    {

        uiText.text = memoText + " " +result;
        
    }

    #endregion


    void Setup(string code)
    {
        TextToSpeech.instance.Setting(code, 1, 1);
        SpeechToText.instance.Setting(code);
    }

    public void SaveData()
    {
        AuthManager.authManagerSingleton.SaveMemo(uiText.text);
    }
}
