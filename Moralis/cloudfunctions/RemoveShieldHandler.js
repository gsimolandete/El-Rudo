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
    const shield = rudo.get("shield");

    rudo.unset("shield");
    rudo.save().then((rudo) => {
        logger.info('Shield unassigned with objectId: ' + rudo.id);
    }, (error) => {
        logger.info('Failed to unassign shield, with error code: ' + error.message);
        SaveFailedTransaction(request);
    });

    shield.set("rudoOwner",-1);
    shield.save().then((shield) => {
        logger.info('rudoowner of shield unassigned with objectId: ' + shield.id);
    }, (error) => {
        logger.info('Failed to unassign rudoowner of shield, with error code: ' + error.message);
        SaveFailedTransaction(request);
    });
})