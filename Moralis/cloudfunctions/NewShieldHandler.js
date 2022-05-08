Moralis.Cloud.beforeSave("NewShield", (request) => {
    logger.info("Handle NewShield");
    
    const confirmed = request.object.get("confirmed");
    if(confirmed) {
        logger.info("transaction confirmed");
        return;
    }

    const Shield = Moralis.Object.extend("ShieldMoralis");
    const shield = new Shield();
    const nftId = parseInt(request.object.get("uid"));
    shield.set("nftId", nftId);
    shield.set("owner", request.object.get("owner"));
    shield.set("weaponId", parseInt(request.object.get("weaponId")));
    shield.set("weaponQuality", parseInt(request.object.get("weaponQuality")));
    
    shield.save().then((shield) => {
        logger.info('New object created with objectId: ' + shield.id);
    }, (error) => {
        logger.info('Failed to create new object, with error code: ' + error.message) ;
        SaveFailedTransaction(request);
  });
})