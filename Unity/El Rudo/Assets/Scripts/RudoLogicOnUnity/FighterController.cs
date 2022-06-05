using CharacterCreator2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static FighterCombat;
using System.Linq;
using UnityEngine.UI;

[System.Serializable]
public class FighterController : MonoBehaviour
{
    [SerializeField]
    bool inverseLook;
    Animator anim;
    [SerializeField]
    Vector3 targetPosition, targetRudoSpawn, ownSpawn;
    [SerializeField]
    bool finishedAction;
    float RudoSpeed = 15;
    [SerializeField]
    public GameObject weaponSpawn, shieldSpawnFront, shieldSpawn;
    public FighterController FighterObjective;

    public RudoCombatVisual rudo;

    //Behaviours
    private bool dodging;
    private bool running;
    private bool disarming;
    private bool disarmingShield;
    private bool parryNextAttack;
    private bool blocking;
    private bool beingHurt;
    private bool beingDefeated;

    public Vector2 TargetPosition { get => targetPosition; set => targetPosition = value; }
    public Vector2 OwnSpawn { get => ownSpawn; set => ownSpawn = value; }
    public bool FinishedAction { get => finishedAction; }
    public void Running() { running = true;  finishedAction = false; }
    public void Disarming() { disarming = true; finishedAction = false; }
    public void DisarmingShield() { disarmingShield = true; finishedAction = false; }
    public void ParryNextAttack() { parryNextAttack = true; }
    public void Blocking() { blocking = true; finishedAction = false; }
    public void BeingHurt() { beingHurt = true; finishedAction = false; }
    public void BeingDefeated() { beingDefeated = true; finishedAction = false; }
    public void Dodging() { dodging = true; finishedAction = false; }

    private void Start()
    {
        finishedAction = true;
        targetPosition = transform.position;
        running = false;
    }

    public void Initialize(Vector3 thisRudoSpawn, Vector3 targetRudoSpawn, RudoCombatVisual rudo = null)
    {
        this.rudo = rudo;
        anim =GetComponent<Animator>();
        transform.position = thisRudoSpawn;
        this.targetRudoSpawn = targetRudoSpawn;
        ownSpawn = thisRudoSpawn;
        FaceTargetRudo();
    }

    private void Update()
    {
        if (!finishedAction)
        {
            //handle move
            if (running && (targetPosition - transform.position).magnitude >= 0.15f)
            {
                anim.SetBool("isRun", true);
                FaceTargetPosition();
                transform.position += (targetPosition - transform.position).normalized * RudoSpeed * Time.deltaTime;
            }
            else if(running)
            {
                anim.SetBool("isRun", false);
                FaceTargetRudo();
                running = false;
                finishedAction = true;
            }
        }
    }
    public void AttackImpactMoment()
    {
        FighterObjective.ReciveAttack();
    }
    public void ReciveAttack()
    {
        if (beingDefeated)
            GetDefeated();
        else if (beingHurt)
            GetHurt();
        else if (blocking)
            Block();
        else if (dodging)
            Dodge();
        else if (parryNextAttack)
            Parry();
        
        MaybeDisarm();

        if (rudo != null)
            rudo.UpdateHpSLider();
    }
    //When parry lands
    public void InteractWithTarget()
    {
        FighterObjective.ReciveAttack();
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        finishedAction = false;
        running = true;
        this.targetPosition = targetPosition;
    }

