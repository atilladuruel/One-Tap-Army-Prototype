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
    public class Unit : MonoBehaviour, IDamageable
    {
        public bool activateStates = false;
        public int playerID;
        public List<Renderer> unitRenderer;
        public NavMeshAgent agent;
        public UnitData unitData;
        public Animator animator; // Animator for animations

        public Vector3 target;
        private Queue<Vector3> movementQueue = new Queue<Vector3>(); // Queue for formation movement

        private int level = 1;
        private string unitName;
        private int health;
        private float speed;
        private int attackPower;
        private Castle targetCastle;
        private IUnitState currentState;
        private UnitAttack unitAttack;
        private IDamageable currentTarget;

        public event System.Action OnLevelUp; // Event to notify level up

        public int Level
        {
            get { return level; }
            set { if (value > 0) level = value; }
        }

        public string UnitName
        {
            get { return unitName; }
            set { unitName = value; }
        }

        public int Health
        {
            get { return health; }
            set { health = Math.Max(0, value); } // Ensure health doesn't go below 0
        }

        public float Speed
        {
            get { return speed; }
            set { speed = Math.Max(0, value); } // Ensure speed isn't negative
        }

        public int AttackPower
        {
            get { return attackPower; }
            set { attackPower = Math.Max(0, value); } // Ensure attack power isn't negative
        }

        private void Awake()
        {
            if (unitData != null) // Initialize unit stats from UnitData
            {
                unitName = unitData.unitName;
                health = unitData.health;
                speed = unitData.speed;
                attackPower = unitData.attack;
            }

            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            unitAttack = GetComponent<UnitAttack>();
            agent.speed = speed;

            ChangeState(new IdleState());
        }

        /// <summary>
        /// Updates the unit's state based on proximity to the target.
        /// </summary>
        private void Update()
        {
            if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;

            currentState?.UpdateState(this);

            // Eðer mevcut hedef ölü ya da yoksa yeni bir hedef bul
            if (currentTarget == null || !currentTarget.IsAlive())
            {
                currentTarget = FindClosestEnemy();
            }

            if (currentTarget != null)
            {
                float distance = Vector3.Distance(transform.position, ((MonoBehaviour)currentTarget).transform.position);

                if (distance <= unitData.attackRange)
                {
                    if (!(currentState is AttackState))
                    {
                        ChangeState(new AttackState(currentTarget, false)); // Yakýn dövüþ
                    }
                }
                else if (unitData.unitType == UnitType.Archer && distance <= unitData.rangedAttackRange)
                {
                    if (!(currentState is AttackState))
                    {
                        ChangeState(new AttackState(currentTarget, true)); // Okçu uzak mesafeden saldýrabilir
                    }
                }
                else if (distance <= unitData.awarenessRange)
                {
                    if (!(currentState is MoveState))
                    {
                        ChangeState(new MoveState(((MonoBehaviour)currentTarget).transform.position)); // Eðer fark ettiyse yaklaþ
                    }
                }
            }
            else
            {
                if (!(currentState is IdleState))
                {
                    Debug.Log("No Target");
                    ChangeState(new IdleState()); // Eðer düþman yoksa IdleState’e geç
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
            currentState?.ExitState(this);
            currentState = newState;
            currentState.EnterState(this);

            // Eðer MoveState deðilse, NavMeshAgent'in kapanmasýný geciktir
            if (!(currentState is MoveState))
            {
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
            if (!(currentState is MoveState)) // Eðer o sýrada MoveState'e geçilmemiþse kapat
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
            if (agent == null)
            {
                Debug.LogError($"? {unitName} has no NavMeshAgent!");
                return;
            }

            if (!agent.enabled)
            {
                Debug.LogWarning($"? {unitName} NavMeshAgent was disabled, enabling now...");
                agent.enabled = true;
            }

            if (agent.isOnNavMesh)
            {
                agent.SetDestination(targetPosition);
                ChangeState(new MoveState(targetPosition));
                //Debug.Log($"?? {unitName} is moving to {targetPosition}");
            }
            else
            {
                Debug.LogError($"? {unitName} is NOT on a NavMesh! Cannot move.");
            }
        }


        /// <summary>
        /// Moves unit in a formation by adding positions to queue.
        /// </summary>
        public void MoveInFormation(List<Vector3> formationPositions)
        {
            movementQueue.Clear();
            foreach (Vector3 pos in formationPositions)
            {
                movementQueue.Enqueue(pos);
            }

            if (movementQueue.Count > 0)
            {
                MoveTo(movementQueue.Dequeue());
            }
        }

        /// <summary>
        /// Increases the unit's level and resets XP progress.
        /// </summary>
        private void LevelUp()
        {
            level++; // Increase level
            OnLevelUp?.Invoke(); // Notify UI or other systems

            Debug.Log($"{unitName} leveled up to {level}!");
        }

        /// <summary>
        /// Returns the current level of the unit.
        /// </summary>
        public int GetLevel()
        {
            return level;
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
            if (health <= 0) return; // Already dead

            health -= damage;

            if (health <= 0)
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
                animator.SetTrigger(triggerName); // Use SetTrigger for smooth transitions
            }
        }

        /// <summary>
        /// Handles unit death and returns to Object Pool.
        /// </summary>
        private void Die()
        {
            Debug.Log($"{unitName} has been destroyed!");

            PlayAnimation("Death"); // Play death animation before disabling

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
            targetCastle = castle;
        }

    }
}
