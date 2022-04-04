Moralis.Cloud.beforeSave("GainExp", async(request) => {
    logger.info("Handle GainExp");

    const confirmed = request.object.get("confirmed");
    if(!confirmed) {
        logger.info("not confirmed");
        return;
    }

    logger.info("Handle GainExp");
    const Rudo = Moralis.Object.extend("Rudo");
    const query = new Moralis.Query(Rudo);
    query.equalTo("rudoId", parseInt(request.object.get("uid")));  
    const rudo = await query.first();
    rudo.set("experience", parseInt(request.object.get("experience")));
    rudo.save().then((rudo) => {
        logger.info('Experience assigned with objectId: ' + rudo.id);
    }, (error) => {
        logger.info('Failed to assign experience, with error code: ' + error.message) ;
  });
})