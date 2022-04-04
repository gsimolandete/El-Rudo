Moralis.Cloud.beforeSave("LevelUpSkill", async(request) => {
    logger.info("Handle LevelUpSkill");

    const confirmed = request.object.get("confirmed");
    if(!confirmed) {
        logger.info("not confirmed");
        return;
    }

    logger.info("Handle LevelUpSkill");
    const Rudo = Moralis.Object.extend("Rudo");
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
    });
    SetNext3Skills(rudoId);
})