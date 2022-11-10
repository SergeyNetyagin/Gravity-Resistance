using UnityEngine;

public class EdgeControl : MonoBehaviour {

    // Starting initialization #################################################################################################################################################
    void Start() {

    }
    
    // Disable of some objects #################################################################################################################################################
    void OnCollisionEnter( Collision collision ) {

        Comet comet = collision.gameObject.GetComponent<Comet>();
        Wanderer wanderer = collision.gameObject.GetComponent<Wanderer>();

        if( comet != null ) comet.GetComponent<AnimationMovement>().enabled = false;
        else if( wanderer != null ) wanderer.Sleep( false );
    }
}
