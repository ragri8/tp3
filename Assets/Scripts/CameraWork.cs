using UnityEngine;

public class CameraWork : MonoBehaviour {
    
    [Tooltip("The height we want the camera to be above the target")] [SerializeField]
    private float height = 10;
    [Tooltip("Distance of the camera behind the target")] [SerializeField]
    private float CAMERA_DISTANCE = 12;
    [Tooltip("Angle of the camera projection")] [SerializeField]
    private float CAMERA_ANGLE = 25;
    private Transform playerTransform;

    private int xmax = 245;//limite de mouvement de la camera pour la bloqué sur les bords
    private int zmax = 180;
    
    Transform cameraTransform;//transform de la camera
    bool isFollowing;//indique si la camera doit suivre le joueur
    private bool cameraSet = false;

    private void Awake() {
        playerTransform = transform;
    }

    void Start() {
    }

    void LateUpdate() {
        if (isFollowing && cameraSet) {
            Apply();//modification de la position de la camera pour suivre le joueur
            return;
        }
        
        if (cameraTransform == null && isFollowing) {
            OnStartFollowing();
        }
    }

    
    public void OnStartFollowing() {
        cameraTransform = Camera.main.transform;
        cameraTransform.rotation = Quaternion.Euler(CAMERA_ANGLE, 0, 0);
        isFollowing = true;
        cameraSet = true;
        Apply();
    }
    
    void Apply() {
        var heightVec = Vector3.up * (height + playerTransform.lossyScale.y);
        
        cameraTransform.rotation = Quaternion.Euler(CAMERA_ANGLE, playerTransform.eulerAngles.y, 0);
        cameraTransform.position = playerTransform.position + playerTransform.forward * (- CAMERA_DISTANCE) + heightVec;
    }
}
