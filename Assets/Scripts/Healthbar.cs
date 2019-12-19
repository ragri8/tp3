using UnityEngine;
using UnityEngine.UI;
/**
 * va servir à afficher la barre de vie du joueur en bas a gauche dans le jeu
 */
public class Healthbar : MonoBehaviour {
    private PlayerManager _target;
    [Tooltip("UI Slider to display Player's Health")]
    [SerializeField]
    private Slider playerHealthSlider;

    void Awake() {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);//on le place dans le canvas
    }
    
    public void SetTarget(PlayerManager target) {
        if (target == null) {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }
        _target = target; //recuperation du Player manager pour obtenir la variable health
    }
    public void Update() {
        playerHealthSlider.value = _target.health; //la valeur du slider est egale à celle du joueur
    }
}
