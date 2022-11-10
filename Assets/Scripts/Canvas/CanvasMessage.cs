using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanvasMessage : MonoBehaviour {

    [SerializeField]
    private EffectiveText text_message;

    private List<ComplexMessage> messages = new List<ComplexMessage>();

    private ComplexMessage current_message = null;

    private AnimationColorAlpha message_animation;

    private const float check_time = 0.5f;

    private WaitForSeconds message_wait_for_seconds = new WaitForSeconds( check_time );

    // Starting initialization #################################################################################################################################################
    void Start() {

        message_animation = text_message.Text_component.GetComponent<AnimationColorAlpha>();

        StartCoroutine( CheckMessageTime() );
    }
  
    // Check a time for deactivation message ###################################################################################################################################
    IEnumerator CheckMessageTime() {

        while( !Game.Is( GameState.Complete ) ) {

            if( current_message != null ) {

                current_message.Usage_time += check_time;
                if( (current_message.Usage_time >= current_message.Max_time) && !Game.Control.IsPlayingVoice( current_message ) ) RemoveCurrentMessage();
            }

            if( (current_message == null) && (messages.Count > 0) ) {

                current_message = messages[0];
                ActivateCurrentMessage();
            }

            yield return message_wait_for_seconds;
        }

        yield break;
    }

    // Show the current message ################################################################################################################################################
    void ActivateCurrentMessage() {

        if( current_message == null ) return;

        if( current_message.Text_key != null ) {

            message_animation.enabled = false;

            text_message.Rewrite( Game.Localization.GetTextValue( current_message.Text_key ) );
            text_message.SetColor( current_message.Color );
            text_message.SetActive( true );

            message_animation.enabled = current_message.Use_blinking;
        }

        Game.Control.PlayAudioMessage( current_message );
    }

    // Hide the current message ################################################################################################################################################
    void RemoveCurrentMessage() {

        if( current_message == null ) return;

        Game.Control.StopAudioMessage( current_message );

        message_animation.enabled = false;
        text_message.SetActive( false );

        messages.Remove( current_message );
        current_message = null;
    }

    // Add the message to the messages' list ###################################################################################################################################
    public void Show( ComplexMessage new_message ) {

        if( (new_message == null) || messages.Contains( new_message ) ) return;

        new_message.Usage_time = 0f;

        messages.Add( new_message );

        if( messages.Count > 1 ) messages.Sort();
    }

    // Remove the message from the messages' list ##############################################################################################################################
    public void Hide( ComplexMessage old_message ) {

        if( (old_message == null) || !messages.Contains( old_message ) ) return;

        if( old_message == current_message ) RemoveCurrentMessage();
        else messages.Remove( old_message );
    }
}
