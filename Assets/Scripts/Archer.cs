using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Troop
{
    bool knockingArrow = false;
    bool drawn = false;
    GameObject arrow;
    void Start()
    {
        StartTroop();
    }
    public override void Orientate(Vector2 force) {
        if (!drawn) {
            base.Orientate(force);
        } else {
            if (((ArcherLeader)leader).targetingReticle != null) {
                if (((ArcherLeader)leader).targetingReticle.position.x - transform.position.x < 0) {
                    if (!flipped)
                        Flip();
                } else {
                    if (flipped)
                        Flip();
                }
            }
        }
    }
    public void DrawBow() {
        drawn = true;
        speedModifier = 0;
        gameObject.GetComponent<Animator>().SetBool("Drawn", true);
        gameObject.GetComponent<Rigidbody2D>().mass = 5f;
        StartCoroutine(Knock());
    }
    IEnumerator Knock() {
        knockingArrow = true;
        arrow = GameObject.Instantiate(Resources.Load<GameObject>("Arrow"));
        arrow.transform.parent = gameObject.transform.GetChild(0);
        arrow.transform.localPosition = Vector3.zero;
        arrow.transform.localRotation = Quaternion.Euler(new Vector3(0,0,-90));
        yield return new WaitForSeconds(1f);
        knockingArrow = false;
    }
    public void Loose(Vector2 offset) {
        GameObject loosed = GameObject.Instantiate(Resources.Load<GameObject>("LoosedArrow"));
        loosed.transform.position = gameObject.transform.position;
        arrow.transform.parent = loosed.transform;
        arrow.GetComponent<Orderer>().parentPos = true;
        loosed.GetComponent<Arrow>().SetTarget((Vector2)transform.position + offset);
        StartCoroutine(LowerBow());
    }
    IEnumerator LowerBow() {
        yield return new WaitForSeconds(1f);
        drawn = false;
        speedModifier = 3;
        gameObject.GetComponent<Animator>().SetBool("Drawn", false);
        gameObject.GetComponent<Rigidbody2D>().mass = 1f;
    }
    public void CancelKnock() {
        drawn = false;
        knockingArrow = false;
        Destroy(arrow);
        speedModifier = 3;
        gameObject.GetComponent<Animator>().SetBool("Drawn", false);
        gameObject.GetComponent<Rigidbody2D>().mass = 1f;
    }
    public override GameObject SpawnDead() {
        GameObject dead = GameObject.Instantiate(Resources.Load<GameObject>("ArcherDead"));
        GameObject show = transform.GetChild(0).gameObject;
        dead.transform.position = show.transform.position;
        foreach (GameObject x in bodyBlood) {
            x.transform.parent = dead.transform;
            x.transform.localPosition = new Vector3(0.015f,0f,0f);
        }
        dead.transform.rotation = Quaternion.Euler(0, 0, GetNegPos() * 90);
        dead.transform.localScale = new Vector3(GetNegPos() * 1, 1, 1);
        dead.transform.GetComponent<Orderer>().Staticize();
        dead.transform.GetChild(0).GetComponent<Orderer>().Staticize();
        return dead;
    }
    void Update()
    {
        UpdateTroop();
        if (knockingArrow) {
            arrow.transform.localPosition = Vector2.Lerp(arrow.transform.localPosition, transform.GetChild(0).GetChild(0).localPosition, 0.05f);
            Vector3 euler = Vector3.Lerp(arrow.transform.localRotation.eulerAngles + new Vector3(0, 0, 360), transform.GetChild(0).GetChild(0).localRotation.eulerAngles + new Vector3(0, 0, 630), 0.15f);
            arrow.transform.localRotation = Quaternion.Euler(euler);
        }
    }
    private void FixedUpdate() {
        FixedUpdateTroop();
    }
}
