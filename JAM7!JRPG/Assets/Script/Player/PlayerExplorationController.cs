﻿using UnityEngine;

public enum PlayerExplorationState
{
    Default = 0,
    Exploring = 1,
    InMenu = 2,
    InCombat = 3,
}

public class PlayerExplorationController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float walkSpeed = 1;

    public int PlayerID { get; private set; }

    private PlayerExplorationState currentState;

    public GameObject cam;

    public PlayerExplorationState CurrentState
    {
        // this allowed to triggger codes when the state switched
        get
        {
            return currentState;
        }

        private set
        {
            if (value == currentState)
            {
                // nothing
            }
            else
            {
                switch (currentState)
                {
                    case PlayerExplorationState.InMenu:
                        HUD.Singleton.HideMenu(PlayerID);
                        break;


                    case PlayerExplorationState.InCombat:
                        HUD.Singleton.ShowExplorationUI(PlayerID);
                        HUD.Singleton.HideCombatUI(PlayerID);
                        gameObject.SetActive(true);
                        cam.GetComponent<ForwardCamera>().enabled = false;
                        cam.GetComponent<OverworldCamera>().enabled = true;
                        break;
                }

                PlayerExplorationState previousState = currentState;
                currentState = value;

                Debug.Log(LogUtility.MakeLogStringFormat("PlayerExplorationController", "{0} --> {1}", previousState, currentState));

                switch (currentState)
                {
                    case PlayerExplorationState.InMenu:
                        HUD.Singleton.ShowMenu(PlayerID);
                        break;


                    case PlayerExplorationState.InCombat:
                        HUD.Singleton.HideExplorationUI(PlayerID);
                        HUD.Singleton.ShowCombatUI(PlayerID);
                        gameObject.SetActive(false);
                        cam.GetComponent<ForwardCamera>().enabled = true;
                        cam.GetComponent<OverworldCamera>().enabled = false;
                        break;
                }
            }
        }
    }

    private PlayerExplorationController() { }

    public void Initialize(int id)
    {
        PlayerID = id;
    }

    public void StartCombat(EnemyProxy enemy, bool isBoss = false)
    {
        CurrentState = PlayerExplorationState.InCombat;

        enemy.StartCombat(this, isBoss);
    }

    public void EndCombat()
    {
        CurrentState = PlayerExplorationState.Exploring;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;
        switch (go.tag)
        {
            case "Enemy":
                StartCombat(go.GetComponent<EnemyProxy>());
                break;
            case "Boss":
                StartCombat(go.GetComponent<EnemyProxy>(), true);
                break;
            case "MusicCollider":
                if (PlayerID == 1) break;
                string[] splitName = go.name.Split('_');
                MusicManager.Instance.PlayMusic(splitName[1]);
                GameObject another = go.GetComponent<mutual>().another;
                go.SetActive(false);
                another.SetActive(true);
                break;
        }
    }

    private void Start()
    {
        CurrentState = PlayerExplorationState.Exploring;
    }

    public void GetCamera()
    {
        var cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        foreach (var camera in cameras)
        {
            if (camera.GetComponent<OverworldCamera>().index == PlayerID)
            {
                cam = camera;
            }
        }
    }

    private void Update()
    {
        if (cam == null)
        {
            GetCamera();
            return;
        }
        switch (currentState)
        {
            case PlayerExplorationState.Exploring:
                if (Input.GetButtonDown("Start" + PlayerID))
                    CurrentState = PlayerExplorationState.InMenu;
                else
                    transform.Translate(Time.deltaTime * walkSpeed * new Vector3(Input.GetAxis("Horizontal" + PlayerID), Input.GetAxis("Vertical" + PlayerID), 0));
                break;
        }
    }
}
