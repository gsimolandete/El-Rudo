Moralis.Cloud.beforeSave("LevelUpSkill", async(request) => {
    logger.info("Handle LevelUpSkill");

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
    rudo.add("skills", request.object.get("skillId"));
    rudo.save().then((rudo) => {
        logger.info('Level assigned with objectId: ' + rudo.id);
    }, (error) => {
        logger.info('Failed to assign level, with error code: ' + error.message);
        SaveFailedTransaction(request);
    });
    SetNext3Skills(rudoId);
})