using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    public void LoadFeelingsInTheHeart()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadChampion()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadLoveUnderTheStars()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene(4);
    }

    public void LoadOurYouthfulBlossomingMoments()
    {
        SceneManager.LoadScene(5);
    }

    public void LoadLotsOfFun()
    {
        SceneManager.LoadScene(6);
    }

    public void LoadForThePast()
    {
        SceneManager.LoadScene(7);
    }

    public void LoadEATDH()
    {
        SceneManager.LoadScene(8);
    }

    public void LoadLevelSelectNight()
    {
        SceneManager.LoadScene(9);
    }

    public void LoadForThePastNight()
    {
        SceneManager.LoadScene(10);
    }

    public void LoadLotsOfFunNight()
    {
        SceneManager.LoadScene(11);
    }

    public void LoadYellowClock()
    {
        SceneManager.LoadScene(12);
    }

    public void LoadYellowClockNight()
    {
        SceneManager.LoadScene(13);
    }

    public void LoadOurYouthfulBlossomingMomentsNight()
    {
        SceneManager.LoadScene(14);
    }

    public void LoadEATDHNight()
    {
        SceneManager.LoadScene(15);
    }

    public void LoadHowToPlay()
    {
        SceneManager.LoadScene(16);
    }

    public void LoadSlide9()
    {
        SceneManager.LoadScene(17);
    }

    public void LoadFail9()
    {
        SceneManager.LoadScene(18);
    }

    

    public void restartLevel() {
        int returnTo = PlayerPrefs.GetInt("returnTo");
        //Debug.Log(returnTo);
        SceneManager.LoadScene(returnTo);
    }
}