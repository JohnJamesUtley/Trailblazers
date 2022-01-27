using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    float time;
    [SerializeField]
    float progression = 0f;
    public float speed;
    public float height;
    public float addedHeight;
    public AnimationCurve curve;
    void Start() {
        time = Random.Range(0,100);
    }
    void Update()
    {
        time += Time.deltaTime;
        progression = (Mathf.Sin(speed * time) + 1)/2;
        transform.localPosition = new Vector2(0, curve.Evaluate(progression) * height + addedHeight);
    }
}
