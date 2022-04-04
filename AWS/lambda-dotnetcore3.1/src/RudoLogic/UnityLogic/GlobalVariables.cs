using System.Collections;
using System.Collections.Generic;
using static ServerDifferentFunctions;

public static class GlobalVariables
{
    static bool ShowLogs = true;
    /// <summary>
    /// rate at wich the turn meter fills, depends on the velocity
    /// </summary>
    public const float velocity_TurnMeter = 1f / 20f;
    /// <summary>
    /// the full capacity of the turn metter
    /// </summary>
    public const float turnMeter = 100f;
    public const float strength_Damage = 1f / 2f;
    public const float velocity_Initiative = 1f / 3f;
    public const float agility_Multihit = 1f / 4f;
    public const float agility_CounterAttack = 1f / 4f;
    public const float agility_Evasion = 1f / 4f;
    public const float velocity_Anticipation = 1f / 4f;
    public const float emptyHandedBlockedDamage = 5f;
    public const float strength_Block = 1f / 3f;
    public const float defaultAttackRange = 5f;
    public const AttackType defaultAttackType = AttackType.Melee;
    public const int MAXIMUMRUDOS = 10;
    public enum AttackType { Ranged, Melee }

    public static float MaxHP(Fighter fighter) { return 50f + fighter.Vitality; }
    public static float TimeToAttack(FighterCombat fc) { return (turnMeter - fc.TurnMeter) / AttackMeterVelocity(fc); }
    public static float MeterIncrement(FighterCombat fc, float time) { return AttackMeterVelocity(fc) * time; }
    static float AttackMeterVelocity(FighterCombat fc) { return (1 + velocity_TurnMeter * fc.Fighter.Velocity); }
    public static void PrintWithColor(string color, string message) { if(ShowLogs) print2("<color="+color+">"+ message +"</color>"); }
}
