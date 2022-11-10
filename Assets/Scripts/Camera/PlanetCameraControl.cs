using UnityEngine;

public class PlanetCameraControl : MonoBehaviour {

    [SerializeField]
    private float movement_rate = 0.01f;

    private Transform cached_transform;

    private Vector3
        position = new Vector3( 0.0f, 0.0f, 0.0f ),
        game_camera_start_position = new Vector3( 0.0f, 0.0f, 0.0f );

    // Starting initialization #################################################################################################################################################
    void Start() {

        cached_transform = GetComponent<Transform>() as Transform;

        game_camera_start_position = Game.Camera_transform.position;

        position = cached_transform.position;
    }
    
    // Update is called once per frame #########################################################################################################################################
    void Update() {

        position.x = (Game.Camera_transform.position.x - game_camera_start_position.x) * movement_rate;
        position.y = (Game.Camera_transform.position.y - game_camera_start_position.y) * movement_rate;

        cached_transform.position = position;
	}
}