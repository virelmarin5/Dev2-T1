/*
 * Script: NukeKillstreak
 * Author: Devin Childs
 *
 * Description:
 * Implements a Call of Duty Zombies-style nuke killstreak.
 * When activated, every active enemy in the scene is immediately killed.
 *
 * The enemies are killed through EnemyBase.ForceKill() instead of being
 * directly destroyed. This allows each enemy to notify the WaveManager,
 * update the remaining enemy count, and trigger its normal death logic.
 *
 * Responsibilities:
 * - Find every active EnemyBase in the scene.
 * - Immediately kill each enemy.
/*
 * Script: NukeKillstreak
 * Author: Devin Childs
 *
 * Description:
 * Implements a Call of Duty Zombies-style Nuke killstreak.
 * When activated, all currently active enemies in the scene are
 * immediately killed.
 *
 * Enemies are killed through EnemyBase.ForceKill() so they still
 * notify WaveManager and run their normal death sequence.
 * Nuke deaths are temporarily prevented from increasing the
 * player's active kill chain.
 *
 * Responsibilities:
 * - Find every active EnemyBase in the scene.
 * - Temporarily suppress kill-chain registration.
 * - Force each enemy through its normal death process.
 * - Restore normal kill-chain registration afterward.
 *
 * Interacts With:
 * - killstreakBase
 * - EnemyBase
 * - killChainManager
 * - waveManager indirectly through EnemyBase
 *
 * Last Updated:
 * Prototype 1
 */

using UnityEngine;

public class nukeKillstreak : killstreakBase
{
    protected override void onActivate()
    {
        // Prevent all enemies killed by the nuke from adding
        // to the current kill chain.
        if (killChainManager.instance != null)
        {
            killChainManager.instance.SetIgnoreRegisteredKills(true);
        }

        // Finds all currently active EnemyBase components.
        EnemyBase[] enemies = FindObjectsByType<EnemyBase>();

        int enemiesKilled = 0;

        foreach (EnemyBase enemy in enemies)
        {
            if (enemy == null)
                continue;

            enemy.ForceKill();
            enemiesKilled++;
        }

        // Return the kill-chain system to normal after the nuke finishes.
        if (killChainManager.instance != null)
        {
            killChainManager.instance.SetIgnoreRegisteredKills(false);
        }

        Debug.Log(
            "Nuke activated. Enemies killed: " + enemiesKilled
        );
    }

    protected override void onDeactivate()
    {
        // The Nuke is an immediate effect, so there is nothing to undo.
    }
}
