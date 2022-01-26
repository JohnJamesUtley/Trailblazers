using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    bool initialized = false;
    public Vector2 target;
    float speed = 5;
    float gravAccel = -4f;
    float yVel = 0;
    float height;
    float landingOffset = 0.15f;
    Vector3 lastPos;
    public void SetTarget(Vector2 target) {
        initialized = true;
        this.target = target;
        height = transform.GetChild(0).localPosition.y;
        float timeToDest = Vector2.Distance(target, (Vector2)transform.position) / speed;
        yVel = -0.5f * gravAccel * timeToDest + (landingOffset - height) / timeToDest;
        lastPos = transform.GetChild(0).position;
    }

    void Update()
    {
        if (initialized) {
            yVel += gravAccel * Time.deltaTime;
            height += yVel * Time.deltaTime;
            transform.GetChild(0).localPosition = new Vector2(transform.GetChild(0).localPosition.x, height);
            transform.position += ((Vector3)target - transform.position).normalized * Time.deltaTime * speed;
            Vector3 change = transform.GetChild(0).position - lastPos;
            lastPos = transform.GetChild(0).position;
            float ang = Mathf.Atan2(change.y, change.x) * Mathf.Rad2Deg + 270;
            Quaternion rotation = Quaternion.AngleAxis(ang, Vector3.forward);
            transform.GetChild(0).rotation = rotation;
            if (Vector2.Distance(target, (Vector2)transform.position) < 0.05f)
                Land();
        }
    }
    void Land() {
        initialized = false;
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.GetMask("Troops"));
        if (nearby.Length > 0) {
            Troop hit = nearby[0].gameObject.GetComponent<Troop>();
            bool isShielded = false;
            if (hit is Infrantry) {
                if (((Infrantry)hit).shield) {
                    isShielded = true;
                }
            }
            if (!isShielded) {
                if (Random.value > 0.5f) {
                    nearby[0].gameObject.GetComponent<Troop>().Kill();
                }
            } else {
                if (Random.value > 0.95f) {
                    nearby[0].gameObject.GetComponent<Troop>().Kill();
                }
            }
        }
        Destroy(gameObject);
    }
}
