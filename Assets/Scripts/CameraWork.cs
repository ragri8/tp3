using UnityEngine;

public class CameraWork : MonoBehaviour {
    
    [Tooltip("The height we want the camera to be above the target")] [SerializeField]
    private float height = 200.0f;

    private int xmax = 245;//limite de mouvement de la camera pour la bloqué sur les bords
    private int zmax = 180;
    
    Transform cameraTransform;//transform de la camera
    bool isFollowing;//indique si la camera doit suivre le joueur
    
    void Start() {
    }

    void LateUpdate() {
        if (cameraTransform == null && isFollowing) {
            OnStartFollowing();
        }

        if (isFollowing) {
            Apply();//modification de la position de la camera pour suivre le joueur
        }
    }

    
    public void OnStartFollowing() {
        cameraTransform = Camera.main.transform;
        isFollowing = true;
        Apply();
    }
    
    void Apply() {
        Vector3 targetCenter = transform.position;
        targetCenter.x = (targetCenter.x < -xmax) ? -xmax : targetCenter.x;//on applique les limites de mouvement de la camera
        targetCenter.x = (targetCenter.x > xmax) ? xmax : targetCenter.x;
        targetCenter.z = (targetCenter.z < -zmax) ? -zmax : targetCenter.z;
        targetCenter.z = (targetCenter.z > zmax) ? zmax : targetCenter.z;
        
        float currentHeight = height;//recuperation de la hauteur de la camera qui ne change pas
        cameraTransform.position = new Vector3(targetCenter.x, currentHeight, targetCenter.z);//modification de la position
        
    }
}
