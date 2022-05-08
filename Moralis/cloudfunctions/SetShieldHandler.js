Moralis.Cloud.beforeSave("SetShield", async(request) => {
    logger.info("Handle SetShield");

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

    const Shield = Moralis.Object.extend("ShieldMoralis");
    const shieldQuery = new Moralis.Query(Shield);
    const shieldId =  parseInt(request.object.get("equipableId"));
    shieldQuery.equalTo("nftId",shieldId);
    const shield = await shieldQuery.first();

    rudo.set("shield", shield);
    rudo.save().then((rudo) => {
        logger.info('Shield assigned with objectId: ' + rudo.id);
    }, (error) => {
        logger.info('Failed to assign shield, with error code: ' + error.message);
        SaveFailedTransaction(request);
    });
})