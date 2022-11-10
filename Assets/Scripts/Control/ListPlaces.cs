using UnityEngine;
using System.Collections.Generic;

public class ListPlaces : MonoBehaviour {

    private List<Transform> list_transform_places = new List<Transform>();

    private Transform cached_transform;

    public int Count { get { return list_transform_places.Count; } }
    public Place GetPlace( int index ) { return ((index < Count) && (index >= 0)) ? list_transform_places[ index ].GetComponent<Place>() : null; }
    public Transform GetTransform( int index ) { return ((index < Count) && (index >= 0)) ? list_transform_places[ index ] : null; }

    public void SetAsBusy( int index ) { Place place = GetPlace( index ); if( place != null ) place.SetAsBusy(); }
    public void SetAsFree( int index ) { Place place = GetPlace( index ); if( place != null ) place.SetAsFree(); }

    public bool IsBusy( int index ) { Place place = GetPlace( index ); return (place != null) ? place.Is_busy : false; }
    public bool IsFree( int index ) { Place place = GetPlace( index ); return (place != null) ? place.Is_free : false; }
    
    // Use this for initialization #############################################################################################################################################
	void Start () {

        cached_transform = transform;
        	
        for( int i = 0; i < cached_transform.childCount; i++ ) {

            if( cached_transform.GetChild( i ).GetComponent<Place>() != null ) list_transform_places.Add( cached_transform.GetChild( i ) );
            cached_transform.GetChild( i ).gameObject.SetActive( false );
        }
    }

    // Returns a free place's transform ########################################################################################################################################
    public Transform GetFreeRandomPlace() {

        Place place;

        // Find next
        int i = Random.Range( 0, list_transform_places.Count - 1 ), index = i; do {

            place = GetPlace( i );

            if( (place != null) && place.Is_free ) {

                place.SetAsBusy();
                return list_transform_places[i];
            }

        } while( ++i < list_transform_places.Count );

        // Find previous
        for( i = index - 1; i > 0; i-- ) {

            place = GetPlace( i );

            if( (place != null) && place.Is_free ) {

                place.SetAsBusy();
                return list_transform_places[i];
            }
        }

        return null;
    }
}
