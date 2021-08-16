using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class NavigationManager : MonoBehaviour
{
    public void ScoresViewPage()
    {

        AuthManager.authManagerSingleton.LoadScoresData();
        //yield return new WaitForSeconds(3);
        //SceneManager.LoadScene("MemosAll");
    }


    public void MemoViewPage()
    { 

        AuthManager.authManagerSingleton.LoadMemoData();
        //yield return new WaitForSeconds(3);
        //SceneManager.LoadScene("MemosAll");
    }

    public void ColorGaze()
    {
        SceneManager.LoadScene("ColourGaze");
    }
    
    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ExitApp()
    {
        Application.Quit();
    }

    public void LogOut()
    {
        AuthManager.authManagerSingleton.SignOutButton();
    }

    

    public void ImageClassificationPanel()
    {
        SceneManager.LoadScene("ImageClassification");
    }

    public void LifeSizeModel()
    {
        SceneManager.LoadScene("LS3DM");
    }

    public void LifeSizeModelVoice()
    {
        SceneManager.LoadScene("VoiceCommandSearch");
    }

    public void VoiceTextChat()
    {
        SceneManager.LoadScene("VoiceChat");
    }

    public void VoiceToText()
    {
        SceneManager.LoadScene("VoiceToText");
    }

    public void QuizPage()
    {
        SceneManager.LoadScene("Quiz");
    }

    public void TextToVoice()
    {
        SceneManager.LoadScene("TextToVoice");
    }

    public void BoatGroundPlane()
    {
        SceneManager.LoadScene("BoatRealLife");
    }

    public void CarGroundPlane()
    {
        SceneManager.LoadScene("CarRealLife");
        //SceneManager.LoadScene("CarRealLife - Copy");
    }

    public void CloudGroundPlane()
    {
        //SceneManager.LoadScene("CloudRealLife");
    }

    public void SheepGroundPlane()
    {
        SceneManager.LoadScene("SheepRealLife");
    }

    public void PigGroundPlane()
    {
        SceneManager.LoadScene("PigRealLife");
    }

    public void AirPlaneGroundPlane()
    {
        //to be made
    }

}
