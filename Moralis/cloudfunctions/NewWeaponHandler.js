Moralis.Cloud.beforeSave("NewWeapon", (request) => {
    logger.info("Handle NewWeapon");
    
    const confirmed = request.object.get("confirmed");
    if(confirmed) {
        logger.info("transaction confirmed");
        return;
    }

    const Weapon = Moralis.Object.extend("WeaponMoralis");
    const weapon = new Weapon();
    const nftId = parseInt(request.object.get("uid"));
    weapon.set("nftId", nftId);
    weapon.set("owner", request.object.get("owner"));
    weapon.set("weaponId", parseInt(request.object.get("weaponId")));
    weapon.set("weaponQuality", parseInt(request.object.get("weaponQuality")));
    
    weapon.save().then((weapon) => {
        logger.info('New object created with objectId: ' + weapon.id);
    }, (error) => {
        logger.info('Failed to create new object, with error code: ' + error.message) ;
        SaveFailedTransaction(request);
  });
})