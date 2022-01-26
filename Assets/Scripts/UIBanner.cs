using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBanner : MonoBehaviour
{
    public Sprite highlighted;
    Sprite unhighlighted;
    private void Awake() {
         unhighlighted = transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite;
    }
    public void Select() {
        transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = highlighted;
    }
    public void Deselect() {
        transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = unhighlighted;
    }
}