    public void Attack(FighterController fighterObjective)
    {
        FighterObjective = fighterObjective;
        fighterObjective.FighterObjective = this;
        finishedAction = false;
        anim.SetTrigger("attack");
    }
    public void WeaponSkillAttack(FighterController fighterObjective)
    {
        FighterObjective = fighterObjective;
        fighterObjective.FighterObjective = this;
        finishedAction = false;
        anim.SetTrigger("weaponSkill");
    }
    public async void YieldWeapon(Weapon weapon)
    {
        finishedAction = false;
        ChangeWeaponAnimations(weapon);
        WeaponSpawnProperties s = await Addressables.LoadAssetAsync<WeaponSpawnProperties>(weapon.pathToPrefab).Task;
        weaponSpawn.GetComponent<SpriteRenderer>().sprite = s.sprite;
        weaponSpawn.transform.localScale = s.scale;
        weaponSpawn.transform.localPosition = s.offset;
        anim.SetTrigger("yieldWeapon");
    }
    public async void YieldShield(Shield shield)
    {
        ShieldSpawnProperties s = await Addressables.LoadAssetAsync<ShieldSpawnProperties>(shield.pathToAddressable).Task;

        shieldSpawn.GetComponent<SpriteRenderer>().sprite = s.sprite;
        shieldSpawnFront.GetComponent<SpriteRenderer>().sprite = s.spriteFront;

        shieldSpawn.transform.localScale = s.scale;
        shieldSpawn.transform.localPosition = s.offset;
        shieldSpawnFront.transform.localScale = s.scale;
        shieldSpawnFront.transform.localPosition = s.offset;

        shieldSpawnFront.SetActive(false);
    }
    async void ChangeWeaponAnimations(Weapon weapon = null)
    {
        AnimationClip newAttack, newWeaponSKill = null;
        if (weapon != null) {
            newAttack = await Addressables.LoadAssetAsync<AnimationClip>(weapon.pathToAnimation).Task;
            newWeaponSKill = await Addressables.LoadAssetAsync<AnimationClip>(weapon.weaponSkill.animationAddressable).Task;
        }
        else
            newAttack = await Addressables.LoadAssetAsync<AnimationClip>("Assets/TestAssets/Rudos/PunchAttack.anim").Task;

        AnimatorOverrideController aoc = new AnimatorOverrideController(anim.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        for (int i = 0; i < aoc.animationClips.Length; i++)
        {
            if (aoc.animationClips[i].name.Contains("Attack"))
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(aoc.animationClips[i], newAttack));
            else if (aoc.animationClips[i].name.Contains("WeaponSkill"))
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(aoc.animationClips[i], newWeaponSKill));
            else
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(aoc.animationClips[i], anim.runtimeAnimatorController.animationClips[i]));
        }
        aoc.ApplyOverrides(anims);
        anim.runtimeAnimatorController = aoc;
    }
    public void SelfDisarm()
    {
        finishedAction = false;
        anim.SetTrigger("selfDisarm");
    }
    public void SelfDisarmShield()
    {
        disarmingShield = true;
    }
    void MaybeDisarm()
    {
        if (disarming)
        {
            disarming = false;
            Disarm();
        }
        if (disarmingShield)
        {
            disarmingShield = false;
            DisarmShield();
        }
    }
    void Disarm()
    {
        if(weaponSpawn.GetComponent<SpriteRenderer>().sprite != null)
        {
            CompleteDisarm(weaponSpawn);
            ChangeWeaponAnimations();
        }
    }
    void DisarmShield()
    {
        if (shieldSpawn.GetComponent<SpriteRenderer>().sprite != null)
        {
            CompleteDisarm(shieldSpawn);
            shieldSpawnFront.GetComponent<SpriteRenderer>().sprite = null;
        }
    }
    async void CompleteDisarm(GameObject spawn)
    {
        GameObject disarmGO = await Addressables.InstantiateAsync("Assets/TestAssets/Weapons/Animations/weaponSpawn.prefab").Task;
        disarmGO.transform.localScale = spawn.transform.localScale;
        //disarmGO.transform.localScale = inverseLook ? new Vector2(spawn.transform.localScale.x * (-1), spawn.transform.localScale.y)  : spawn.transform.localScale;
        disarmGO.transform.position = spawn.transform.position;
        disarmGO.transform.GetChild(0).rotation = spawn.transform.rotation;
        disarmGO.GetComponentInChildren<SpriteRenderer>().sprite = spawn.GetComponent<SpriteRenderer>().sprite;
        disarmGO.GetComponent<Animator>().Play("disarmAnimation");
        spawn.GetComponent<SpriteRenderer>().sprite = null;
    }
    void Dodge()
    {
        dodging = false;
        finishedAction = false;
        anim.SetTrigger("dodge");
    }

    void Block()
    {
        blocking = false;
        finishedAction = false;
        anim.SetTrigger("block");
    }

    void GetHurt()
    {
        beingHurt = false;
        finishedAction = false;
        anim.SetTrigger("hurt");
    }
    void Parry()
    {
        finishedAction = false;
        parryNextAttack = false;
        anim.SetTrigger("parry");
    }

    void GetDefeated()
    {
        beingDefeated = false;
        anim.SetTrigger("die");
    }

    public void EndAnimation()
    {
        finishedAction = true;
    }

    void FaceTargetRudo()
    {
        if(!inverseLook)
            transform.localScale = new Vector3((targetRudoSpawn.x - transform.position.x) > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3((targetRudoSpawn.x - transform.position.x) > 0 ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void FaceTargetPosition()
    {
        if (!inverseLook)
            transform.localScale = new Vector3((targetPosition.x - transform.position.x) > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3((targetPosition.x - transform.position.x) > 0 ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void Teleport(float time)
    {

    }
}
