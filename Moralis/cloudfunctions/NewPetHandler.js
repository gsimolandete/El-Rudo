Moralis.Cloud.beforeSave("NewPet", (request) => {
    logger.info("Handle NewPet");
    
    const confirmed = request.object.get("confirmed");
    if(confirmed) {
        logger.info("transaction confirmed");
        return;
    }

    const Pet = Moralis.Object.extend("PetMoralis");
    const pet = new Pet();
    const nftId = parseInt(request.object.get("uid"));
    pet.set("nftId", nftId);
    pet.set("owner", request.object.get("owner"));
    pet.set("weaponId", parseInt(request.object.get("weaponId")));
    pet.set("weaponQuality", parseInt(request.object.get("weaponQuality")));
    
    pet.save().then((pet) => {
        logger.info('New object created with objectId: ' + pet.id);
    }, (error) => {
        logger.info('Failed to create new object, with error code: ' + error.message) ;
        SaveFailedTransaction(request);
  });
})