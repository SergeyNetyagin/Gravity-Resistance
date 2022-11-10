using UnityEngine;

public class StationSitShip : MonoBehaviour {

    // Initialise components ###################################################################################################################################################
	void Start() {

    }

    // Collision analysis ######################################################################################################################################################
	void OnTriggerEnter( Collider collider ) {

        if( !collider.CompareTag( "Support" ) ) return;

        Game.Player.ResetShipConstraints();

        if( Game.Player.Is_landing_zone ) return;

        Game.Player.Ship.AttachSupport( collider );

        if( Game.Player.Ship.All_supports_attached ) {

            Game.Player.AddState( PlayerState.Landing_zone );
            if( Game.Use_horizontal_control ) Game.Input_control.DisableHorizontalControl();
        }
    }
            
    // #########################################################################################################################################################################
    void OnTriggerExit( Collider collider ) {

        if( !collider.CompareTag( "Support" ) ) return;

        if( !Game.Player.Is_landing_zone ) return;

        Game.Player.Ship.DettachSupport( collider );

        if( Game.Player.Ship.All_supports_dettached ) {

            Game.Player.ResetState( PlayerState.Landing_zone );
            if( Game.Use_horizontal_control ) Game.Input_control.EnableHorizontalControl();
        }
    }
}