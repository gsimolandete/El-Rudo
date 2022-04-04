using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ServerDifferentFunctions;

public static class GlobalVariables
{
    public static CombatDynamics CombatDynamicsInstance; //for testing

    public static bool ShowLogs = false, DeadKeepFighting = true;

    public const int MAXIMUMRUDOS = 10;

    public const float  turnMeter = 100f, // full capacity of turn metter
                        strength_noWeaponDamage = 1f / 35f,
                        velocity_TurnMeter = 1f / 20f, //rate at wich the turn meter fills, depends on the velocity
                        velocity_Initiative = 1f / 30f,
                        velocity_Anticipation = 1f / 40f,
                        velocity_Multihit = 1f / 40f,
                        agility_CounterAttack = 1f / 100f,
                        agility_Evasion = 1f / 100f,
                        emptyHandedBlockPercent = 0.1f,
                        BlockFlatDamage = 2f,
                        defaultAttackDistance = 5f,
                        defaultInitiative = 0,
                        defaultMultiHit = 0.15f,
                        defaultCounterattack = 0.15f,
                        defaultEvasion = 0.10f,
                        defaultAnticipate = 0.05f,
                        defaultBlock = 0.20f,
                        defaultArmor = 0.9f,
                        defaultDisarm = 0,
                        defaultPrecision = 0,
                        defaultAccuracy = 0;
    public enum AttackType { Melee, Ranged }
    public enum Rarities { Common, Rare, Epic, Legendary}
    public static float MaxHP(Fighter fighter) { return 50f + fighter.Vitality; }
    public static float TimeToAttack(FighterCombat fc) { return (turnMeter - fc.TurnMeter) / AttackMeterVelocity(fc); }
    public static float MeterIncrement(FighterCombat fc, float time) { return AttackMeterVelocity(fc) * time; }
    static float AttackMeterVelocity(FighterCombat fc) { return (1 + velocity_TurnMeter * fc.Fighter.Velocity); }
    public static void PrintWithColor(string message, string color = "#FFFFFF") { if(ShowLogs) print2("<color="+color+">"+ message +"</color>"); }
}
