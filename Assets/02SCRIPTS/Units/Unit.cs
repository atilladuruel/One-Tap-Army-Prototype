using UnityEngine.AI;
using UnityEngine;
using System;
using System.Collections.Generic;
using Game.Core;
using Game.Scriptables;
using Game.Units.States;
using System.Collections;

namespace Game.Units
{
    public class Unit : MonoBehaviour
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
        private IUnitState currentState;

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
            animator = GetComponent<Animator>(); // Gettin Animator component
            agent.speed = speed;

            ChangeState(new IdleState());
        }

        private void Update()
        {
            currentState?.UpdateState(this);

            // If unit reaches its destination, switch to IdleState
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                ChangeState(new IdleState());
            }

            // If movement queue is not empty, move to next position
            if (movementQueue.Count > 0 && agent.remainingDistance <= agent.stoppingDistance)
            {
                MoveTo(movementQueue.Dequeue());
            }
        }

        public void ChangeState(IUnitState newState)
        {
            currentState?.ExitState();
            currentState = newState;
            currentState.EnterState(this);
        }

        /// <summary>
        /// Moves the unit to a target position.
        /// </summary>
        public void MoveTo(Vector3 targetPosition)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPosition, out hit, 2f, NavMesh.AllAreas))
            {
                ChangeState(new MoveState(hit.position));
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
    }
}
