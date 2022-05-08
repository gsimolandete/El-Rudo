Moralis.Cloud.beforeSave("SetWeapon", async(request) => {
    logger.info("Handle SetWeapon");

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
    weaponQuery.equalTo("nftId", weaponId);
    const weapon = await weaponQuery.first();

    rudo.addUnique("weapons", weapon);
    rudo.save().then((rudo) => {
        logger.info('Weapon assigned with objectId: ' + rudo.id);
    }, (error) => {
        logger.info('Failed to assign weapon, with error code: ' + error.message);
        SaveFailedTransaction(request);
   });
})