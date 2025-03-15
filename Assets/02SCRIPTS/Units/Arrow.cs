using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Transform target;
    private int damage;
    public float speed = 10f;
    public float maxLifetime = 3f; // Arrow disappears after 3 seconds if no hit

    public void SetTarget(Transform enemyTarget, int arrowDamage)
    {
        target = enemyTarget;
        damage = arrowDamage;
        Destroy(gameObject, maxLifetime); // Destroy after set time to prevent memory leaks
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        if (target != null)
        {
            Unit targetUnit = target.GetComponent<Unit>();
            if (targetUnit != null)
            {
                targetUnit.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}
