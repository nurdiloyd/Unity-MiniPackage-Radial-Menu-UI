using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterController
{
    protected override void IWon() {
        base.IWon();
        levelManager.PlayerWon();
    }

    public override void KillMe() {
        if (tag == "Player") {
            TapticPlugin.TapticManager.Impact(TapticPlugin.ImpactFeedback.Heavy);
        }
        
        levelManager.PlayerIsDead();
    }
}
