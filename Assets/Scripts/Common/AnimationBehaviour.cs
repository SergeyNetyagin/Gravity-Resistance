using UnityEngine;

public enum AnimationMode {

    Once,
	Loop,
	Pingpong,
    Stopped
}

[System.Serializable]
public class AnimationBehaviour {

    [SerializeField]
    private AnimationMode mode = AnimationMode.Loop;
    public AnimationMode Mode { get { return mode; } }
    public void SetMode( AnimationMode mode ) { this.mode = mode; }

    [SerializeField]
    [Tooltip( "ƒл€ параметров, имеющих фиксированное максимальное значение, лучше назначать кривую с абсолютными значени€ми; в других случа€х кривой можно задавать произвольные значени€ и регулировать результирующий эффект скоростью" )]
    private AnimationCurve curve;
    public AnimationCurve Curve { get { return curve; } }
    public void SetCurve( AnimationCurve curve ) { this.curve = curve; }
    public bool Has_curve { get { return (curve.length > 0); } }

    [SerializeField]
    [Tooltip( "—корость, определ€юща€ местонахождени€ точки на кривой; служит дл€ регулировки скорости эффекта; по умолчанию равна 1" )]
    [Range( -100f, 100f )]
    private float speed = 1f;
    public float Speed { get { return speed; } }
    public void SetSpeed( float speed ) { this.speed = speed; }

    private float full_time = 0f;
    private float curve_time = 0f;

    public void Reset() { full_time = curve_time = 0f; }
    public int Length { get { return (curve == null) ? 0 : curve.length; } }
    public bool Is_stopped { get { return (mode == AnimationMode.Stopped); } }

    // ƒлительность одного цикла кривой ########################################################################################################################################
    private float duration = float.MaxValue;
    public float Duration { get {

        if( duration != float.MaxValue ) return duration;

        if( (curve != null) && (curve.length > 1) ) duration = curve.keys[curve.length-1].time;
        else duration = 0f;

        return duration;
    } }

    // ¬озвращает среднюю скорость кривой в сочетании со скоростью #############################################################################################################
    private float average_speed = float.MaxValue;
    public float Average_speed { get {

        if( average_speed != float.MaxValue ) return average_speed;

        int length = (curve == null) ? 0 : curve.length;

        average_speed = 0f;
        for( int i = 0; i < length; i++ ) average_speed += curve.keys[i].value;
        if( length != 0 ) average_speed /= length;
        average_speed *= speed;

        return average_speed;
    } }

    // Evaluate current value directly #########################################################################################################################################
    public float Evaluate( float delta_time ) {

        // ѕроверка выполн€етс€ об€зательно через свойство, чтобы гарантировать правильное определение длительности
        if( Duration == 0f ) return 0f;

        full_time += delta_time * speed;

		switch( mode ) {

		    case AnimationMode.Once:

			    if( (curve_time = full_time) >= duration ) mode = AnimationMode.Stopped;
			    break;

            case AnimationMode.Loop:

			    curve_time = Mathf.Repeat( full_time, duration );
			    break;

            case AnimationMode.Pingpong:

			    curve_time = Mathf.PingPong( full_time, duration );
			    break;

            default:

                curve_time = duration;
                break;
        }

        return (mode == AnimationMode.Stopped) ? 0f : curve.Evaluate( curve_time );
    }
}