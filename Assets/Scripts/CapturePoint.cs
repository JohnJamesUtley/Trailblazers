using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePoint : MonoBehaviour
{
    public int player;
    public bool active = false;
    public bool vulnerable = false;
    public Game game;
    float TimeTillCheck = 0;
    Animator anim;
    private void Start() {
        anim = gameObject.GetComponent<Animator>();
    }
    public void SetVulnerable(bool state) {
        vulnerable = state;
        if(state)
            anim.SetBool("Wavering", true);
        else
            anim.SetBool("Wavering", false);
        TimeTillCheck = 0.4f;
    }
    public void SwitchPlayer(int playerId) {
        SetVulnerable(false);
        player = playerId;
        if (player == 0) {
            anim.SetBool("Red", true);
        } else {
            anim.SetBool("Red", false);
        }
    }
    private void Update() {
        if (vulnerable) {
            if (TimeTillCheck <= 0) {
                TimeTillCheck = 0.4f;
                Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, 1f, LayerMask.GetMask("Troops"));
                int friends = 0;
                int enemies = 0;
                for (int i = 0; i < nearby.Length; i++) {
                    if (nearby[i].gameObject.GetComponent<Troop>().playerID == player) {
                        friends++;
                    } else if (nearby[i].gameObject.GetComponent<Troop>().playerID != player) {
                        enemies++;
                    }
                }
                if(enemies * 0.2f > friends) {
                    if (player == 0) {
                        SwitchPlayer(1);
                    } else {
                        SwitchPlayer(0);
                    }
                    game.PointTaken();
                }
            }
            TimeTillCheck -= Time.deltaTime;
        }
    }
}
