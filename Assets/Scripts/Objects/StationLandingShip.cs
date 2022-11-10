using UnityEngine;

public class StationLandingShip : MonoBehaviour {

    private int animation_ID_ship_sit;
    private int animation_ID_ship_landing;

    // Initialise components ###################################################################################################################################################
	void Start() {

        animation_ID_ship_sit = Animator.StringToHash( "Ship_sit" );
        animation_ID_ship_landing = Animator.StringToHash( "Ship_landing" );
    }

    // Detecting triggers ######################################################################################################################################################
    void OnTriggerEnter( Collider collider ) {

        if( !collider.CompareTag( "Landing" ) ) return;

        Game.Player.Ship_animator.SetInteger( animation_ID_ship_sit, 0 );
        Game.Player.Ship_animator.SetInteger( animation_ID_ship_landing, 1 );
    }
            
    // #########################################################################################################################################################################
    void OnTriggerExit( Collider collider ) {

        if( !collider.CompareTag( "Landing" ) ) return;

        Game.Player.Ship_animator.SetInteger( animation_ID_ship_sit, 0 );
        Game.Player.Ship_animator.SetInteger( animation_ID_ship_landing, -1 );
    }
}