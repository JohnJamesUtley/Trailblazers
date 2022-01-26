using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeTroopBlood : MonoBehaviour
{
    bool correctlyOriented = false;
    // Start is called before the first frame update
    void Start() {
        Sprite[] sprites = Resources.LoadAll<Sprite>("UpdateTroopBlood");
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, 8)];
        transform.localScale = new Vector3(1, 1, 1);
    }
     void Update() {
        if (transform.localScale.x == -1 && !correctlyOriented) {
            transform.localScale = new Vector3(1, 1, 1);
            correctlyOriented = true;
        }
    }
}
