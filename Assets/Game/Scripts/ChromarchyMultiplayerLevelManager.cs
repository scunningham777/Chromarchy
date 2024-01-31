using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

public class ChromarchyMultiplayerLevelManager : MultiplayerLevelManager, MMEventListener<MMGameEvent>
{
    public struct ChromarchyPoints
    {
        public string PlayerID;
        public int Points;
    }
    [Header("Chromarchy Bindings")]
    /// An array to store each player's points
    [Tooltip("an array to store each player's points")]
    public ChromarchyPoints[] Points;
    /// the list of countdowns we need to update
    [Tooltip("the list of countdowns we need to update")]
    public List<MMCountdown> Countdowns;
    [Tooltip("the container of all ChromaBlock objects")]
    public GameObject ChromaBlockContainer;
    [Header("Chromarchy Settings")]
    /// the duration of the game, in seconds
    [Tooltip("the duration of the game, in seconds")]
    public int GameDuration = 99;
    public string WinnerID { get; set; }
    protected bool _gameOver = false;

    public virtual void Update()
    {
        UpdateCountdown();
        CheckForGameRestart();
    }

    protected override void Initialization()
    {
        base.Initialization();
        WinnerID = "";
        Points = new ChromarchyPoints[Players.Count];
        int i = 0;
        foreach (Character player in Players)
        {
            Points[i].PlayerID = player.PlayerID;
            Points[i].Points = 0;
            i++;
        }
        foreach(MMCountdown countdown in Countdowns)
        {
            countdown.CountdownFrom = GameDuration;
            countdown.ResetCountdown();
        }
    }

    public virtual void OnMMEvent(MMGameEvent gameEvent)
    {
        switch (gameEvent.EventName)
        {
            case "ChromaBlockColorChange":
                // check for instant victory conditions
                break;
        }
    }

    protected virtual void UpdateCountdown()
    {
        if (_gameOver)
        {
            return;
        }

        float remainingTime = GameDuration;
        foreach (MMCountdown countdown in Countdowns)
        {
            if (countdown.gameObject.activeInHierarchy)
            {
                remainingTime = countdown.CurrentTime;
            }
        }
        if (remainingTime <= 0f)
        {
            RecalculatePoints();
            StartCoroutine(GameOver());
        }
    }

    protected virtual IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2f);
        DetermineWinner();
        
        MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 0f, 0f, false, 0f, true);
        _gameOver = true;
        MMSoundManagerAllSoundsControlEvent.Trigger(MMSoundManagerAllSoundsControlEventTypes.FreeAllLooping);
        TopDownEngineEvent.Trigger(TopDownEngineEventTypes.GameOver, null);
    }

    private void RecalculatePoints()
    {
        for (int i = 0; i < Points.Length; i++)
        {
            ChromarchyPoints points = Points[i];
            foreach (ChromaBlock block in ChromaBlockContainer.GetComponentsInChildren<ChromaBlock>())
            {
                if (points.PlayerID == block.GetOwnerID())
                {
                    points.Points += 1;
                    Points[i] = points;
                }
            }
        }
    }

    private void DetermineWinner()
    {
        int maxPoints = 0;
        foreach (ChromarchyPoints points in Points)
        {
            if (points.Points > maxPoints)
            {
                maxPoints = points.Points;
                WinnerID = points.PlayerID;
            }
        }

        if (WinnerID == "")
        {
            WinnerID = "Player1";
        }
    }

    protected virtual void CheckForGameRestart()
		{
			if (_gameOver)
			{
				if ( (Input.GetButton("Player1_Jump"))
				     || (Input.GetButton("Player2_Jump"))
				     || (Input.GetButton("Player3_Jump"))
				     || (Input.GetButton("Player4_Jump")) )
				{
					MMTimeScaleEvent.Trigger(MMTimeScaleMethods.Reset, 1f, 0f, false, 0f, true);
					MMSceneLoadingManager.LoadScene(SceneManager.GetActiveScene().name);
				}
			}
		}

    protected override void OnEnable()
    {
        base.OnEnable();
        this.MMEventStartListening<MMGameEvent>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.MMEventStopListening<MMGameEvent>();
    }
}
