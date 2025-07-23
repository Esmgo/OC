using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : UIPanel
{
    public override void OnOpen()
    {
        RegisterButton("Continue", () => GameApplication.Instance.ResumeGame());
        RegisterButton("Quit", () => GameApplication.Instance.ExitGame());
    }
}
