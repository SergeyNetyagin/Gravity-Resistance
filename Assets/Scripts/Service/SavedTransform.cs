using UnityEngine;

public class SavedTransform : MonoBehaviour {

	private Vector3 
        saved_position,
        saved_angles,
        saved_scale;

    public Vector3 Start_scale { get { return saved_scale; } }
    public Vector3 Start_angles { get { return saved_angles; } }
    public Vector3 Start_position { get { return saved_position; } }

    public void RestoreScale() { transform.localScale = saved_scale; }
    public void RestoreAngles() { transform.eulerAngles = saved_angles; }
    public void RestorePosition() { transform.position = saved_position; }
    public void RestoreTransform() { RestorePosition(); RestoreAngles(); RestoreScale(); }

    // Save a start transform ##################################################################################################################################################
	void Awake() {

		saved_scale = gameObject.transform.localScale;
        saved_angles = gameObject.transform.eulerAngles;
		saved_position = gameObject.transform.position;
	}

    // Starting initialization #################################################################################################################################################
    void Start() {

    }
}
