using System.Collections;
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
    [SerializeField] private Image background;
    [SerializeField] private Image fill;
    private bool lowHealth = false;
    private Coroutine flashingBar;
    private float flashTimer = 0.0f;
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
    public void Update()
    {
        playerHealthSlider.value = _target.health; //la valeur du slider est egale à celle du joueur
        if (playerHealthSlider.value == 3 && lowHealth == false)
        {
            lowHealth = true;
            flashingBar = StartCoroutine(criticalHealth());
        }
        else if (playerHealthSlider.value == 0)
        {
            StopCoroutine(flashingBar);
            background.color = Color.black;
            fill.color = Color.black;
        }
    }

    private IEnumerator criticalHealth()
    {
        while (true)
        {
            flashTimer += Time.deltaTime;
            if (flashTimer >= 1.0f)
            {
                flashTimer = 0.0f;
            }
            if ((0.0f < flashTimer && flashTimer < 0.20f) || (0.40f < flashTimer && flashTimer < 0.60f))
            {
                background.color = Color.red;
            }
            else
            {
                background.color = Color.black;
            }

            yield return null;
        }
    }
}
