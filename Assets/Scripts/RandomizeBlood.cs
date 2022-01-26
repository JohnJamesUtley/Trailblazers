using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeBlood : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        Sprite[] sprites = Resources.LoadAll<Sprite>("Blood");
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, 16)];
    }
}
