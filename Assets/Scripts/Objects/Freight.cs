using UnityEngine;

// Груз - это ценность (Value), которая имеет контейнер; внутри контейнера могут находиться различные виды товаров
[RequireComponent( typeof( ObstacleControl ) )]
public class Freight : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Тип (конкретное название) груза" )]
    private FreightType type = FreightType.Unknown;
    public FreightType Type { get { return type; } }

    [SerializeField]
    [Tooltip( "Материал для данного типа груза" )]
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