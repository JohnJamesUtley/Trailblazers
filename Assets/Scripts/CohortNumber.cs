using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CohortNumber : MonoBehaviour {
    int cohortNum;
    SpriteRenderer sprRend;
    Sprite[] sprites;
    bool setUp = false;
    bool dead = false;
    public void SetCohortNumber(int num) {
        if (!setUp) {
            sprRend = GetComponent<SpriteRenderer>();
            sprites = Resources.LoadAll<Sprite>("CohortNumbers");
            setUp = true;
        }
        cohortNum = num;
        switch (num) {
            case 1:
                sprRend.sprite = sprites[0];
                break;
            case 2:
                sprRend.sprite = sprites[2];
                break;
            case 3:
                sprRend.sprite = sprites[4];
                break;
        }
    }
    public void SelectCohort(int num) {
        if (!dead) {
            if (num == cohortNum) {
                switch (num) {
                    case 1:
                        sprRend.sprite = sprites[1];
                        break;
                    case 2:
                        sprRend.sprite = sprites[3];
                        break;
                    case 3:
                        sprRend.sprite = sprites[5];
                        break;
                }
            } else {
                SetCohortNumber(cohortNum);
            }
        }
    }
    public void KillCohortNumber() {
        dead = true;
        switch (cohortNum) {
            case 1:
                sprRend.sprite = sprites[0];
                break;
            case 2:
                sprRend.sprite = sprites[2];
                break;
            case 3:
                sprRend.sprite = sprites[4];
                break;
        }
    }
}
