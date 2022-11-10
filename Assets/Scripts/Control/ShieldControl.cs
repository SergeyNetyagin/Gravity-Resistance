using UnityEngine;
using System.Collections;

public class ShieldControl : MonoBehaviour {

    // Истина, если защитное поле включено
    private bool is_active = false;
    public bool Is_active { get { return is_active; } }

    // Истина, если защитное поле готово к применению
    private bool is_ready = false;
    public bool Is_ready { get { return is_ready; } }

    private Ship ship;
    private Protection[] shields;
    private SoundEffects sound_effects;

    private float charge_time = 0f;
    private float protect_time = 0f;

    private WaitForSeconds charge_wait_for_seconds;
    private WaitForSeconds protect_wait_for_seconds;

    private Transform cached_transform;

    public bool Is_used { get { return (ship.Shield_power.Available > 0f); } }

    // Starting initialization #################################################################################################################################################
    void Start() {

        cached_transform = transform;

        ship = GetComponent<Ship>();
        shields = GetComponentsInChildren<Protection>( true );
        sound_effects = GetComponentInParent<SoundEffects>();

        for( int i = 0; i < shields.Length; i++ ) shields[i].DisableAutoProtection();

        if( ship.Shield_time.Available == ship.Shield_time.Maximum ) is_ready = true;

        // Если корабль используется в рекламе, ссылка на Canvas у него будет отсутствовать
        charge_time = 1f / ((GetComponentInParent<Player>() != null) ? Game.Canvas.Charge_refresh_speed : 1f);
        protect_time = 1f / ((GetComponentInParent<Player>() != null) ? Game.Canvas.Protect_refresh_speed : 1f);

        charge_wait_for_seconds = new WaitForSeconds( charge_time );
        protect_wait_for_seconds = new WaitForSeconds( protect_time );
    }

    // Charge the shiled #######################################################################################################################################################
    IEnumerator Charge() {

        float last_time = Time.time;
        float shield_rate = ship.Shield_time.Maximum / ship.Charge_time.Maximum;

        ship.Charge_time.Available = 0f;

        while( !is_ready ) {

            // В режиме паузы, а также при отсутствии топлива (например, находясь на станции), защита не должна восстанавливаться
            if( !Game.Is( GameState.Paused ) && (ship.Fuel_capacity.Available > 0f) ) {

                Game.Canvas.RefreshShieldIndicator( false );

                ship.Charge_time.Available += (Time.time - last_time);
                ship.Shield_time.Available = ship.Charge_time.Available * shield_rate;

                Game.Player.CalculateFuelReserve( charge_time, ship.Fuel_charge_usage * ship.Shield_power.Available * ship.Shield_power.Upgrade_max_game_inversed );

                if( ship.Charge_time.Available >= ship.Charge_time.Maximum ) {

                    ship.Shield_time.Available = ship.Shield_time.Maximum;
                    ship.Charge_time.Available = ship.Charge_time.Maximum;

                    is_ready = true;
                }
            }

            last_time = Time.time;

            yield return charge_wait_for_seconds;
        }

        if( !Game.Input_control.Control_is_disabled ) Game.Canvas.RefreshShieldIndicator( true );
        else Game.Canvas.RefreshShieldIndicator( false );

        yield break;
    }

    // Protect the ship ########################################################################################################################################################
    IEnumerator Protect() {

        float last_time = Time.time;

        ship.Shield_time.Available = ship.Shield_time.Maximum;

        while( is_active ) {

            // В режиме паузы защита не должна расходоваться
            if( !Game.Is( GameState.Paused ) ) {

                Game.Canvas.RefreshShieldIndicator( false );

                ship.Shield_time.Available -= (Time.time - last_time);

                if( ship.Shield_time.Available <= 0f ) {

                    ship.Shield_time.Available = 0f;
                    ship.Charge_time.Available = 0f;

                    if( is_active ) DeactivateProtection();
                }
            }

            last_time = Time.time;

            yield return protect_wait_for_seconds;
        }

        Game.Canvas.RefreshShieldIndicator( false );

        yield break;
    }
    
    // Activates protection's shield ###########################################################################################################################################
    public void ActivateProtection() {

        is_ready = false;
        is_active = true;

        if( sound_effects != null ) sound_effects.PlayOn();

        if( shields != null ) for( int i = 0; i < shields.Length; i++ ) shields[i].Enable();

        StartCoroutine( Protect() );
    }

    // Deactivates protection's shield #########################################################################################################################################
    public void DeactivateProtection() {

        is_ready = false;
        is_active = false;

        if( sound_effects != null ) sound_effects.PlayOff();

        if( shields != null ) for( int i = 0; i < shields.Length; i++ ) shields[i].Disable();

        StartCoroutine( Charge() );
    }
}