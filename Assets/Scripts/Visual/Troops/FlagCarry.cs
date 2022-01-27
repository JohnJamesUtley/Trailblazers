using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagCarry : MonoBehaviour
{
    public GameObject carrier;
    bool dropped = false;
    void Update()
    {
        if (!dropped) {
            if (carrier != null) {
                transform.position = (Vector2)carrier.transform.position + new Vector2(0.1468f, 0.212f);
            } else {
                dropped = true;
                transform.rotation = Quaternion.Euler(0,0,(GetNegPos() * 80) + ((Random.value * 2) - 1) * 10);
                transform.localScale = new Vector3(GetNegPos() * 1, 1, 1);
                transform.position = (Vector2)transform.position + new Vector2(0, -0.212f) + (Random.insideUnitCircle * 0.5f);
                transform.GetChild(0).GetComponent<CohortNumber>().KillCohortNumber();
                transform.GetChild(0).GetComponent<Orderer>().adjustment = -19;
                GetComponent<Orderer>().adjustment = -20;
                GetComponent<Orderer>().Staticize();
                GameObject.Destroy(this);
            }
        }
    }
    float GetNegPos() {
        float rand = (Random.value * 2) - 1;
        if (rand < 0)
            return -1f;
        return 1f;
    }
}
