using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Core;
using Game.Player;
using Game.Units;
using Game.Scriptables;

namespace Game.AI
{
    public class AIController : MonoBehaviour
    {
        public int aiID;
        public Castle castle;
        public List<Unit> activeUnits = new List<Unit>();
        private UnitData selectedUnitData;
        private Coroutine spawnRoutine;

        [Header("Default Unit")]
        public UnitData defaultUnit; // The default unit AI will spawn

        [Header("Attack Logic")]
        public List<Castle> enemyCastles;
        private Castle targetCastle;

        private float decisionInterval = 5f;

        private void Start()
        {
            if (defaultUnit != null)
            {
                SelectUnit(defaultUnit);
            }

            StartCoroutine(AIBehaviorLoop()); // Start AI decision loop
        }

        /// <summary>
        /// Spawns a unit at the AI's castle spawn point.
        /// </summary>
        private void SpawnUnit()
        {
            if (selectedUnitData == null || GameManager.Instance.isGameOver) return;

            Vector3 spawnPosition = castle.spawnPoint.position;
            Quaternion spawnRotation = Quaternion.identity;

            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
            spawnPosition += randomOffset;

            GameObject newUnit = ObjectPooler.Instance.GetUnit(selectedUnitData.unitType, spawnPosition);
            newUnit.transform.position = spawnPosition;
            newUnit.transform.rotation = spawnRotation;

            if (newUnit.TryGetComponent<Unit>(out Unit unit))
            {
                unit.Initialize(aiID);
                activeUnits.Add(unit);
            }
        }

        public void SelectUnit(UnitData newUnitData)
        {
            if (newUnitData == null || GameManager.Instance.isGameOver)
                return;

            selectedUnitData = newUnitData;

            if (spawnRoutine != null)
                StopCoroutine(spawnRoutine);

            spawnRoutine = StartCoroutine(SpawnSelectedUnitContinuously());
        }

        private IEnumerator SpawnSelectedUnitContinuously()
        {
            while (!GameManager.Instance.isGameOver && selectedUnitData != null)
            {
                SpawnUnit();
                yield return new WaitForSeconds(selectedUnitData.spawnTime);
            }
        }

        /// <summary>
        /// AI decision-making loop: decides between attack or defense.
        /// </summary>
        private IEnumerator AIBehaviorLoop()
        {
            while (!GameManager.Instance.isGameOver)
            {
                yield return new WaitForSeconds(decisionInterval);

                if (activeUnits.Count == 0)
                    continue; // Skip if AI has no units

                if (activeUnits.Count >= 4)
                {
                    // If AI has enough units, start attacking a random castle
                    if (targetCastle == null || !targetCastle.IsAlive())
                    {
                        SelectRandomTarget();
                    }

                    if (targetCastle != null)
                    {
                        AttackTarget();
                    }
                }
                else if (IsUnderAttack()) // If under attack, defend
                {
                    MoveUnitsInFormation(castle.transform.position);
                }
            }
        }

        /// <summary>
        /// Selects a random enemy castle to attack.
        /// </summary>
        private void SelectRandomTarget()
        {
            List<Castle> aliveEnemyCastles = enemyCastles.FindAll(castle => castle.IsAlive());

            if (aliveEnemyCastles.Count > 0)
            {
                targetCastle = aliveEnemyCastles[Random.Range(0, aliveEnemyCastles.Count)];
                Debug.Log($"⚔ AI has selected {targetCastle.ownerName}'s castle as the target!");
            }
        }

        /// <summary>
        /// Sends AI units to attack the selected target castle.
        /// </summary>
        private void AttackTarget()
        {
            if (targetCastle == null) return;

            Debug.Log($"🔥 AI is attacking {targetCastle.ownerName}'s castle!");

            // Ensure that AI moves to the castle and attacks immediately
            MoveUnitsInFormation(targetCastle.transform.position, true);
        }

        /// <summary>
        /// Checks if AI's castle is under attack.
        /// </summary>
        private bool IsUnderAttack()
        {
            Collider[] enemies = Physics.OverlapSphere(castle.transform.position, 10f, LayerMask.GetMask("Unit"));
            return enemies.Length > 3; // If more than 3 enemy units are near, AI defends
        }

        /// <summary>
        /// Moves AI units in formation towards the target position.
        /// If the target is an enemy castle, units will attack immediately upon arrival.
        /// </summary>
        private void MoveUnitsInFormation(Vector3 targetPosition, bool isTargetCastle = false)
        {
            if (activeUnits.Count == 0) return;

            List<Vector3> formationPositions = (activeUnits.Count > 5)
                ? GetGridFormation(targetPosition, activeUnits.Count)
                : GetFormationPositions(targetPosition, activeUnits.Count);

            for (int i = 0; i < activeUnits.Count; i++)
            {
                if (activeUnits[i] != null)
                {
                    activeUnits[i].MoveTo(formationPositions[i]);

                    // If target is a castle, set it as the attack target immediately
                    if (isTargetCastle)
                    {
                        activeUnits[i].SetTargetCastle(targetCastle);
                    }
                }
            }
        }

        /// <summary>
        /// Generates a circular formation around the target position.
        /// </summary>
        private List<Vector3> GetFormationPositions(Vector3 targetPosition, int unitCount)
        {
            List<Vector3> positions = new List<Vector3>();
            float radius = 2.0f;
            float angleStep = 360f / unitCount;

            for (int i = 0; i < unitCount; i++)
            {
                float angle = i * angleStep;
                float x = targetPosition.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float z = targetPosition.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                positions.Add(new Vector3(x, targetPosition.y, z));
            }

            return positions;
        }

        /// <summary>
        /// Generates a grid formation around the target position.
        /// </summary>
        private List<Vector3> GetGridFormation(Vector3 targetPosition, int unitCount, int rowSize = 3, float spacing = 0.5f)
        {
            List<Vector3> positions = new List<Vector3>();

            int rows = Mathf.CeilToInt((float)unitCount / rowSize);
            int cols = Mathf.Min(unitCount, rowSize);
            int index = 0;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (index >= unitCount) break;

                    float x = targetPosition.x + (c - (cols / 2)) * spacing;
                    float z = targetPosition.z + (r - (rows / 2)) * spacing;
                    positions.Add(new Vector3(x, targetPosition.y, z));

                    index++;
                }
            }
            return positions;
        }
    }
}
