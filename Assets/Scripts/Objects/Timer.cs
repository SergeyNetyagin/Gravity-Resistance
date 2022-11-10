using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {

    private float total_time = 0f;
    private float total_time_inversed = 0f;
    public float Total_time { get { return total_time; } }
    public float Total_time_inversed { get { return total_time_inversed; } }

    private float current_time = 0f;
    public float Current_time { get { return current_time; } }

    private bool is_enabled = false;
    public bool Is_enabled { get { return is_enabled; } }

    private float refresh_time = 1f;

    private WaitForSeconds timer_wait_for_seconds;

	// Use this for initialization #############################################################################################################################################
	void Start() {

        timer_wait_for_seconds = new WaitForSeconds( refresh_time );
	}
	
	// Start the timer #########################################################################################################################################################
	public void Start( float time ) {

        is_enabled = true;
        total_time = current_time = time;

        total_time_inversed = 1f / total_time;

        StartCoroutine( RefreshTimer() );
	}

	// Internal stop the timer #################################################################################################################################################
	public void Stop() {

        if( Game.Scenario_control.Has_active_mission ) Game.Scenario_control.CancelMission();

        is_enabled = false;
        current_time = 0f;

        total_time = total_time_inversed = 0f;
    }
    
    // Refresh navigator's time ################################################################################################################################################
    IEnumerator RefreshTimer() {

        while( current_time > 0f ) {

            current_time -= refresh_time;

            yield return timer_wait_for_seconds;
        }

        Stop();

        yield break;
    }
}
