using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI {
    public class AIBehaviour {
        private static float DETECTION_RANGE = 40f;
        private static float DETECTION_LOST_RANGE = 50f;
        private static float MINIMUM_POI_RANGE = 30f;
        private static float MINIMUM_PRECISION_ANGLE = 3.0f;
        private static float APPROXIMATION_FRONT_COEFFICIENT = 0.25f;
        private static float REFLEXES = 0.5f;
        private static float MINIMUM_ATTACKING_RANGE = 3.0f;
        
        private readonly List<GameObject> _players;
        private readonly Transform _current;
        public ActionState _actionState;
        private Vector3 _pointOfInterest;
        private Transform _target;
        private bool _targetActive;
        private float _actionCooldownTime;
        private float _reflexCooldownTime;
        private float _cannonCooldownTime;
        private MovementRotationState _movementRotationState;
        private MovementTranslationState _movementTranslationState;
        
        public bool isAttacking;

        public AIBehaviour(GameObject gameObject) {
            _current = gameObject.transform;
            _players = new List<GameObject>(GameObject.FindGameObjectsWithTag(Global.PLAYER_TAG));
            changeActionState(ActionState.WAITING);
            _movementRotationState = MovementRotationState.NONE;
            _movementTranslationState = MovementTranslationState.NONE;
            _targetActive = false;
            _actionCooldownTime = 0f;
            _reflexCooldownTime = 0f;
            isAttacking = false;
            _cannonCooldownTime = 0f;
            _pointOfInterest = _current.position;
        }

        // update is called once per frame
        public void update() {
            var timelapse = Time.deltaTime;
            playStateAction(timelapse);
            updateMovementState();
        }

        public MovementRotationState getRotationState() {
            return _movementRotationState;
        }

        public MovementTranslationState getTranslationState() {
            return _movementTranslationState;
        }

        private void updateMovementState() {
            if (_actionState == ActionState.WANDERING) {
                movementAdjustmentToObjective(_pointOfInterest);
            } else if (_actionState == ActionState.APPROACHING || _actionState == ActionState.ATTACKING) {
                // TODO: change movement if attacking generate a new collider
                var target = _target.position;
                movementAdjustmentToObjective(target);
            }
        }

        private void playStateAction(float timelapse) {
            _actionCooldownTime -= timelapse;
            if (_actionCooldownTime > 0) {
                resumeStateAction();
            } else {
                completeStateAction();
            }
        }

        private void resumeStateAction() {
            switch (_actionState) {
                case ActionState.WANDERING:
                    resumeWanderingAction();
                    break;
                case ActionState.APPROACHING:
                    resumeApproachAction();
                    break;
                case ActionState.ATTACKING:
                    resumeAttackingAction();
                    break;
            }
        }

        // resume action when AI is wandering between points of interests, looking for an enemy
        private void resumeWanderingAction() {
            searchTarget();
            if (_targetActive) {
                changeActionState(ActionState.APPROACHING);
                _actionCooldownTime = 1f;
                _cannonCooldownTime = Math.Max(_cannonCooldownTime, REFLEXES);
            } else {
                if (Vector3.Distance(_current.position, _pointOfInterest) < MINIMUM_POI_RANGE) {
                    _actionCooldownTime = 0f;
                }
            }
        }

        private void resumeApproachAction() {
            if (Vector3.Distance(_current.position, _target.position) > DETECTION_LOST_RANGE) {
                _actionCooldownTime = 0f;
            } else if (Vector3.Distance(_current.position, _target.position) <= MINIMUM_ATTACKING_RANGE) {
                _actionCooldownTime = 0f;
            }
        }

        private void resumeAttackingAction() {
            if (Vector3.Distance(_current.position, _target.position) > MINIMUM_ATTACKING_RANGE) {
                _actionCooldownTime = 0f;
            }
        }

        private void completeStateAction() {
            switch (_actionState) {
                case ActionState.WAITING:
                    completeWaitingAction();
                    break;
                case ActionState.WANDERING:
                    completeWanderingAction();
                    break;
                case ActionState.APPROACHING:
                    completeApproachAction();
                    break;
                case ActionState.ATTACKING:
                    completeAttackingAction();
                    break;
            }
        }

        private void completeWaitingAction() {
            searchTarget();
            if (_targetActive) {
                changeActionState(ActionState.APPROACHING);
                _actionCooldownTime = 1f;
                _cannonCooldownTime = Math.Max(_cannonCooldownTime, REFLEXES);
            } else {
                changeActionState(ActionState.WANDERING);
                _actionCooldownTime = 4f;
                _pointOfInterest = generatePointOfInterest();
            }
        }

        private void completeWanderingAction() {
            changeActionState(ActionState.WAITING);
            _actionCooldownTime = Random.Range(1f, 3f);
        }

        private void completeApproachAction() {
            var targetDistance = Vector3.Distance(_current.position, _target.position);
            if (targetDistance > DETECTION_RANGE) {
                changeActionState(ActionState.WAITING);
                _actionCooldownTime = Random.Range(1f, 3f);
            } else if (targetDistance > MINIMUM_ATTACKING_RANGE) {
                _actionCooldownTime = 1.0f;
            } else {
                isAttacking = true;
                changeActionState(ActionState.ATTACKING);
                _actionCooldownTime = 1.0f;
            }
        }

        private void completeAttackingAction() {
            var targetDistance = Vector3.Distance(_current.position, _target.position);
            if (targetDistance > DETECTION_RANGE) {
                isAttacking = false;
                changeActionState(ActionState.WAITING);
                _actionCooldownTime = Random.Range(1f, 3f);
            } else if (targetDistance > MINIMUM_ATTACKING_RANGE) {
                isAttacking = false;
                changeActionState(ActionState.APPROACHING);
                _actionCooldownTime = 1.0f;
            } else {
                _actionCooldownTime = 1.0f;
            }
        }

        private void searchTarget() {
            var closestUnit = _current;
            var closestDistance = float.MaxValue;
            foreach (var player in _players) {
                var playerTr = player.transform;
                var playerDistance = Vector3.Distance(_current.position, playerTr.position);
                if (playerDistance < DETECTION_RANGE) {
                    if (playerDistance < closestDistance) {
                        closestUnit = playerTr;
                        closestDistance = playerDistance;
                    }
                }
            }
            if (closestDistance < float.MaxValue) {
                setTarget(closestUnit);
            } else {
                _targetActive = false;
            }
        }

        private Vector3 generatePointOfInterest() {
            // TODO: use gamegrid for POI research
            var distance = Random.Range(DETECTION_RANGE % 2, DETECTION_RANGE);
            var degreeVariation = Random.Range(-90, 90);
            var currentPosition = _current.position;
            var orientation = Util.toRad(_current.eulerAngles.y + degreeVariation);
            return new Vector3(
                currentPosition.x - distance * (float)Math.Sin(orientation),
                currentPosition.y,
                currentPosition.z - distance * (float)Math.Cos(orientation));
        }

        private void setTarget(Transform newTarget) {
            _target = newTarget;
            _targetActive = true;
        }

        private void forceResetTarget() {
            _targetActive = false;
            _target = null;
            if (_actionState != ActionState.WAITING && _actionState != ActionState.WANDERING) {
                changeActionState(ActionState.WAITING);
                _actionCooldownTime = Random.Range(1f, 3f);
            }
        }

        private void movementAdjustmentToObjective(Vector3 objective) {
            var angle = alignmentToObjective(objective);
            translationToObjective(objective, angle);
        }

        private double alignmentToObjective(Vector3 objective) {
            var currentPosition = _current.position;
            var relativeAngle = Util.angleBetweenVec(
                currentPosition.x,
                objective.x,
                currentPosition.z,
                objective.z);
            relativeAngle -= _current.rotation.eulerAngles.y;
            if (relativeAngle < -180) {
                relativeAngle += 360;
            } else if (relativeAngle > 180) {
                relativeAngle -= 360;
            }
            if (_reflexCooldownTime > 0) {
                _movementRotationState = MovementRotationState.NONE;
            } else if (relativeAngle > MINIMUM_PRECISION_ANGLE / 2) {
                _movementRotationState = MovementRotationState.LEFT;
            } else if (relativeAngle < -MINIMUM_PRECISION_ANGLE / 2) {
                _movementRotationState = MovementRotationState.RIGHT;
            } else {
                _movementRotationState = MovementRotationState.NONE;
            }
            return relativeAngle;
        }

        private void translationToObjective(Vector3 objective, double relativeAngle) {
            if (Math.Abs(relativeAngle) > 45) {
                _movementTranslationState = MovementTranslationState.SLOW;
            } else {
                _movementTranslationState = MovementTranslationState.FORWARD;
            }
        }

        private void changeActionState(ActionState newState) {
            _actionState = newState;
        }

        public void removePlayer(GameObject player) {
            _players.Remove(player);
            if (_targetActive && player.transform.Equals(_target)) {
                forceResetTarget();
            }
        }
    }
}
