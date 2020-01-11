using AI;
using UnityEngine;


namespace Manager.GameInstance {
    public class LauncherBotManager : MonoBehaviour {
        private static float MAX_SPEED = 7.0f;
        private static float ROTATION_SPEED = 50.0f;

        private GameObject LocalPlayerInstance;
        [SerializeField] private AudioSource scream;
        [SerializeField] private AudioSource moan;
        private AILauncherSceneBehaviour aiBehaviour;
        private Animator anim;
        private float _speed;
        private bool isAttacking = false;

        void Awake() {
            anim = GetComponent<Animator>();
            LocalPlayerInstance = gameObject;
            aiBehaviour = new AILauncherSceneBehaviour(gameObject);
            moan.volume = (1 / 100f) * PlayerPrefs.GetInt("sfxVolume", 100);
            scream.volume = (1 / 100f) * PlayerPrefs.GetInt("sfxVolume", 100);
            moan.Play();
        }

        void Update() {
            aiBehaviour.update();
            ProcessInputs();
        }

        void ProcessInputs() {
            if (aiBehaviour.getTranslationState() == MovementTranslationState.FORWARD) {
                _speed = MAX_SPEED;
                //moan.Play();
            } else if (aiBehaviour.getTranslationState() == MovementTranslationState.SLOW) {
                _speed = MAX_SPEED;
            } else {
                _speed = 0.0f;
            }

            anim.SetFloat("speed", _speed);

            if (aiBehaviour.getRotationState() == MovementRotationState.LEFT) {
                transform.Rotate(0, ROTATION_SPEED * Time.deltaTime * 1, 0);
            } else if (aiBehaviour.getRotationState() == MovementRotationState.RIGHT) {
                transform.Rotate(0, ROTATION_SPEED * Time.deltaTime * -1, 0);
            }
        }
    }
}
