using Manager.GameInstance;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
    private PlayerManager target;

    [Tooltip("Pixel offset from the player target")] [SerializeField]
    private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    [Tooltip("UI Text to display Player's Name")] [SerializeField]
    private Text playerNameText;

    Transform targetTransform;
    Renderer targetRenderer;
    CanvasGroup _canvasGroup;
    Vector3 targetPosition;

    public void SetTarget(PlayerManager target) {
        if (target == null) {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }

        // Cache references for efficiency
        this.target = target;
        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponent<Renderer>();
    }

    void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
        transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }
    
    void Update() {
        if (target == null) {
            Destroy(gameObject);
        }
    }

    void LateUpdate() {
// Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
        if (targetRenderer != null) {
            _canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        }
        // #Critical
        // Follow the Target GameObject on screen.
        if (targetTransform != null) {
            targetPosition = targetTransform.position;
            transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }
    }
}
