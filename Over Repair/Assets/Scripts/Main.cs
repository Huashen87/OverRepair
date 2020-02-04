using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static int active = 0;
    private bool gotoPlay = false;

    public GameState gameState = GameState.Init;
    public enum GameState
    {
        Init,
        Title,
        GamePrepare,
        Gaming,
        GameRoundClear,
        GameFail
    }
    public int gameRound = 0;
    private Vector3 defaultPlayerPosition;

    [Header("Game Managers")]
    public ItemManager itemManager;
    public PlayerControl playerControl;
    public RobotDown robotDown;
    public ConveryorMover converyorMover;
    public AudioManager audioManager;
    public GameObject joystick;
    public Timer timer;
    public bool isTouch;

    public float roundClearTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        playerControl.isPlaying = false;
        defaultPlayerPosition = playerControl.transform.position;
        robotDown.transform.position = new Vector2(0, 0);

        timer.Reset();

        CameraEffect.Instance.MoveTo(new Vector2(0, 0), 0f);
        CameraEffect.Instance.oncolorfinish = ReadyToTitle;
        CameraEffect.Instance.FadeIn(1f, 1f);
    }

    private void ReadyToTitle()
    {
        gameState = GameState.Title;
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.Title:
                {
                    if (Input.anyKey && !gotoPlay)
                    {
                        itemManager.enableSpawn = false;
                        itemManager.RemoveAll();

                        gameRound = 0;

                        timer.Reset();

                        playerControl.isPlaying = false;
                        playerControl.Init(new bool[] { true, true, true, true, true, true });
                        robotDown.transform.position = new Vector2(0, 0);
                        playerControl.transform.position = defaultPlayerPosition;

                        robotDown.Init(new bool[] { true, true, true, true, true, false });

                        gotoPlay = true;
                        CameraEffect.Instance.onposfinish = ReadyToGame;
                        CameraEffect.Instance.MoveTo(new Vector2(0, 0), 1f);
                    }
                    break;
                }
            case GameState.GamePrepare:
                {
                    gameState = GameState.Gaming;
                    break;
                }
            case GameState.Gaming:
                {
                    if (Input.touchCount > 0)
                    {
                        if (!isTouch)
                        {
                            joystick.SetActive(true);
                            joystick.transform.position = Input.GetTouch(0).position;
                            isTouch = true;
                            joystick.GetComponent<Joystick>().isDragging = true;
                        }
                    }
                    if (Input.touchCount == 0)
                    {
                        joystick.GetComponent<Joystick>().isDragging = false;
                        joystick.SetActive(false);
                        isTouch = false;
                    }
                    GamingLogic();
                    break;
                }
            case GameState.GameRoundClear:
                {
                    joystick.GetComponent<Joystick>().isDragging = false;
                    joystick.SetActive(false);
                    isTouch = false;

                    if (roundClearTimer > 0)
                    {
                        roundClearTimer -= Time.deltaTime;
                        if (roundClearTimer <= 0)
                        {
                            CameraEffect.Instance.oncolorfinish = NextRoundPrepare;
                            CameraEffect.Instance.FadeOut(0.5f, 2.5f);
                            Debug.Log("Fade Out");
                            audioManager.Play("transition");
                        }
                    }
                    break;
                }
            case GameState.GameFail:
                {
                    joystick.GetComponent<Joystick>().isDragging = false;
                    joystick.SetActive(false);
                    isTouch = false;

                    if (roundClearTimer > 0)
                    {
                        roundClearTimer -= Time.deltaTime;
                        if (roundClearTimer <= 0)
                        {
                            CameraEffect.Instance.onposfinish = BackToTitle;
                            CameraEffect.Instance.MoveTo(new Vector2(0, -7.68f), 1f);
                        }
                    }
                    break;
                }
            default:
                break;
        }
    }

    private void BackToTitle()
    {
        playerControl.isBlind = false;
        playerControl.CheckBlind();
        gameState = GameState.Title;
    }

    private void NextRoundPrepare()
    {
        gameRound++;

        Vector3 tmpPosition = playerControl.transform.position;
        tmpPosition.x = Mathf.Clamp(tmpPosition.x, -2.27f, 2.13f);
        tmpPosition.y = Mathf.Clamp(tmpPosition.y, -1.55f, -1.56f);
        playerControl.transform.position = tmpPosition;

        tmpPosition = robotDown.transform.position;
        robotDown.transform.position = playerControl.transform.position;
        playerControl.transform.position = tmpPosition;

        playerControl.Remain();

        bool[] tmpState = robotDown.GetStateData();
        robotDown.Init(playerControl.MyStates);
        playerControl.Init(tmpState);
        playerControl.CheckBlind();

        timer.Reset();
        itemManager.enableSpawn = true;

        converyorMover.speed = 2;

        Debug.Log(gameRound + " Round");

        CameraEffect.Instance.oncolorfinish = GoNextRound;
        CameraEffect.Instance.FadeIn(0.5f, 2.5f);
    }

    private void GoNextRound()
    {
        playerControl.isPlaying = true;
        
        timer.StartTimer();

        gameState = GameState.Gaming;
    }

    private void GamingLogic()
    {
        // Check Is All Fix
        if (robotDown.IsAllFix)
        {
            playerControl.isPlaying = false;
            timer.StopTimer();
            roundClearTimer = 1f;
            gameState = GameState.GameRoundClear;
            
            return;
        }

        // Check Timesup
        if (!timer.active)
        {
            playerControl.isPlaying = false;

            if (robotDown.IsCanPlayNextRound)
            {
                roundClearTimer = 1f;
                gameState = GameState.GameRoundClear;
            }
            else
            {
                roundClearTimer = 1f;
                gameState = GameState.GameFail;
            }
            return;
        }
    }

    private void ReadyToGame()
    {
        gameState = GameState.GamePrepare;
        gameRound++;

        playerControl.isPlaying = true;
        itemManager.enableSpawn = true;

        timer.Reset();
        timer.StartTimer();

        converyorMover.speed = 2;

        Debug.Log("First Round");
        gotoPlay = false;
    }
}
