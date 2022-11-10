using UnityEngine;

// Минерал - это ценность (Value), которая не имеет контейнера и выглядит как обломок астероида
[RequireComponent( typeof( ObstacleControl ) )]
public class Mineral : MonoBehaviour {
 
    [SerializeField]
    [Tooltip( "Тип (конкретное название) минерала" )]
    private MineralType type = MineralType.Unknown;
    public MineralType Type { get { return type; } }

    [SerializeField]
    [Tooltip( "Материал для данного типа минерала" )]
    private Material material;

    // Awake ###################################################################################################################################################################
    void Awake() {

        if( material != null ) {

            MeshRenderer mesh = GetComponent<MeshRenderer>();
            if( mesh != null ) mesh.material = material;
        }
    }

    // Start ###################################################################################################################################################################
    void Start() {

    }
}