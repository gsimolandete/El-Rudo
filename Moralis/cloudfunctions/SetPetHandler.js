Moralis.Cloud.beforeSave("SetPet", async(request) => {
    logger.info("Handle SetPet");

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

    const Pet = Moralis.Object.extend("PetMoralis");
    const petQuery = new Moralis.Query(Pet);
    const petId =  request.object.get("equipableId");
    petQuery.equalTo("nftId",petId);
    const pet = await petQuery.first();

    rudo.set("pet", pet);
    rudo.save().then((rudo) => {
        logger.info('Pet assigned with objectId: ' + rudo.id);
    }, (error) => {
        logger.info('Failed to assign pet, with error code: ' + error.message);
        SaveFailedTransaction(request);
    });
})