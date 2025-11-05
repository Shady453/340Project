using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public CanvasGroup StartCanvasGroup;
    public CanvasGroup ConfigCanvasGroup;
    public CanvasGroup PlayCanvasGroup;
    public CanvasGroup PauseCanvasGroup;
    public CanvasGroup SettingsCanvasGroup;
    public CanvasGroup GameoverCanvasGroup;

    public TMP_Text AliveCount;
    public TMP_Text SpawnedThisRound;
    public TMP_Text ToSpawnThisRound;
    
    private List<CanvasGroup> canvasGroups;

    public void ShowAliveCount(int value)
    {
        AliveCount.text = value.ToString();
    }
    
    public void ShowSpawnedThisRound(int value)
    {
        SpawnedThisRound.text = value.ToString();
    }
    
    public void ShowToSpawnThisRound(int value)
    {
        ToSpawnThisRound.text = value.ToString();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroups = new List<CanvasGroup>();
        canvasGroups.Add(StartCanvasGroup);
        canvasGroups.Add(ConfigCanvasGroup);
        canvasGroups.Add(PlayCanvasGroup);
        canvasGroups.Add(PauseCanvasGroup);
        canvasGroups.Add(SettingsCanvasGroup);
        canvasGroups.Add(GameoverCanvasGroup);
        
        ShowStartScreen();
    }

    public void ShowStartScreen()
    {
        HideAllScreens();
        ShowCanvasGroup(StartCanvasGroup);
    }
    
    public void ShowConfigScreen()
    {
        HideAllScreens();
        ShowCanvasGroup(ConfigCanvasGroup);
    }
    
    public void ShowPlayScreen()
    {
        HideAllScreens();
        ShowCanvasGroup(PlayCanvasGroup);
    }
    
    public void ShowPauseScreen()
    {
        HideAllScreens();
        ShowCanvasGroup(PauseCanvasGroup);
    }
    
    public void ShowSettingsScreen()
    {
        HideAllScreens();
        ShowCanvasGroup(SettingsCanvasGroup);
    }
    
    public void ShowGameoverScreen()
    {
        HideAllScreens();
        ShowCanvasGroup(GameoverCanvasGroup);
    }

    private void HideAllScreens()
    {
        foreach (CanvasGroup canvasGroup in canvasGroups)
            HideCanvasGroup(canvasGroup);
    }

    private void ShowCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    
    private void HideCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
