using UnityEngine;
using System.Collections.Generic;

public class JetCollision : MonoBehaviour {

    [SerializeField]
    [Range( 1, 5 )]
    [Tooltip( "Количество коллизий, анализируемых при столкновении струи двигателя с внешним объектом: по умолчанию = 1" )]
    private int max_collisions = 1;

    private ParticleSystem particle_system;
    private List<ParticleCollisionEvent> events_list;

    private ObstacleControl obstacle;

    private bool is_ship = false;

	// Use this for initialization #############################################################################################################################################
	void Start () {

        events_list = new List<ParticleCollisionEvent>();
        particle_system = GetComponent<ParticleSystem>();	

        is_ship = (GetComponentInParent<Ship>() != null ) ? true : false;
    }

    // Processing of particle collision ########################################################################################################################################
    void OnParticleCollision( GameObject collision_object ) {

        particle_system.GetCollisionEvents( collision_object, events_list );
        if( events_list.Count < 1 ) return;

        obstacle = collision_object.GetComponent<ObstacleControl>();

        for( int i = 0, size = Mathf.Min( events_list.Count, max_collisions ); i < size; i++ ) {
            
            // Создаём эффект столкновения струи двигателя с поверхностью
            if( ((Time.time - obstacle.Contact_sum_time) >= obstacle.Contact_delta_time) && (obstacle.Contact_prefab != null) ) {

                obstacle.Contact_sum_time = Time.time;
                Game.Effects_control.Show( obstacle.Jet_contact_prefab, events_list[i].intersection, (collision_object.CompareTag( "Ship" ) ? true : Game.Use_sound_in_vacuum) );
            }
            
            // Если тело не представляет собой препятствие, обрабатываем следующую коллизию
            if( obstacle == null ) continue;

            // Создаём эффект реакции поверхности на столкновение струи двигателя с ней
            if( ((Time.time - obstacle.Reaction_sum_time) >= obstacle.Reaction_delta_time) && (obstacle.Jet_reaction_prefab != null) ) {

                obstacle.Reaction_sum_time = Time.time;
                Game.Effects_control.Show( obstacle.Jet_reaction_prefab, events_list[i].intersection, (collision_object.CompareTag( "Ship" ) ? true : Game.Use_sound_in_vacuum) );
            }

            // Если объект находитс под защитой, вызываем эффекты защиты
            if( obstacle.Is_protection && obstacle.Protection.enabled ) obstacle.Protection.ShowLongHitEffect( events_list[i].intersection, (collision_object.CompareTag( "Ship" ) ? true : Game.Use_sound_in_vacuum) );

            // Если объект представляет собой нагреваемое тело, раскаляем его струёй от двигателей
            else if( obstacle.Is_space_body ) obstacle.Space_body.OnHitCollision( events_list[i].intersection, is_ship );
        }
        
        // Очистка списка коллизий после цикла обработки коллизий
        events_list.Clear();
    }
}
