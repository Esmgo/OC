using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏状态管理器
/// </summary>
public class GameStateManager : MonoBehaviour
{
    #region 单例
    public static GameStateManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    [Tooltip("是否在战斗中")]
    private bool isFighting = false;
    public bool IsFighting => isFighting;
    [Tooltip("游戏是否处于暂停")]
    private bool isPaused = false;
    public bool IsPaused => isPaused;

    /// <summary>
    /// 进入游戏战斗状态
    /// </summary>
    public void StartFight()
    {
        isFighting = true;
    }

    public async void PauseGame()
    {
        if (isFighting && !isPaused)
        {
            isPaused = true;
            Time.timeScale = 0f;
            await UIManager.Instance.OpenPanelAsync<PausePanel>("PausePanel");
        }
    }

    public void ResumeGame()
    {
        if (isFighting && isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;
            UIManager.Instance.ClosePanel("PausePanel");
        }
    }

    public void ExitGame()
    {
        isFighting = false;
        isPaused = false;
        Time.timeScale = 1f;
    }
}
