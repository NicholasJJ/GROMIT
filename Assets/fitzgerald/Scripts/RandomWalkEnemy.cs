using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalkEnemy : FitzEnemy
{

    protected override void WhileAlive()
    {
        WanderUpdate();
    }

    protected void WanderUpdate()
    {
        if (agent.remainingDistance <= 0.001f)
        {
            var newDest = roomVol.RandomPointInRoom();
            newDest.y = transform.position.y;

            //Debug.Log("Moving to " + newDest);

            agent.destination = newDest;
        }
    }
}
