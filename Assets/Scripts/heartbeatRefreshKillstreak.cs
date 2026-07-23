/*
 * Script: HeartbeatRefreshKillstreak
 * Author: Devin Childs
 *
 * Description:
 * Immediately removes all player stress and restores the player's
 * heart rate to the resting BPM configured in HeartbeatManager.
 *
 * For the intended Prototype 1 effect, set Resting BPM to 20
 * on the HeartbeatManager object in the Unity Inspector.
 *
 * Responsibilities:
 * - Locate HeartbeatManager.
 * - Clear current player stress.
 * - Restore the player's BPM to its resting value.
 *
 * Interacts With:
 * - killstreakBase
 * - heartbeatManager
 * - gameManager indirectly through heartbeatManager
 *
 * Last Updated:
 * Prototype 1
 */

using UnityEngine;

public class heartbeatRefreshKillstreak : killstreakBase
{
    protected override void onActivate()
    {
        if (heartbeatManager.instance == null)
        {
            // Debug.LogWarning(
            //     "Heartbeat Refresh failed because HeartbeatManager was not found."
            // );

            return;
        }

        heartbeatManager.instance.resetHeartbeat();

        //Debug.Log("Heartbeat Refresh activated.");
    }

    protected override void onDeactivate()
    {
        // This is an immediate effect, so there is nothing to undo.
    }
}
