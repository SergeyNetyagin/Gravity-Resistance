using UnityEngine;

public class StationAutolandingShip : MonoBehaviour {

    private Vector2 landing_point = new Vector2( 0f, 0f );

    private Transform cached_transform;

    // Initialise components ###################################################################################################################################################
	void Start() {

        cached_transform = transform;
    }

    // Collision analysis ######################################################################################################################################################
	void OnTriggerEnter( Collider collider ) {

        if( !collider.CompareTag( "Landing" ) ) return;

        if( Game.Player.Is_autolanding || (Game.Player.Ship.Autolanding_amount.Available <= 0f) ) Game.Canvas.RefreshAutolandingIndicator( false );
        else Game.Canvas.RefreshAutolandingIndicator( true );

        Game.Canvas.ShowAutolandingButton();

        landing_point.x = cached_transform.position.x;
        landing_point.y = cached_transform.position.y;
        Game.Player.Landing_axle = landing_point;
    }

    // #########################################################################################################################################################################
	void OnTriggerExit( Collider collider ) {

        if( !collider.CompareTag( "Landing" ) ) return;

        Game.Canvas.RefreshAutolandingIndicator( false );
        Game.Canvas.HideAutolandingButton();
    }
}