Moralis.Cloud.beforeSave("LevelUpStats", async(request) => {
    logger.info("Handle LevelUpStats");

    const confirmed = request.object.get("confirmed");
    if(confirmed) {
        logger.info("transaction confirmed");
        return;
    }
    const Rudo = Moralis.Object.extend("RudoMoralis");
    const query = new Moralis.Query(Rudo);
    const rudoId = parseInt(request.object.get("uid"));
    query.equalTo("rudoId", rudoId);  
    const rudo = await query.first();
    rudo.set("level", parseInt(request.object.get("level")));
    rudo.set("vitality", parseInt(request.object.get("vitality")));
    rudo.set("strength", parseInt(request.object.get("strength")));
    rudo.set("agility", parseInt(request.object.get("agility")));
    rudo.set("velocity", parseInt(request.object.get("velocity")));
    rudo.save().then((rudo) => {
        logger.info('Level assigned with objectId: ' + rudo.id);
    }, (error) => {
        logger.info('Failed to assign level, with error code: ' + error.message);
        SaveFailedTransaction(request);
    });
    SetNext3Skills(rudoId);
})