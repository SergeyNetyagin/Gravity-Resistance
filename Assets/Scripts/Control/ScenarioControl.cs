using UnityEngine;
using System.Collections;

public class ScenarioControl : MonoBehaviour {

    [System.Serializable]
    private class ScenarioItem {

        public MissionType mission_type;

        [Tooltip( "Will does game use this mission cyclically or one time only" )]
        public bool is_reuse = true;

        [Range( 0f, 1f )]
        public float probability_of_use = 0.5f;

        public GameObject mission_prefab;
        [HideInInspector, System.NonSerialized]
        public ObstacleControl obstacle;

        [Tooltip( "Places of constant work: if they are empty, game will assign automaticaly ones" )]
        [Space( 10 )]
        public ObstacleControl source;
        public ObstacleControl destination;

        [System.NonSerialized]
        public float delivery_time = 0f;

        [HideInInspector, System.NonSerialized]
        public bool was_used = false;

        [HideInInspector, System.NonSerialized]
        public float accumulated_probabilty = 0f;
    }

    [Space( 10 )]
    [Header( "SCENARIO'S PROPERTIES" )]
    [Tooltip( "Average speed of movement on the level: unit in second" )]
    [Range( 0.1f, 10f )]
    [SerializeField]
    private float delivery_speed = 1f;

    [Tooltip( "Checking time for choice of new scenario" )]
    [Range( 10f, 300f )]
    [SerializeField]
    private float scenario_check_time = 30f;

    [Header( "LIST'S OF KEY SPACE PLACES OF SCENARIO" )]
    [SerializeField]
    private ListStations list_stations;

    [SerializeField]
    private ListPlaces list_places;

    [Header( "ITEMS OF SCENARIO" )]
    [SerializeField]
    private ScenarioItem[] scenario;

    private ScenarioItem current_mission;
    public bool Has_active_mission { get { return (current_mission != null); } }

    [System.NonSerialized]
    private ObstacleControl current_target;
    public ObstacleControl Current_target { get { return current_target; } }

    public ObstacleControl Source { get { return (current_mission == null) ? null : current_mission.source; } }
    public ObstacleControl Destination { get { return (current_mission == null) ? null : current_mission.destination; } }

    private WaitForSeconds check_scenario_wait_for_seconds;
    
    // Use this for initialization #############################################################################################################################################
	void Start() {

        current_target = null;

        check_scenario_wait_for_seconds = new WaitForSeconds( scenario_check_time );
//        StartCoroutine( CheckForNewMission() );
	}

    // Check scenario and start a new mission if it possible ###################################################################################################################
    IEnumerator CheckForNewMission() {

        while( !Game.Is( GameState.Complete ) ) {

            // Assign the special mission's object
            for( int i = 0, found = 0; !Has_active_mission && (i < scenario.Length); i++ ) {

                found = Random.Range( 0, scenario.Length );

                if( !scenario[ found ].is_reuse && scenario[ found ].was_used ) continue;
                else scenario[ found ].accumulated_probabilty += scenario[ found ].probability_of_use;

                if( scenario[ found ].accumulated_probabilty >= 1f ) {

                    current_mission = scenario[ found ];
                    current_mission.was_used = true;
                    current_mission.accumulated_probabilty = 0f;

                    current_mission.source = null;
                    current_mission.destination = null;
                    current_mission.delivery_time = 0f;

                    if( MissionActivated() ) break;
                    else ReleaseMissionResources();
                }
            }

            yield return check_scenario_wait_for_seconds;
        }

        yield break;
    }

    // Releases mission's busy resources #######################################################################################################################################
    void ReleaseMissionResources() {

        if( current_mission == null ) return;

        if( (current_mission.source != null) && current_mission.source.Is_station ) current_mission.source.Station.CancelMissionValue();
        if( current_mission.obstacle != null ) current_mission.obstacle.Sleep();

        if( (current_mission.source != null) && (current_mission.source.GetComponent<Place>() != null) ) current_mission.source.GetComponent<Place>().SetAsFree();
        if( (current_mission.destination != null) && (current_mission.destination.GetComponent<Place>() != null) ) current_mission.destination.GetComponent<Place>().SetAsFree();

        current_mission = null;
    }

