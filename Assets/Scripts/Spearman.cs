using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spearman : Troop
{
    float bloodyChance;
    float fightPerSec = 5;
    float timeFight = 0;
    bool skirmish = false;
    float bonusDamage = 1f;
    void Start()
    {
        StartTroop();
        bloodyChance = 1 - Mathf.Pow(0.8f, 1 / (fightPerSec));
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTroop();
    }

    private void FixedUpdate() {
        FixedUpdateTroop();
        if (timeFight > 1 / fightPerSec) {
            Fight();
            timeFight = 0;
        }
        timeFight += Time.fixedDeltaTime;
    }
    public void ToggleSkirmish(bool state) {
        skirmish = state;
        if (skirmish) {
            speedModifier = 0;
            bonusDamage = 1f;
        } else {
            speedModifier = 4f;
            bonusDamage = 1.5f;
        }
    }
    public override GameObject SpawnDead() {
        GameObject dead = GameObject.Instantiate(Resources.Load<GameObject>("SpearmanDead"));
        GameObject show = transform.GetChild(0).gameObject;
        dead.transform.position = show.transform.position;
        dead.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite;
        foreach (GameObject x in bodyBlood) {
            x.transform.parent = dead.transform;
        }
        dead.transform.rotation = Quaternion.Euler(0, 0, GetNegPos() * 90);
        dead.transform.localScale = new Vector3(GetNegPos() * 1, 1, 1);
        dead.transform.GetComponent<Orderer>().Staticize();
        dead.transform.GetChild(0).GetComponent<Orderer>().Staticize();
        return dead;
    }

    void Fight() {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, 1f, LayerMask.GetMask("Troops"));
        List<Transform> friends = new List<Transform>();
        List<Transform> enemies = new List<Transform>();
        for (int i = 0; i < nearby.Length; i++) {
            if (!nearby[i].transform.Equals(transform)) {
                if (nearby[i].transform.GetComponent<Troop>().playerID == playerID) {
                    friends.Add(nearby[i].transform);
                } else {
                    enemies.Add(nearby[i].transform);
                }
            }
        }
        foreach (Transform f in friends) {
            if (debug)
                Debug.DrawLine(f.position, transform.position, col);
            foreach (Transform e in enemies) {
                if (InBox(transform.position, f.position, e.position)) {
                    if (!e.GetComponent<Troop>().immune) {
                        if (Random.value < bloodyChance) {
                            e.GetComponent<Troop>().Bloody(1);
                            Bloody(1);
                        }
                        e.GetComponent<Troop>().health -= ((1 / fightPerSec) / 5) * bonusDamage;
                    }
                }
            }
        }
        if (enemies.Count > 0) {
            GetComponent<Animator>().SetBool("Fighting", true);
            GetComponent<Animator>().SetFloat("RandomFighting", Random.value);
        } else {
            GetComponent<Animator>().SetBool("Fighting", false);
        }
    }
    bool InBox(Vector2 corner1, Vector2 corner2, Vector2 point) {
        if (InBetween(corner1.x, corner2.x, point.x))
            if (InBetween(corner1.y, corner2.y, point.y))
                return true;
        return false;
    }
    bool InBetween(float pos1, float pos2, float point) {
        float max = Mathf.Max(pos1, pos2);
        float min = Mathf.Min(pos1, pos2);
        if (point >= min && point <= max) {
            return true;
        }
        return false;
    }
}
