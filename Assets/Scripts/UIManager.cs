using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextSpeech;
using UnityEngine.Android;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;

    //const string LANG_CODE = "en-US";

    private void Start()
    {

    #if UNITY_ANDROID
        if(!Permission.HasUserAuthorizedPermission(Permission.Microphone)){
            Permission.RequestUserPermission(Permission.Microphone);
        }
    #endif




        //Setup(LANG_CODE);

        //CheckPermission();
    }
    /*void Setup(string code)
    {
        TextToSpeech.instance.Setting(code, 1, 1);
        SpeechToText.instance.Setting(code);
    }*/

    /*void CheckPermission()
    {
#if UNITY_ANDROID
    if(!Permission.HasUserAuthorizedPermission(Permission.Microphone)){
        Permission.RequestUserPermission(Permission.Microphone);
    }
#endif
    }*/


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    //Functions to change the login screen UI
    public void LoginScreen() //Back button
    {
        loginUI.SetActive(true);
        registerUI.SetActive(false);
    }
    public void RegisterScreen() // Regester button
    {
        loginUI.SetActive(false);
        registerUI.SetActive(true);
    }
}
