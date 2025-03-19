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
            if (agent == null || !agent.isOnNavMesh) return;

            currentState?.UpdateState(this);

            // Eðer mevcut hedef ölü ya da yoksa yeni bir hedef bul
            if (currentTarget == null || !currentTarget.IsAlive())
            {
                currentTarget = FindClosestEnemy();
            }

            // Eðer düþman bulduysa ona saldýr ya da yaklaþ
            if (currentTarget != null)
            {
                float distance = Vector3.Distance(transform.position, ((MonoBehaviour)currentTarget).transform.position);

                if (distance <= unitData.attackRange)
                {
                    ChangeState(new AttackState(currentTarget, false)); // Yakýn dövüþ
                }
                else if (unitData.unitType == UnitType.Archer && distance <= unitData.rangedAttackRange)
                {
                    ChangeState(new AttackState(currentTarget, true)); // Okçu uzak mesafeden saldýrabilir
                }
                else if (distance <= unitData.awarenessRange)
                {
                    ChangeState(new MoveState(((MonoBehaviour)currentTarget).transform.position)); // Eðer fark ettiyse yaklaþ
                }
            }
            else
            {
                Debug.Log("No Target");
                ChangeState(new IdleState()); // Eðer düþman yoksa boþa hareket etmesin
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
            currentState?.ExitState();
            currentState = newState;
            currentState.EnterState(this);

            // Enable NavMeshAgent only if in MoveState
            agent.enabled = currentState is MoveState;
        }


        /// <summary>
        /// Finds the closest enemy unit or castle within attack range.
        /// </summary>
        private IDamageable FindClosestEnemy()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, unitData.awarenessRange, LayerMask.GetMask("Unit", "Castle"));
            IDamageable closestTarget = null;
            float minDistance = float.MaxValue;

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
                    float distance = Vector3.Distance(transform.position, ((MonoBehaviour)target).transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestTarget = target;
                    }
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
