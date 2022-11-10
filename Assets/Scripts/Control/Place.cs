using UnityEngine;

public class Place : MonoBehaviour {

    private bool is_free = true;
    public bool Is_free { get { return is_free; } }
    public bool Is_busy { get { return !is_free; } }

    public void SetAsBusy() { is_free = false; }
    public void SetAsFree() { is_free = true; }

    void Awake() {

        GetComponent<SpriteRenderer>().enabled = false;
    }

    void Start() {

        GetComponent<Transform>().localScale = Vector3.one;
    }
}