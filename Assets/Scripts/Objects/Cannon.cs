using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour {

    [HideInInspector]
    // Visible clamp in Unity editor for manual adjustment (left top maximum shot point) - for using in EditCannon
    public Vector3 Shot_sector_left_top_point = Vector3.zero;

    [HideInInspector]
    // Visible clamp in Unity editor for manual adjustment (right top maximum shot point) - for using in EditCannon
    public Vector3 Shot_sector_right_top_point = Vector3.zero;

    [HideInInspector]
    // Distance for shooting - calculated in EditCannon
    public float Max_distance = 0.0f;

    [SerializeField]
    private Transform bullet_start_point;

    [SerializeField]
    private GameObject[] bullet_prefabs;

    [SerializeField]
    private float attack_rate = 2.0f;
    
    [SerializeField]
    private float activation_distance = 50.0f;

    [SerializeField]
    private bool is_can_awake = true;

    [SerializeField]
    private float reaction_speed = 10.0f;

    [SerializeField]
    private float rotation_time = 0.0f;
    private float rotation_timer = 0.0f;
    private float attack_timer = 0.0f;

    // Calculated variables for changing of cannon state
    private Quaternion look_rotation;
    private Quaternion look_quaternion_right_top;
    private Quaternion look_quaternion_left_forward;
    private float start_angle = 0.0f;
    private float angle_devider = 1.0f;
    private float angle_right_top = 0.0f;
    private float angle_left_forward = 0.0f;

    private AudioSource shot_sound;
    private AnimationRotation animation_rotation;
    private AnimationMovement animation_movement;

    // Starting initialization #################################################################################################################################################
    void Start() {

        shot_sound = GetComponent<AudioSource>() as AudioSource;
        animation_rotation = GetComponent<AnimationRotation>() as AnimationRotation;
        animation_movement = GetComponent<AnimationMovement>() as AnimationMovement;
    }

    // #########################################################################################################################################################################
    private void AttackTarget() {

    }

    // #########################################################################################################################################################################
    private void WaitForTarget() {

    }

    // #########################################################################################################################################################################
    private void FireCannon() {

    }

    // #########################################################################################################################################################################
    private bool IsSeeTarget( Vector3 point ) {

        return false;
    }
}
