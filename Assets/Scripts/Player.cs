using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public MovementTypes controlType;
    public float moveForce;
    public List<Leader> leaders;
    public List<CohortNumber> cohortNums;
    public int playerID;
    public Sprite[] TroopSprites;
    public int selectedCohort = 1;
    public int deaths = 0;
    public bool controlsEnabled = false;
    Game gme;
    int SelectedLeader = 0;
    KeyCode[] controls;
    public GameObject[] UIBanners;
    public void AddDeath() {
        deaths++;
        if(playerID == 1)
            gme.deaths[0].text = "Kills\n" + deaths;
        else
            gme.deaths[1].text = "Kills\n" + deaths;
    }
    void Start() {
        gme = GameObject.Find("Game").GetComponent<Game>();
        SetControls();
        UIBanners[0].GetComponent<Animator>().SetBool("Opened", true);
        UIBanners[0].GetComponent<UIBanner>().Select();
    }
    void SetControls() {
        switch (controlType) {
            case MovementTypes.WASD:
                controls = new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.LeftShift, KeyCode.Tab};
                break;
            case MovementTypes.Arrows:
                controls = new KeyCode[] { KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.RightControl, KeyCode.RightShift};
                break;
        }
    }
    void FixedUpdate() {
        if(controlsEnabled)
            Movement();
    }
    void Movement() {
        if (Input.GetKey(controls[0])) {
            leaders[SelectedLeader].Move(Vector2.up * moveForce);
        }
        if (Input.GetKey(controls[2])) {
            leaders[SelectedLeader].Move(Vector2.down * moveForce);
        }
        if (Input.GetKey(controls[1])) {
            leaders[SelectedLeader].Move(Vector2.left * moveForce);
        }
        if (Input.GetKey(controls[3])) {
            leaders[SelectedLeader].Move(Vector2.right * moveForce);
        }
    }
    private void Update() {
         SwitchLeaders();
        if (controlsEnabled)
            UseAbility();
    }
    void UseAbility() {
        if (Input.GetKeyDown(controls[5])) {
            leaders[SelectedLeader].Ability();
        }
    }
    void SwitchLeaders() {
        if (Input.GetKeyDown(controls[4])) {
            leaders[SelectedLeader].OnDeselect();
            SelectedLeader++;
            if (SelectedLeader == leaders.Count)
                SelectedLeader = 0;
            SelectLeader();
        }
    }
    void SelectLeader() {
        selectedCohort = SelectedLeader + 1;
        foreach (CohortNumber x in cohortNums) {
            x.SelectCohort(selectedCohort);
        }
        for (int i = 0; i < UIBanners.Length; i++) {
            if (i == SelectedLeader) {
                UIBanners[i].GetComponent<Animator>().SetBool("Opened", true);
                UIBanners[i].GetComponent<UIBanner>().Select();
            } else {
                UIBanners[i].GetComponent<Animator>().SetBool("Opened", false);
                UIBanners[i].GetComponent<UIBanner>().Deselect();

            }
        }
        if(leaders.Count > SelectedLeader)
            leaders[SelectedLeader].OnSelect();
    }
    public void SpawnLeaders(Vector2 pos) {
        SelectedLeader = 0;
        foreach (Leader x in leaders)
            x.Kill();
        leaders.Clear();
        cohortNums.Clear();
        for (int i = 0; i < 3; i++) {
            float rand = Random.value;
            GameObject lead;
            if (rand < 0.33f) {
                lead = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("InfrantryLeader"));
            } else if(rand >= 0.33f && rand < 0.67f) {
                lead = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("ArcherLeader"));
            } else {
                lead = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("InfrantryLeader"));
            }
            lead.transform.position = new Vector2(pos.x, pos.y + ((i - 1) * 4));
            lead.GetComponent<Leader>().player = this;
            lead.GetComponent<Leader>().playerID = playerID;
        }
        SelectLeader();
    }
    public void AdvanceLeaders(Vector2 pos, bool right) {
        SelectedLeader = 0;
        SelectLeader();
        for (int i = 0; i < 3; i++) {
            leaders[i].Teleport(new Vector2(pos.x, pos.y + ((i - 1) * 4)));
        }
        float push;
        if (right)
            push = 6;
        else
            push = -6;
        for (int i = 0; i < 3; i++) {
            leaders[i].targetPos = new Vector2(pos.x + push, pos.y + ((i - 1) * 4));
            leaders[i].advancing = true;
        }
    }
}
