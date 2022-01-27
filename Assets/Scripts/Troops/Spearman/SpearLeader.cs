using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearLeader : Leader
{
    void Start() {
        StartLeader();
    }
    void FixedUpdate() {
        FixedUpdateLeader();
    }
    public override void Ability() {
        if (abilityReady) {
            abilityReady = false;
            StartCoroutine(ActivateSkirmish());
        }
    }
    IEnumerator ActivateSkirmish() {
        foreach (Troop x in men) {
            ((Spearman)x).ToggleSkirmish(true);
        }
        yield return new WaitForSeconds(3f);
        foreach (Troop x in men) {
            ((Spearman)x).ToggleSkirmish(false);
        }
        yield return new WaitForSeconds(1.5f);
        abilityReady = true;
    }
    public override void AddTroop() {
        GameObject troop = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Spearman"));
        troop.transform.position = (Vector2)transform.position + Random.insideUnitCircle;
        SpriteRenderer spriteRend = troop.transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteRend.color = sprite.color;
        spriteRend.sprite = player.TroopSprites[6];
        troop.GetComponent<Troop>().col = sprite.color;
        troop.GetComponent<Troop>().leader = this;
        men.Add(troop.GetComponent<Troop>());
    }
}
