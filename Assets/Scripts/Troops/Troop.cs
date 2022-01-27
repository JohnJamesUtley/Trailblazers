using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
public class Troop : MonoBehaviour {
    SpriteRenderer spriteRend;
    Rigidbody2D rigid;
    public Leader leader;
    public float speedModifier;
    public float health = 1f;
    public int playerID;
    public bool debug;
    public List<GameObject> bodyBlood;
    public bool flipped;
    public Color col;
    GameObject flagBear;
    public bool immune = true;
    float life = 0;
    bool flee = false;
    Vector2 fleePos;
    public void Move(Vector2 force) {
        Orientate(force);
        rigid.AddForce(force * speedModifier, ForceMode2D.Force);
    }
    public void Bloody(int splats) {
        for (int i = 0; i < splats; i++) {
            if (bodyBlood.Count >= 3) {
                break;
            }
            GameObject blood = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("TroopBlood"));
            blood.transform.position = (Vector2)transform.GetChild(0).position;
            blood.transform.parent = transform.GetChild(0);
            bodyBlood.Add(blood);
        }
    }
    public virtual void Orientate(Vector2 force) {
        if (force.x < 0) {
            if (!flipped)
                Flip();
        } else {
            if (flipped)
                Flip();
        }
    }
    public void Flip() {
        if (!flipped) {
            transform.localScale = new Vector3(-1, 1, 1);
        } else {
            transform.localScale = new Vector3(1, 1, 1);
        }
        flipped = !flipped;
    }
    public void Kill() {
        leader.men.Remove(this);
        leader.player.AddDeath();
        leader.cohortSize--;
        CreateDeadBody();
        SplatterBlood();
        GameObject.Destroy(gameObject);
    }
    void SplatterBlood() {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.GetMask("Troops"));
        for (int i = 0; i < nearby.Length; i++) {
            if (!nearby[i].transform.Equals(transform) && Random.value < 0.05f) {
                nearby[i].gameObject.GetComponent<Troop>().Bloody(1);
            }
        }
        if (Random.value < 0.35f) {
            GameObject blood = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Blood"));
            blood.transform.position = (Vector2)transform.GetChild(0).position + Random.insideUnitCircle;
        }
        if (Random.value < 0.4f) {
            GameObject parts = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("BloodParticles"));
            parts.transform.position = transform.position;
        }
    }
    public void CreateDeadBody() {
        Bloody(Random.Range(0, 2));
        GameObject dead = SpawnDead();
        dead.GetComponent<SpriteRenderer>().sprite = spriteRend.sprite;
        foreach (GameObject x in bodyBlood) {
            x.GetComponent<Orderer>().adjustment = -11;
            x.GetComponent<Orderer>().Staticize();
        }
    }
    public virtual GameObject SpawnDead() {
        return null;
    }
    public float GetNegPos() {
       float rand = (Random.value * 2) - 1;
        if (rand < 0)
            return -1f;
        return 1f;
    }
    public void StartTroop() {
        playerID = leader.player.playerID;
        FindComponents();
        SpawnFlag();
    }
    void SpawnFlag() {
        if(Random.value > 0.7) {
            GameObject flag = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Flag"));
            flag.GetComponent<FlagCarry>().carrier = transform.GetChild(0).gameObject;
            flag.GetComponent<SpriteRenderer>().sprite = leader.player.TroopSprites[4];
            CohortNumber coNum = flag.transform.GetChild(0).GetComponent<CohortNumber>();
            coNum.SetCohortNumber(leader.cohortNum);
            coNum.SelectCohort(leader.player.selectedCohort);
            leader.player.cohortNums.Add(coNum);
            flagBear = flag;
        }
    }
    void FindComponents() {
        rigid = GetComponent<Rigidbody2D>();
        spriteRend = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    public void FixedUpdateTroop()
    {
        CheckHealth();
        Follow();
    }
    void CheckHealth() {
        if (health < 1) {
            health += Time.fixedDeltaTime / 8;
            if (health > 1)
                health = 1;
        }
        if (health < 0)
            Kill();
    }
    void Follow() {
        if (leader != null)
            Move(Vector2.ClampMagnitude(leader.transform.position - transform.position, 1));
        else {
            if (!flee) {
                fleePos = new Vector2(transform.position.x + (12 + 16 * Random.value) * PlayerFleeDir(), (10 + Random.value * 15) * NegPos());
                gameObject.layer = 10;
                flee = true;
            }
            Move(Vector2.ClampMagnitude(fleePos - (Vector2)transform.position, 1));
        }
        if(Mathf.Abs(transform.position.y) > 9) {
            GameObject.Destroy(flagBear);
            GameObject.Destroy(gameObject);
        }
    }
    int NegPos() {
        if(Random.value > 0.5f) {
            return -1;
        } else {
            return 1;
        }
    }
    int PlayerFleeDir() {
        if (playerID == 0) {
            return -1;
        } else {
            return 1;
        }
    }
    public void UpdateTroop() {
        if (Input.GetKeyDown(KeyCode.Alpha8))
            Kill();
        life += Time.deltaTime;
        if (immune && life > 0.5f)
            immune = false;
    }
}
