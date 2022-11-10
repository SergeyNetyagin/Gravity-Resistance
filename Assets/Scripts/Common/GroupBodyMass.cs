using UnityEngine;

public class GroupBodyMass : MonoBehaviour {

    void Start() {

        #if UNITY_EDITOR
        CalculateBodyMass();
        #endif
    }

    // Recalculate cost of the freight #########################################################################################################################################
    [ContextMenu( "CUSTOM: Calculate space bodies' mass and cost" )]
    private void CalculateBodyMass() {

        for( int i = 0; i < transform.childCount; i++ ) {

            AutoMass mass = transform.GetChild( i ).GetComponent<AutoMass>();
            if( mass != null ) mass.CalculateMass();

            Value value = transform.GetChild( i ).GetComponent<Value>();
            if( value != null ) value.FullMassAndCostCalculation();
        }

        #if UNITY_EDITOR
        if( !Application.isPlaying ) Debug.Log( "The child objects' Freight and Rigidbody mass of the <" + gameObject.name + "> is calculated" );
        #endif
    }
}