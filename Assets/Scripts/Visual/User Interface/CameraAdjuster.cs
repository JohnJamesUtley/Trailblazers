using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdjuster : MonoBehaviour
{
    public bool onLeaders = true;
    public List<GameObject> leaders;
    public Vector3 target;
    Camera cam;
    float camSpeed = 0.015f;
    float camSizeSpeed = 0.05f;
    float ratio;
    private void Start() {
        cam = GetComponent<Camera>();
    }
    public void SetTarget(Vector2 small, Vector2 large, float buffer) {
        float smX = small.x;
        float lgX = large.x;
        float smY = small.y;
        float lgY = large.y;
        Vector2 pos = (Vector3)((new Vector2(lgX, lgY) - new Vector2(smX, smY)) / 2) + new Vector3(smX, smY, -10f);
        float ySize = ((lgY - smY) + buffer) / 2;
        float xSize = (((lgX - smX) + buffer) / 2) / ratio;
        float size = Mathf.Max(ySize, xSize);
        target = new Vector3(pos.x, pos.y, size);
    }
    void Update()
    {
        ratio = cam.aspect;
        if (onLeaders) {
            float smX = leaders[0].transform.position.x;
            float lgX = leaders[0].transform.position.x;
            float smY = leaders[0].transform.position.y;
            float lgY = leaders[0].transform.position.y;
            foreach (GameObject x in leaders) {
                if (x.transform.position.x > lgX) {
                    lgX = x.transform.position.x;
                }
                if (x.transform.position.y > lgY) {
                    lgY = x.transform.position.y;
                }
                if (x.transform.position.x < smX) {
                    smX = x.transform.position.x;
                }
                if (x.transform.position.y < smY) {
                    smY = x.transform.position.y;
                }
            }
            SetTarget(new Vector2(smX, smY), new Vector2(lgX, lgY), 4f);
        }
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.x, target.y, -1), camSpeed);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, target.z, camSizeSpeed);
    }
}
