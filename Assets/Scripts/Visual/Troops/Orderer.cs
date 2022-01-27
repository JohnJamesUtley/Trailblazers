using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orderer : MonoBehaviour
{
    SpriteRenderer sprite;
    public bool foreground;
    public bool StartStaticized;
    public bool parentPos;
    public int adjustment;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (StartStaticized) {
            Staticize();
        }
    }
    void Update()
    {
        float pos = transform.position.y;
        if(parentPos)
            pos = transform.parent.position.y;
        sprite.sortingOrder = -1 * (int)(pos * 100);
        if (foreground)
            sprite.sortingOrder += 5;
        sprite.sortingOrder += adjustment;
    }
    public void Staticize() {
        sprite = GetComponent<SpriteRenderer>();
        float pos = transform.position.y;
        if (parentPos)
            pos = transform.parent.position.y;
        sprite.sortingOrder = -1 * (int)(pos * 100);
        if (foreground)
            sprite.sortingOrder += 5;
        sprite.sortingOrder += adjustment;
        GameObject.Destroy(this);
    }
}
