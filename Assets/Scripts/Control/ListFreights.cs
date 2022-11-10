using UnityEngine;
using System.Collections.Generic;

public class ListFreights : MonoBehaviour {

    [SerializeField]
    private bool 
        collect_subchild_objects = false,
        mission_objects_only = true,
        activate_on_start = false;

    private List<Transform> list_transform_freights = new List<Transform>();

    private Transform cached_transform;

    public int Count { get { return list_transform_freights.Count; } }
    public Freight GetFreight( int index ) { return ((index >= 0) && (index < Count)) ? list_transform_freights[ index ].GetComponent<Freight>() : null; }
    public Transform GetTransform( int index ) { return ((index < Count) && (index >= 0)) ? list_transform_freights[ index ] : null; }

    // Returns only free freight (which not in space) ##########################################################################################################################
    public Transform GetOnlyFreeTransform( int index ) {

        if( (index >= Count) || (index < 0) ) return null;

        Rigidbody physics = list_transform_freights[ index ].GetComponent<Rigidbody>() as Rigidbody;

        if( (physics.constraints & RigidbodyConstraints.FreezeRotationX) != 0 ) return list_transform_freights[ index ];
        else return null;
    }

    // Use this for initialization #############################################################################################################################################
	void Start () {

        cached_transform = transform;
        	
        for( int i = 0; i < cached_transform.childCount; i++ ) {

            // If need to takes of only child objects of the parent object
            if( collect_subchild_objects ) {

                Transform group_freights_transform = cached_transform.GetChild( i );

                for( int j = 0; j < group_freights_transform.childCount; j++ ) {

                    if( mission_objects_only && (group_freights_transform.GetChild( j ).GetComponent<Mission>() == null) ) continue;
                    else if( group_freights_transform.GetChild( j ).GetComponent<Freight>() != null ) list_transform_freights.Add( group_freights_transform.GetChild( j ) );

                    group_freights_transform.GetChild( j ).gameObject.SetActive( activate_on_start );
                }

            }

            // If need to takes of all child objects of the parent's child objects
            else {

                if( mission_objects_only && (cached_transform.GetChild( i ).GetComponent<Mission>() == null) ) continue;
                else if( cached_transform.GetChild( i ).GetComponent<Freight>() != null ) list_transform_freights.Add( cached_transform.GetChild( i ) );

                cached_transform.GetChild( i ).gameObject.SetActive( activate_on_start );
            }
        }
    }
}
