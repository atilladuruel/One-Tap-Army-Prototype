using UnityEngine.AI;
using UnityEngine;
using System;
using System.Collections.Generic;
using Game.Core;
using Game.Scriptables;
using Game.Units.States;
using System.Collections;
using Game.Player;

namespace Game.Units
{

    [SelectionBase]
    public class Unit : MonoBehaviour, IDamageable
    {
        public int playerID;
        public List<Renderer> unitRenderer;
        public NavMeshAgent agent;
        public UnitData unitData;
        public Animator animator; // Animator for animations

        private Vector3 _positionTarget;
        private Queue<Vector3> _movementQueue = new Queue<Vector3>(); // Queue for formation movement

        private int _level = 1;
        private string _unitName;
        private int _health;
        private float _speed;
        private int _attackPower;
        private Castle _targetCastle;
        private IUnitState _currentState;
        private UnitAttack _unitAttack;
        private IDamageable _unitTarget;

        public event System.Action OnLevelUp; // Event to notify level up

        public int Level
        {
            get { return _level; }
            set { if (value > 0) _level = value; }
        }

        public string UnitName
        {
            get { return _unitName; }
            set { _unitName = value; }
        }

        public int Health
        {
            get { return _health; }
            set { _health = Math.Max(0, value); } // Ensure health doesn't go below 0
        }

        public float Speed
        {
            get { return _speed; }
            set { _speed = Math.Max(0, value); } // Ensure speed isn't negative
        }

        public int AttackPower
        {
            get { return _attackPower; }
            set { _attackPower = Math.Max(0, value); } // Ensure attack power isn't negative
        }

        private void Awake()
        {
            if (unitData != null) // Initialize unit stats from UnitData
            {
                _unitName = unitData.unitName;
                _health = unitData.health;
                _speed = unitData.speed;
                _attackPower = unitData.attack;
            }

            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            _unitAttack = GetComponent<UnitAttack>();
            agent.speed = _speed;

            ChangeState(new IdleState());
        }

        /// <summary>
        /// Updates the unit's state based on proximity to the target.
        /// </summary>
        private void Update()
        {


            _currentState?.UpdateState(this);

            // Eðer mevcut hedef ölü ya da yoksa yeni bir hedef bul
            if (_unitTarget == null || !_unitTarget.IsAlive())
            {
                _unitTarget = FindClosestEnemy();
            }

            if (_unitTarget != null && !(_currentState is AttackState))
            {
                float distance = Vector3.Distance(transform.position, ((MonoBehaviour)_unitTarget).transform.position);

                if (distance <= unitData.awarenessRange)
                {
                    if (!(_currentState is MoveState))
                    {
                        MoveTo(((MonoBehaviour)_unitTarget).transform.position); // Eðer fark ettiyse yaklaþ
                    }
                }
            }
        }


        private void OnDrawGizmosSelected()
        {
            if (unitData == null) return;

            // Awareness Range (Mavi)
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, unitData.awarenessRange);

            // Attack Range (Kýrmýzý)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, unitData.attackRange);

