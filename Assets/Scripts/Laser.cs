using UnityEngine;

public class Laser : MonoBehaviour {
    private int speed = 300;
    private int largeur = 500; //dimensions de l'arene avec de la marge pour les supprimers une fois sortis de celle-c
    private int hauteur = 350;
    public GameManager game; //recuperation du GameManager poour savoir lorsqu'on est en pause
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (!game.paused) { //seul le proprietaire du laser peut le deplacer et on test si le jeu n'est pas en pause
            transform.Translate	(speed * Time.deltaTime * Vector3.forward);
            if (transform.position.x < -largeur || transform.position.x > largeur || transform.position.z < -hauteur ||
                transform.position.z > hauteur) { //lorsque le laser sort de l'arene on lui laisse un peut de marge puis on le supprime 
                hit();
            }
        }
    }

    public void hit() {
        Destroy(gameObject);
    }
}
