Moralis.Cloud.beforeSave("NewRudo", (request) => {
    logger.info("Handle NewRudo");
    
    const confirmed = request.object.get("confirmed");
    if(confirmed) {
        logger.info("transaction confirmed");
        return;
    }

    const Rudo = Moralis.Object.extend("RudoMoralis");
    const rudo = new Rudo();
    const rudoId = parseInt(request.object.get("uid"));
    rudo.set("rudoId", rudoId);
    rudo.set("owner", request.object.get("owner"));
    rudo.set("name", request.object.get("name"));
    rudo.set("level", 1);
    rudo.set("vitality", parseInt(request.object.get("vitality")));
    rudo.set("strength", parseInt(request.object.get("strength")));
    rudo.set("agility", parseInt(request.object.get("agility")));
    rudo.set("velocity", parseInt(request.object.get("velocity")));
    rudo.set("elo", 0);
    rudo.set("experience", 0);
    
    rudo.save().then((rudo) => {
        logger.info('New object created with objectId: ' + rudo.id);
        logger.info('Setting next 3 skills for rudoId '+ rudoId);
        SetNext3Skills(rudoId);
    }, (error) => {
        logger.info('Failed to create new object, with error code: ' + error.message) ;
        SaveFailedTransaction(request);
  });
})