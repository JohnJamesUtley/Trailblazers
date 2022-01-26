using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
public class Leader : MonoBehaviour {
    Rigidbody2D rigid;
    public SpriteRenderer sprite;
    public Player player;
    public float SpeedModifier;
    public int cohortSize;
    public int playerID;
    bool flipped;
    public int cohortNum;
    public List<Troop> men;
    public Vector2 targetPos;
    public bool advancing = false;
    public bool abilityReady = true;
    public virtual void Move(Vector2 force) {
        if (force.x < 0) {
            if (!flipped)
                Flip();
        } else if (force.x > 0) {
            if (flipped)
                Flip();
        }
        rigid.AddForce(force * SpeedModifier, ForceMode2D.Force);
    }
    public virtual void Ability() {}
    void Flip() {
        if (!flipped) {
            transform.localScale = new Vector3(-1, 1, 1);
        } else {
            transform.localScale = new Vector3(1, 1, 1);
        }
        flipped = !flipped;
    }
    public virtual void Teleport(Vector2 pos) {
        transform.position = pos;
        foreach(Troop x in men) {
            x.transform.position = pos + Random.insideUnitCircle;
        }
    }
    public void StartLeader() {
        player.leaders.Add(this);
        playerID = player.playerID;
        CohortNumber flagNum = transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<CohortNumber>();
        player.cohortNums.Add(flagNum);
        cohortNum = player.leaders.Count;
        flagNum.SetCohortNumber(cohortNum);
        flagNum.SelectCohort(player.selectedCohort);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraAdjuster>().leaders.Add(gameObject);

        FindComponents();
        sprite.sprite = player.TroopSprites[2];
        transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = player.TroopSprites[3];
        for (int i = 0; i < cohortSize; i++)
            AddTroop();
    }
    public virtual void AddTroop() {}
    void FindComponents() {
        rigid = GetComponent<Rigidbody2D>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    public virtual void Kill() {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraAdjuster>().leaders.Remove(gameObject);
        GameObject.Destroy(gameObject);
    }
    public void FixedUpdateLeader() {
        if (advancing) {
            Vector2 force = (targetPos - (Vector2)transform.position).normalized;
            if (force.x < 0) {
                if (!flipped)
                    Flip();
            } else if (force.x > 0) {
                if (flipped)
                    Flip();
            }
            rigid.AddForce(force * SpeedModifier * 4f, ForceMode2D.Force);
            if (Vector2.Distance(targetPos, (Vector2)transform.position) < 0.5f)
                advancing = false;
        }
    }
    public virtual void OnSelect() {}
    public virtual void OnDeselect() {}
}
