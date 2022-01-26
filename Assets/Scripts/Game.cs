using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Game : MonoBehaviour
{
    public static Game game;
    public Player[] players;
    public CameraAdjuster cam;
    public List<Transform> mapMarkers;
    public List<GameObject> capPoints;
    public Text time;
    public Text phase;
    public Text[] deaths;
    public GameObject victoryBanner;
    public GameObject EndButtons;
    bool gameOver = false;
    List<GameObject> activeCapPoints;
    int vulnerablePoint = 0;
    bool currentDirectionRight = false;
    int areaPos = 2;
    float timeLeft = 0;
    readonly float timeForSkirmish = 60f;
    readonly float timeForAssault = 75f;
    readonly float timeForAdvance = 15f;
    string currentPhase = "Skirmish";
    int[][] rightAreas = new int[][] {
        new int []{0,1},
        new int []{1,3},
        new int []{3,5},
        new int []{5,7},
        new int []{7,9},
    };
    int[][] leftAreas = new int[][] {
        new int []{0,2},
        new int []{2,4},
        new int []{4,6},
        new int []{6,8},
        new int []{8,9},
    };
    public void PointTaken() {
        if (currentDirectionRight) {
            vulnerablePoint++;
            if (vulnerablePoint > 2) {
                Shift(true);
                return;
            }
        } else {
            vulnerablePoint--;
            if (vulnerablePoint < 0) {
                Shift(false);
                return;
            }
        }
        activeCapPoints[vulnerablePoint].GetComponent<CapturePoint>().SetVulnerable(true);
    }
    void SetPhase(string newPhase) {
        phase.text = newPhase;
        currentPhase = newPhase;
    }
    private void Start() {
        timeLeft = 1f;
        SetPhase("Set Up");
        OrganizeMarkers();
        OrganizeCapturePoints();
    }
    void OrganizeMarkers() {
        Transform[] transforms = transform.GetComponentsInChildren<Transform>();
        foreach(Transform x in transforms) {
            if(x != transform)
                mapMarkers.Add(x);
        }
        List<Transform> newMapMarkers = new List<Transform>();
        newMapMarkers.Add(mapMarkers[0]);
        for (int i = 1; i < mapMarkers.Count; i++) {
            bool added = false;
            for (int j = 0; j < newMapMarkers.Count; j++) {
                if (mapMarkers[i].position.x < newMapMarkers[j].position.x) {
                    newMapMarkers.Insert(j, mapMarkers[i]);
                    added = true;
                    break;
                }
            }
            if (!added)
                newMapMarkers.Add(mapMarkers[i]);
        }
        mapMarkers.Clear();
        mapMarkers = newMapMarkers;
    }
    void OrganizeCapturePoints() {
        GameObject[] points = GameObject.FindGameObjectsWithTag("CapturePoint");
        foreach (GameObject x in points) {
            capPoints.Add(x);
        }
        List<GameObject> newCaps = new List<GameObject>();
        newCaps.Add(capPoints[0]);
        for (int i = 1; i < capPoints.Count; i++) {
            bool added = false;
            for (int j = 0; j < newCaps.Count; j++) {
                if (capPoints[i].transform.position.x < newCaps[j].transform.position.x) {
                    newCaps.Insert(j, capPoints[i]);
                    added = true;
                    break;
                }
            }
            if (!added)
                newCaps.Add(capPoints[i]);
        }
        capPoints.Clear();
        capPoints = newCaps;
        /*
        for (int i = 0; i < 6; i++) {
            capPoints[i].GetComponent<CapturePoint>().SwitchPlayer(0);
        }
        */
        for (int i = 0; i < 15; i++) {
            capPoints[i].GetComponent<CapturePoint>().game = this;
        }
        for (int i = 0; i < 9; i++) {
            capPoints[i].GetComponent<CapturePoint>().SwitchPlayer(0);
        }
        for (int i = 9; i < 15; i++) {
            capPoints[i].GetComponent<CapturePoint>().SwitchPlayer(1);
        }
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            SetArea(3, 6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            SetArea(0, 9);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9)) {
            Shift(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            Shift(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7)) {
            cam.onLeaders = true;
        }
        timeLeft -= Time.deltaTime;
        if(timeLeft <= 0)
            TimeStop();
        SetClock();
    }
    void TimeStop() {
        switch (currentPhase) {
            case "Set Up":
                ToggleControls(true);
                timeLeft = timeForSkirmish;
                SetPhase("Skirmish");
                break;
            case "Skirmish":
                for (int i = 0; i < 15; i++) {
                    capPoints[i].GetComponent<Animator>().SetBool("Started", true);
                }
                if(players[0].deaths > players[1].deaths) {
                    Shift(true);
                } else {
                    Shift(false);
                }
                break;
            case "Assault":
                Shift(!currentDirectionRight);
                break;
            case "Advancing":
                cam.onLeaders = true;
                ToggleControls(true);
                timeLeft = timeForAssault;
                    SetPhase("Assault");
                break;
        }
    }
    void ToggleControls(bool controls) {
        foreach (Player x in players)
            x.controlsEnabled = controls;
    }
    void SetClock() {
        int seconds = (int)timeLeft % 60;
        int minutes = (int)timeLeft / 60;
        if (minutes >= 1) {
            if (seconds >= 10) {
                time.text = minutes + ":" + seconds;
            } else {
                time.text = minutes + ":0" + seconds;
            }
        } else {
            time.text = ((int)timeLeft).ToString();
        }
    }
    void ActivateArea(int area, bool right) {
        for (int i = area * 3; i < (area + 1)* 3; i++) {
            capPoints[i].GetComponent<CapturePoint>().active = true;
        }
        activeCapPoints = new List<GameObject>();
        for (int i = area * 3; i < area * 3 + 3; i++) {
            activeCapPoints.Add(capPoints[i]);
        }
        if (right) {
            vulnerablePoint = 0;
        } else {
            vulnerablePoint = 2;
        }
        activeCapPoints[vulnerablePoint].GetComponent<CapturePoint>().SetVulnerable(true);
        currentDirectionRight = right;
    }
    void DeactivateArea(int area, bool right) {
        for (int i = area * 3; i < (area + 1) * 3; i++) {
            capPoints[i].GetComponent<CapturePoint>().active = false;
            capPoints[i].GetComponent<CapturePoint>().SetVulnerable(false);
            if (right)
                capPoints[i].GetComponent<CapturePoint>().SwitchPlayer(0);
            else
                capPoints[i].GetComponent<CapturePoint>().SwitchPlayer(1);

        }
    }
    void Shift(bool right) {
        cam.onLeaders = false;
        ToggleControls(false);
        DeactivateArea(areaPos, right);
        IEnumerator advance = null;
        if (right) {
            if (areaPos != 4) {
                areaPos++;
                SetArea(rightAreas[areaPos][0], rightAreas[areaPos][1]);
                players[0].SpawnLeaders((Vector2)mapMarkers[rightAreas[areaPos][1]].position + new Vector2(-3, 7.5f));
                advance = Advancement(players[1], (Vector2)mapMarkers[rightAreas[areaPos][0]].position + new Vector2(-3, 7.5f), right);
            } else {
                victoryBanner.GetComponent<Animator>().SetBool("Red", true);
                victoryBanner.GetComponentInChildren<Text>().text = "Red Victory";
                gameOver = true;
            }
        }
        if (!right) {
            if (areaPos != 0) {
                areaPos--;
                SetArea(leftAreas[areaPos][0], leftAreas[areaPos][1]);
                players[1].SpawnLeaders((Vector2)mapMarkers[leftAreas[areaPos][0]].position + new Vector2(3, -7.5f));
                advance = Advancement(players[0], (Vector2)mapMarkers[leftAreas[areaPos][1]].position + new Vector2(3, -7.5f), right);
            } else {
                victoryBanner.GetComponent<Animator>().SetBool("Blue", true);
                victoryBanner.GetComponentInChildren<Text>().text = "Blue Victory";
                gameOver = true;
            }
        }
        if (!gameOver) {
            StartCoroutine(advance);
            ActivateArea(areaPos, right);
            timeLeft = timeForAdvance;
            SetPhase("Advancing");
        } else {
            SetPhase("Game Over");
            EndButtons.GetComponent<Animator>().SetBool("GameEnded", true);
        }
    }
     IEnumerator Advancement(Player advPlayer,Vector2 pos, bool right) {
        yield return new WaitForSeconds(6);
        advPlayer.AdvanceLeaders(pos, right);
    }
    void SetArea(int firstMark, int lastMark) {
        cam.onLeaders = false;
        float smX = mapMarkers[firstMark].position.x;
        float lgX = mapMarkers[firstMark].position.x;
        float smY = mapMarkers[firstMark].position.y;
        float lgY = mapMarkers[firstMark].position.y;
        for (int i = firstMark; i < lastMark + 1; i++) {
            if (mapMarkers[i].position.x > lgX) {
                lgX = mapMarkers[i].position.x;
            }
            if (mapMarkers[i].position.y > lgY) {
                lgY = mapMarkers[i].position.y;
            }
            if (mapMarkers[i].position.x < smX) {
                    smX = mapMarkers[i].position.x;
            }
            if (mapMarkers[i].position.y < smY) {
                    smY = mapMarkers[i].position.y;
            }
            cam.SetTarget(new Vector2(smX, smY), new Vector2(lgX, lgY), 0f);
        }
    }
}