    // Activate a new mission and source object ################################################################################################################################
    bool MissionActivated() {

        switch( current_mission.mission_type ) {

            case MissionType.Deliver_freight:

                if( !MissionObjectActivated( true, true ) ) return false;
                break;

            default:

                return false;
        }

        Game.Message.Show( current_mission.obstacle.Mission.Source_message_key );

        return true;
    }

    // Activate the freight ####################################################################################################################################################
    bool MissionObjectActivated( bool source_at_station, bool destination_at_station ) {

        // Find the mission's source
        if( source_at_station ) current_mission.source = GetSuitableStation( null );
        //else current_mission.source_transform = GetSuitablePlace( null );
        //if( current_mission.source_transform == null ) return false;

        // Find the mission's destination
        if( destination_at_station ) current_mission.destination = GetSuitableStation( current_mission.source );
        //else current_mission.destination_transform = GetSuitablePlace( current_mission.source_transform );
        //if( current_mission.destination_transform == null ) return false;

        // Create object if it is not created yet or wake up it
        if( current_mission.obstacle == null ) current_mission.obstacle = Instantiate( current_mission.mission_prefab ).GetComponent<ObstacleControl>();
        else current_mission.obstacle.WakeUp();

/*        current_mission.object_transform = current_mission.mission_object.transform;
        current_mission.object_transform.parent = transform;

        current_mission.object_transform.position = current_mission.source_transform.position;
        current_mission.object_transform.Translate( 0f, current_mission.mission.Altitude_over_station, 0f );

        current_target_transform = current_mission.source_transform;
        current_mission.source_transform.GetComponent<Station>().AssignMissionValue( current_mission.object_transform );

        current_mission.value = current_mission.mission_object.GetComponent<Value>();
        if( (current_mission.value != null) ) if( Game.Control.IsPerishable( current_mission.value.Subject_type ) ) SetMissionTime();
*/
        return true;
    }

    // Returns the random free mission #########################################################################################################################################
    ObstacleControl GetSuitableStation( ObstacleControl conflict_obstacle ) {

        if( list_stations.Count == 0 ) return null;

        ObstacleControl station = null;

        // Find next
        int i = Random.Range( 0, list_stations.Count ), index = i; do {
            
            station = list_stations.GetObstacle( i );
            if( !station.Is_visible && (station != conflict_obstacle) ) return station;
            else station = null;

        } while( ++i < list_stations.Count );

        // Find previous
        for( i = index - 1; i >= 0; i-- ) {

            station = list_stations.GetObstacle( i );
            if( !station.Is_visible && (station != conflict_obstacle) ) return station;
            else station = null;
        }

        return station;
    }

    // Returns the random source station  ######################################################################################################################################
    Transform GetSuitablePlace( Transform conflict_transform ) {

        if( list_places.Count == 0 ) return null;

        Transform place_transform = null;

        // Find next
        int i = Random.Range( 0, list_places.Count ), index = i; do {
   
            place_transform = list_places.GetTransform( i );
            if( !place_transform.GetComponent<Place>().Is_busy && (place_transform != conflict_transform) ) return place_transform;
            else place_transform = null;

        } while( ++i < list_places.Count );

        // Find previous
        for( i = index - 1; i >= 0; i-- ) {

            place_transform = list_places.GetTransform( i );
            if( !place_transform.GetComponent<Place>().Is_busy && (place_transform != conflict_transform) ) return place_transform;
            else place_transform = null;
        }

        return place_transform;
    }
    
    // Cancel the mission ######################################################################################################################################################
    public void CancelMission() {

        if( current_mission == null ) return;

        if( Game.Timer.Is_enabled ) Game.Timer.Stop();

        if( current_mission.obstacle.Mission.Penalty > 0f ) {

            float penalty = Mathf.Floor( current_mission.obstacle.Mission.Penalty * Game.Level.Complication * 0.1f ) * 10f;

            Game.Money -= penalty;
            Game.Canvas.RefreshMoneyIndicator( true );

            Game.Effects_control.Show( current_mission.obstacle.Mission.Failure_prefab, current_mission.obstacle.Cached_transform.position, true);

            Game.Message.Hide( current_mission.obstacle.Mission.Source_message_key );
            Game.Message.Hide( current_mission.obstacle.Mission.Destination_message_key );
            Game.Message.Show( current_mission.obstacle.Mission.Failed_message_key );
        }

        ReleaseMissionResources();
    }

