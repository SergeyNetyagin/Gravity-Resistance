using UnityEngine;
using System.Collections.Generic;

public class ListStations : MonoBehaviour {

    [SerializeField]
    private bool 
        collect_subchild_objects = false,
        mission_stations_only = false;

    private List<ObstacleControl> list_stations = new List<ObstacleControl>();

    private Transform cached_transform;

    public int Count { get { return list_stations.Count; } }
    public Station GetStation( int index ) { return ((index >= 0) && (index < Count)) ? list_stations[ index ].Station : null; }
    public Transform GetTransform( int index ) { return ((index < Count) && (index >= 0)) ? list_stations[ index ].Cached_transform : null; }
    public ObstacleControl GetObstacle( int index ) { return ((index < Count) && (index >= 0)) ? list_stations[ index ] : null; }
        
    // Use this for initialization #############################################################################################################################################
	void Start () {

        cached_transform = transform;

        for( int i = 0; i < cached_transform.childCount; i++ ) {

            // If need to takes of only child objects of the parent object
            if( collect_subchild_objects ) {

                Transform group_station_transform = cached_transform.GetChild( i );

                for( int j = 0; j < group_station_transform.childCount; j++ ) {

                    if( mission_stations_only && !group_station_transform.GetChild( j ).GetComponent<Station>().Use_in_missions ) continue;
                    else if( group_station_transform.GetChild( j ).GetComponent<Station>() != null ) list_stations.Add( group_station_transform.GetChild( j ).GetComponent<ObstacleControl>() );
                }

            }

            // If need to takes of all child objects of the parent's child objects
            else {

                if( mission_stations_only && cached_transform.GetChild( i ).GetComponent<Station>().Use_in_missions ) continue;
                else if( cached_transform.GetChild( i ).GetComponent<Station>() != null ) list_stations.Add( cached_transform.GetChild( i ).GetComponent<ObstacleControl>() );
            }
        }

        // Check for the station names
        CheckStationNames();
	}

    // Check for the station names #############################################################################################################################################
    void CheckStationNames() {

        for( int i = 0; i < list_stations.Count; i++ ) {

            Station checking_station = list_stations[i].Station;

            if( checking_station == null ) {

                #if UNITY_EDITOR
                Debug.Log( "Object " + checking_station + " has not component <Station>" );
                #endif
            }

            else {

                if( string.IsNullOrEmpty( checking_station.Name_key ) ) {

                    #if UNITY_EDITOR
                    Debug.Log( "Object " + checking_station.name + " has empty name of the station" );
                    #endif
                }

                for( int j = i + 1; j < list_stations.Count; j++ ) {

                    Station other_station = list_stations[j].Station;

                    if( other_station == null ) continue;

                    if( string.Equals( checking_station.Name_key, other_station.Name_key ) ) {

                        #if UNITY_EDITOR
                        Debug.Log( "The " + checking_station.name + " station and the " + other_station.name + " station have identical names" );
                        #endif
                    }
                }
            }
        }
    }
    
    // Detect the nearest station ##############################################################################################################################################
    public IDetecting DetectNearestStation() {

        Vector2 distance;

        IDetecting nearest_station = null;

        float min_sqr_magnitude = float.MaxValue;

        for( int i = 0; i < list_stations.Count; i++ ) {

            distance.x = Mathf.Abs( Game.Player_transform.position.x - list_stations[i].Cached_transform.position.x );
            distance.y = Mathf.Abs( Game.Player_transform.position.y - list_stations[i].Cached_transform.position.y );

            if( distance.sqrMagnitude < min_sqr_magnitude ) {

                min_sqr_magnitude = distance.sqrMagnitude;
                nearest_station = list_stations[i];
            }
        }

        if( nearest_station != null ) {

            nearest_station.Magnitude = Mathf.Sqrt( min_sqr_magnitude );
            nearest_station.Sqr_magnitude = min_sqr_magnitude;
            nearest_station.Detected_point = nearest_station.Cached_transform.position;
        }

        return nearest_station;
    }
}