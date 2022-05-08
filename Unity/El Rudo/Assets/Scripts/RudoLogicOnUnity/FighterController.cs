using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static FighterCombat;

[System.Serializable]
public class FighterController : MonoBehaviour
{
    [SerializeField]
    Animator anim;
    [SerializeField]
    Vector3 targetPosition, targetRudoSpawn, ownSpawn;
    [SerializeField]
    bool finishedAction;
    float RudoSpeed = 10;
    [SerializeField]
    Transform weaponSpawn;

    //Behaviours
    bool running, disarming;

    public Vector2 TargetPosition { get => targetPosition; set => targetPosition = value; }
    public Vector2 OwnSpawn { get => ownSpawn; set => ownSpawn = value; }
    public bool FinishedAction { get => finishedAction; set => finishedAction = value; }

    private void Start()
    {
        finishedAction = true;
        targetPosition = transform.position;
        running = false;
    }

    public void Initialize(Vector3 thisRudoSpawn, Vector3 targetRudoSpawn)
    {
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
            if (running && (targetPosition - transform.position).magnitude >= 0.05f)
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

    public void SetTargetPosition(Vector3 targetPosition)
    {
        finishedAction = false;
        running = true;
        this.targetPosition = targetPosition;
    }

    public void Attack()
    {
        finishedAction = false;
        anim.SetTrigger("attack");
    }
    public void YieldWeapon(Weapon weapon)
    {
        Addressables.InstantiateAsync(weapon.Equipable.pathToAddressable, weaponSpawn);
        finishedAction = false;
        anim.SetTrigger("yieldWeapon");
    }
    public void SelfDisarm()
    {
        finishedAction = false;
        anim.SetTrigger("selfDisarm");
    }
    public void ForcedDisarm()
    {
        disarming = true;
    }
    public void MaybeDisarm()
    {
        if (disarming)
        {
            disarming = false;
            Disarm();
        }
    }
    public void Disarm()
    {
        weaponSpawn.GetComponentInChildren<RudoWeaponController>().Disarm();
    }
    public void Dodge()
    {
        finishedAction = false;
        anim.SetTrigger("dodge");
    }

    public void Block()
    {
        finishedAction = false;
        anim.SetTrigger("block");
    }

    public void GetHurt()
    {
        finishedAction = false;
        anim.SetTrigger("hurt");
    }

    public void GetDefeated()
    {
        finishedAction = false;
        anim.SetTrigger("die");
    }

    public void EndAnimation()
    {
        finishedAction = true;
    }

    void FaceTargetRudo()
    {
        transform.localScale = new Vector3((targetRudoSpawn.x - transform.position.x) > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void FaceTargetPosition()
    {
        transform.localScale = new Vector3((targetPosition.x - transform.position.x) > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}