    // Cancel the mission ######################################################################################################################################################
    public void MissionAccomplished() {

        if( current_mission == null ) return;

        if( Game.Timer.Is_enabled ) Game.Timer.Stop();

        if( current_mission.obstacle.Mission.Reward > 0f ) {

            float reward = Mathf.Floor( current_mission.obstacle.Mission.Reward * Game.Level.Complication * 0.1f ) * 10f;

            Game.Money += reward;
            Game.Canvas.RefreshMoneyIndicator( true );

            Game.Effects_control.Show( current_mission.obstacle.Mission.Success_prefab, current_mission.obstacle.Cached_transform.position, true);

            Game.Message.Hide( current_mission.obstacle.Mission.Destination_message_key );
            Game.Message.Show( current_mission.obstacle.Mission.Accomplished_message_key );
        }

        ReleaseMissionResources();
    }
    
    // Activate a new mission and it source object #############################################################################################################################
    public bool ActivateMissionDestination() {

        current_target = null;

        if( current_mission == null ) { CancelMission(); return false; }

        switch( current_mission.mission_type ) {

            case MissionType.Deliver_freight:

                if( current_mission.source.Station != null ) current_mission.source.Station.CancelMissionValue();
                
                current_target = current_mission.destination;

                Game.Message.Hide( current_mission.obstacle.Mission.Source_message_key );
                Game.Message.Show( current_mission.obstacle.Mission.Destination_message_key );
                
                break;
        }

        return (current_target != null);
    }

    // Drop the freight in space ###############################################################################################################################################
    public void DropMissionLoad( Value value ) {

        if( value == null ) return;
        if( !value.Is_mission ) return;
        if( current_mission == null ) return;
// Здесь в этот момент нужно сделать так, чтобы новые миссии не выдавались в течени этих 3-х секунд - вдруг игрок снова подберёт миссионный груз и проолжит миссию
// Или более либеральный вариант: пусть этот груз так и летает до конца уровня: если его подобрать и при этом нет активной миссии, тогда можно продолжить подобранную миссию
// Короче, нужно подумать...
        StartCoroutine( DestroyMissionLoad( 3f ) );
    }

    // Destroy the mission freight #############################################################################################################################################
    IEnumerator DestroyMissionLoad( float time ) {

        while( time > 0f ) {

            time -= Time.deltaTime;

            yield return null;
        }

        CancelMission();

        yield break;
    }
    
    // Set the mission's time ##################################################################################################################################################
    public void SetMissionTime() {

        if( current_mission == null ) return;
        if( (current_mission.source == null) || (current_mission.destination == null) ) return;
        if( current_mission.delivery_time != 0f ) return;

        Vector3 operation;

        operation.x = current_mission.source.Cached_transform.position.x - current_mission.destination.Cached_transform.position.x;
        operation.y = current_mission.source.Cached_transform.position.y - current_mission.destination.Cached_transform.position.y;
        operation.z = current_mission.source.Cached_transform.position.z - current_mission.destination.Cached_transform.position.z;

        current_mission.delivery_time = operation.magnitude / (delivery_speed * Game.Level.Complication);
    }

    // Определяет точку миссии #################################################################################################################################################
    public IDetecting DetectMissionPoint() {

        Vector2 distance;

        if( current_target != null ) {

            distance.x = Game.Player_transform.position.x - current_target.Cached_transform.position.x;
            distance.y = Game.Player_transform.position.y - current_target.Cached_transform.position.y;

            current_target.Magnitude = distance.magnitude;
            current_target.Sqr_magnitude = distance.sqrMagnitude;
            current_target.Detected_point = current_target.Cached_transform.position;
        }

        return null;
    } 
}