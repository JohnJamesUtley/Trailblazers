using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherLeader : Leader
{
    bool knockingArrow = false;
    bool knocked = false;
    bool inRange = false;
    public Transform targetingReticle;
    float targetingSpeed = 0.3f;
    void Start() {
        targetingReticle = transform.GetChild(1);
        StartLeader();
    }
    public override void Move(Vector2 force) {
        if (!knocked) {
            base.Move(force);
        } else {
            targetingReticle.position += (Vector3)force.normalized * targetingSpeed;
            float dist = Vector3.Distance(targetingReticle.position, transform.position);
            if (!inRange) {
                if (dist > 3 && dist < 15) {
                    SetInRange(true);
                }
            } else {
                if (dist < 3 || dist > 15) {
                    SetInRange(false);
                }
            }
        }
    }
    void FixedUpdate() {
        FixedUpdateLeader();
    }
    public override void Ability() {
        if (!knocked) {
            if (abilityReady) {
                foreach (Archer x in men)
                    x.DrawBow();
                knocked = true;
                knockingArrow = true;
                EnableTargeting();
                abilityReady = false;
                StartCoroutine(Knock());
            }
        } else {
            if (!knockingArrow) {
                if (inRange) {
                    Vector3 offset = FinishTargeting();
                    foreach (Archer x in men)
                        x.Loose(offset + (Vector3)Random.insideUnitCircle);
                    knocked = false;
                } else {
                    CancelKnock();
                }
            }
        }
    }
    void SetInRange(bool inRange) {
        this.inRange = inRange;
        if (inRange) {
            targetingReticle.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("ArcherTarget");
        } else {
            targetingReticle.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("ArcherTargetGreyed");
        }
    }
    void EnableTargeting() {
        targetingReticle.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = true;
        SetInRange(false);
    }
    Vector2 FinishTargeting() {
        targetingReticle.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = false;
        Vector3 offset = targetingReticle.position - transform.position;
        targetingReticle.localPosition = Vector3.zero;
        StartCoroutine(LowerBows());
        return offset;
    }

    IEnumerator LowerBows() {
        yield return new WaitForSeconds(1.5f);
        abilityReady = true;
    }
    IEnumerator Knock() {
        yield return new WaitForSeconds(1f);
        knockingArrow = false;
    }
    void CancelKnock() {
        FinishTargeting();
        if (knocked) {
            foreach (Archer x in men)
                x.CancelKnock();
        }
        knocked = false;
    }
    public override void Teleport(Vector2 pos) {
        CancelKnock();
        base.Teleport(pos);
    }
    public override void AddTroop() {
        GameObject troop = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Archer"));
        troop.transform.position = (Vector2)transform.position + Random.insideUnitCircle;
        SpriteRenderer spriteRend = troop.transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteRend.color = sprite.color;
        spriteRend.sprite = player.TroopSprites[5];
        troop.GetComponent<Troop>().col = sprite.color;
        troop.GetComponent<Troop>().leader = this;
        men.Add(troop.GetComponent<Troop>());
    }
    public override void Kill() {
        CancelKnock();
        base.Kill();
    }

    public override void OnSelect() {
        if(knocked)
            transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = true;
    }
    public override void OnDeselect() {
        if (knocked)
            transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = false;
    }
}
