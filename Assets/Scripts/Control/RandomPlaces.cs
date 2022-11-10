using UnityEngine;
using System.Collections.Generic;

public class RandomPlaces : MonoBehaviour {

    [SerializeField]
    private bool collect_subchild_objects = false;

    [SerializeField]
    private ListPlaces list_places;

    private Transform cached_transform;

    // Use this for initialization #############################################################################################################################################
	void Start () {

        cached_transform = transform;
        	
        for( int i = 0; i < cached_transform.childCount; i++ ) {

            // If need to takes of only child objects of the parent object
            if( collect_subchild_objects ) {

                Transform group_freights_transform = cached_transform.GetChild( i );

                for( int j = 0; j < group_freights_transform.childCount; j++ ) {

                    group_freights_transform.GetChild( j ).GetComponent<Transform>().position = list_places.GetFreeRandomPlace().position;
                }
            }

            // If need to takes of all child objects of the parent's child objects
            else {

                cached_transform.GetChild( i ).GetComponent<Transform>().position = list_places.GetFreeRandomPlace().position;
            }
        }
    }
}
