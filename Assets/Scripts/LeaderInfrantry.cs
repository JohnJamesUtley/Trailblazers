using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderInfrantry : Leader
{
    void Start()
    {
        StartLeader();
    }
    void FixedUpdate() {
        FixedUpdateLeader();
    }
    public override void Ability() {
        StartCoroutine(RaiseShields());
    }
    IEnumerator RaiseShields() {
        foreach (Infrantry x in men)
            x.SetShield(true);
        yield return new WaitForSeconds(2.5f);
        foreach (Infrantry x in men)
            x.SetShield(false);
    }
    public override void AddTroop() {
        GameObject troop = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Infrantry"));
        troop.transform.position = (Vector2)transform.position + Random.insideUnitCircle;
        SpriteRenderer spriteRend = troop.transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteRend.color = sprite.color;
        spriteRend.sprite = player.TroopSprites[0];
        spriteRend.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = player.TroopSprites[1];
        troop.GetComponent<Troop>().col = sprite.color;
        troop.GetComponent<Troop>().leader = this;
        men.Add(troop.GetComponent<Troop>());
    }
}

