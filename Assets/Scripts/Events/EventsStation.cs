using UnityEngine;

public class EventsStation : MonoBehaviour {

    private Animator animator;

    // Start ###################################################################################################################################################################
    void Start() {

        animator = GetComponent<Animator>();
    }

    // Animation event: station's service ######################################################################################################################################
    public void EventAnimationServiceEnabled() {

        Game.Canvas.RefreshServiceIndicators( true );
    }

    // Animation event: station's service ######################################################################################################################################
    public void EventAnimationServiceDisabled() {

        Game.Canvas.RefreshServiceIndicators( false );
    }
}