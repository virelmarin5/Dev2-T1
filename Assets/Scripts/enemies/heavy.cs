using UnityEngine;
using System.Collections;

// Winds up, then (if the player is still in range) shoves them via
// playerController.PushBack and applies damage shortly after the shove
// lands, rather than instantly.
public class enemyHeavy : enemyAI_Base
{
    [Header("Heavy Attack")]
    [SerializeField] float windupTime = 0.5f;
    [SerializeField] float pushForce = 15f;
    [SerializeField] int pushDamage = 2;
    [SerializeField] float damageDelay = 0.2f;

    bool isAttacking;

    protected override void attack()
    {
        if (isAttacking) return;
        attackTimer = 0;
        StartCoroutine(pushAttack());
    }

    IEnumerator pushAttack()
    {
        isAttacking = true;

        // telegraph the attack before it lands
        yield return new WaitForSeconds(windupTime);

        GameObject player = gameManager.instance.player;
        float dist = Vector3.Distance(transform.position, player.transform.position);

        if (dist <= attackRange)
        {
            Vector3 dir = (player.transform.position - transform.position).normalized;

            playerController pc = player.GetComponent<playerController>();
            pc?.PushBack(dir, pushForce);

            // damage lands a beat after the shove instead of simultaneously
            yield return new WaitForSeconds(damageDelay);

            IDamage dmg = player.GetComponent<IDamage>();
            dmg?.takeDamage(pushDamage);
        }

        isAttacking = false;
    }
}