Moralis.Cloud.beforeSave("RemoveWeapon", async(request) => {
    logger.info("Handle RemoveWeapon");

    const confirmed = request.object.get("confirmed");
    if(confirmed) {
        logger.info("transaction confirmed");
        return;
    }
    const Rudo = Moralis.Object.extend("RudoMoralis");
    const query = new Moralis.Query(Rudo);
    const rudoId = parseInt(request.object.get("rudoId"));
    query.equalTo("rudoId", rudoId);  
    const rudo = await query.first();

    const Weapon = Moralis.Object.extend("WeaponMoralis");
    const weaponQuery = new Moralis.Query(Weapon);
    const weaponId =  parseInt(request.object.get("equipableId"));
    weaponQuery.equalTo("nftId",weaponId);
    const weapon = await weaponQuery.first();

    rudo.remove("weapons", weapon);
    rudo.save().then((rudo) => {
        logger.info('Weapon unassigned with objectId: ' + rudo.id);
    }, (error) => {
        logger.info('Failed to unassign weapon, with error code: ' + error.message);
        SaveFailedTransaction(request);
    });

    weapon.unset("rudoOwner",-1);
    weapon.save().then((weapon) => {
        logger.info('rudoowner of weapon unassigned with objectId: ' + weapon.id);
    }, (error) => {
        logger.info('Failed to unassign rudoowner of weapon, with error code: ' + error.message);
        SaveFailedTransaction(request);
    });
})