            // Ranged Attack Range (Yeþil) - Sadece Archer için
            if (unitData.unitType == UnitType.Archer)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, unitData.rangedAttackRange);
            }
        }


        /// <summary>
        /// Changes the unit's state and enables/disables NavMeshAgent accordingly.
        /// </summary>
        public void ChangeState(IUnitState newState)
        {
            _currentState?.ExitState(this);
            _currentState = newState;
            _currentState.EnterState(this);

            // Eðer MoveState deðilse, NavMeshAgent'in kapanmasýný geciktir
            if (!(_currentState is MoveState))
            {
                if (playerID == 0) Debug.Log($"Exit: {_currentState.ToString()}");
                StartCoroutine(DisableNavMeshAgentWithDelay(0.2f)); // 0.2 saniye gecikmeli kapat
            }
            else
            {
                agent.enabled = true; // MoveState içindeyken NavMeshAgent hep açýk
            }
        }

        private IEnumerator DisableNavMeshAgentWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (!(_currentState is MoveState)) // Eðer o sýrada MoveState'e geçilmemiþse kapat
            {
                agent.enabled = false;
            }
        }

        /// <summary>
        /// Finds the closest enemy unit or castle within attack range.
        /// </summary>
        private IDamageable FindClosestEnemy()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, unitData.awarenessRange, LayerMask.GetMask("Unit", "Castle"));
            IDamageable closestTarget = null;

            foreach (Collider col in colliders)
            {
                Unit targetUnit = col.GetComponent<Unit>();
                Castle targetCastle = col.GetComponent<Castle>();

                IDamageable target = null;

                if (targetUnit != null && IsEnemy(targetUnit))
                {
                    target = targetUnit;
                }
                else if (targetCastle != null && targetCastle.playerID != playerID)
                {
                    target = targetCastle;
                }

                if (target != null && target.IsAlive())
                {
                    closestTarget = target;
                }
            }

            return closestTarget;
        }


        /// <summary>
        /// Moves the unit to a target position.
        /// </summary>
        public void MoveTo(Vector3 targetPosition)
        {
            if (_unitTarget == null)
            {
                if (agent == null)
                {
                    Debug.LogError($"? {_unitName} has no NavMeshAgent!");
                    return;
                }

                if (!agent.enabled)
                {
                    Debug.LogWarning($"? {_unitName} NavMeshAgent was disabled, enabling now...");
                    agent.enabled = true;
                }

                if (agent.isOnNavMesh)
                {
                    ChangeState(new MoveState(targetPosition));
                    //Debug.Log($"?? {unitName} is moving to {targetPosition}");
                }
                else
                {
                    Debug.LogError($"? {_unitName} is NOT on a NavMesh! Cannot move.");
                }
            }
        }


        /// <summary>
        /// Moves unit in a formation by adding positions to queue.
        /// </summary>
        public void MoveInFormation(List<Vector3> formationPositions)
        {
            _movementQueue.Clear();
            foreach (Vector3 pos in formationPositions)
            {
                _movementQueue.Enqueue(pos);
            }

            if (_movementQueue.Count > 0)
            {
                MoveTo(_movementQueue.Dequeue());
            }
        }

        /// <summary>
        /// Increases the unit's level and resets XP progress.
        /// </summary>
        private void LevelUp()
        {
            _level++; // Increase level
            OnLevelUp?.Invoke(); // Notify UI or other systems

            Debug.Log($"{_unitName} leveled up to {_level}!");
        }

        /// <summary>
        /// Returns the current level of the unit.
        /// </summary>
        public int GetLevel()
        {
            return _level;
        }

        public void Initialize(int ownerID)
        {
            playerID = ownerID;
            ApplyTeamColor();
        }

        private void ApplyTeamColor()
        {
            Color teamColor = PlayerManager.Instance.GetPlayerByID(playerID).teamColor;
            foreach (var renderer in unitRenderer)
            {
                renderer.material.color = teamColor;
            }
        }

        public bool IsEnemy(Unit other)
        {
            return playerID != other.playerID; // If they have different IDs, they are enemies
        }

        public bool IsAlive()
        {
            return Health > 0;
        }

        /// <summary>
        /// Takes damage from enemy attacks.
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (_health <= 0) return; // Already dead

            _health -= damage;

            if (_health <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Plays a specific animation.
        /// </summary>
        public void PlayAnimation(string triggerName)
        {
            if (animator != null)
            {
                Debug.Log($"anim trigger: {triggerName}");
                animator.SetTrigger(triggerName); // Use SetTrigger for smooth transitions
            }
        }

        /// <summary>
        /// Handles unit death and returns to Object Pool.
        /// </summary>
        private void Die()
        {
            ChangeState(new DeathState());
            // Wait for animation to finish before deactivating
            StartCoroutine(DisableAfterDelay(2.5f));
        }

        private IEnumerator DisableAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ObjectPooler.Instance.ReturnUnit(gameObject);
        }

        /// <summary>
        /// Assigns a castle as the attack target.
        /// </summary>
        public void SetTargetCastle(Castle castle)
        {
            _targetCastle = castle;
        }

    }
}
