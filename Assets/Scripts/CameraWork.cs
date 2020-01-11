using System.Collections.Generic;
using UnityEngine;

public class CameraWork : MonoBehaviour {
    
    [Tooltip("The height we want the camera to be above the target")] [SerializeField]
    private float height = 10;
    [Tooltip("Distance of the camera behind the target")] [SerializeField]
    private float CAMERA_DISTANCE = 12;
    [Tooltip("Angle of the camera projection")] [SerializeField]
    private float CAMERA_ANGLE = 25;
    private Transform playerTransform;
    private List<Renderer> hiddenWalls;

    private Transform cameraTransform;
    private bool isFollowing;
    private bool cameraSet;

    private void Awake() {
        playerTransform = transform;
        hiddenWalls = new List<Renderer>();
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

        hideWalls();
    }

    void hideWalls() {
        foreach (var wallRenderer in hiddenWalls) {
            wallRenderer.enabled = true;
        }
        hiddenWalls.Clear();
        
        var cameraPos = cameraTransform.position;
        var playerPos = playerTransform.position;

        var direction = playerPos - cameraPos;
        var ry = new Ray(cameraPos, direction);

        var hits = Physics.RaycastAll(ry, direction.magnitude);
        //Debug.DrawRay (currentPos, direction, Color.red, 1.0f);
        foreach (var hit in hits) {
            var objectTransform = hit.transform;
            var hitObject = objectTransform.gameObject;
            //Debug.Log("Contact with " + hitObject.name);
            if (hitObject.CompareTag(Global.WALL_TAG) || hitObject.CompareTag(Global.WALL_PART_TAG)) {
                var renderer = hitObject.GetComponent<MeshRenderer>();
                renderer.enabled = false;
                hiddenWalls.Add(renderer);
            }
        }
    }
}
