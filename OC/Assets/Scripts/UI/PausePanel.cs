using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : UIPanel
{
    public override void OnOpen()
    {
        RegisterButton("Continue", () => GameStateManager.Instance.ResumeGame());
        RegisterButton("Quit", () => GameStateManager.Instance.ExitGame());
    }
}
