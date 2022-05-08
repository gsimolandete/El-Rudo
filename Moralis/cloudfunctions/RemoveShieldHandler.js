Moralis.Cloud.beforeSave("RemoveShield", async(request) => {
    logger.info("Handle RemoveShield");

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
    rudo.unset("shield");
    rudo.save().then((rudo) => {
        logger.info('Shield unassigned with objectId: ' + rudo.id);
    }, (error) => {
        logger.info('Failed to unassign shield, with error code: ' + error.message);
        SaveFailedTransaction(request);
    });
})