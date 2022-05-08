async function SaveFailedTransaction(request){
    const FailedTransaction = Moralis.Object.extend("FailedTransaction");
    const failedTransaction = new FailedTransaction();
    failedTransaction.set("transaction_hash", request.object.get("transaction_hash"));
    failedTransaction.save().then((failedTransaction) => {
        logger.info("failedTransaction saved");
    }, (error) => {
        logger.info("failed to save failedTransaction with hash: "+ failedTransaction.get("transaction_hash") +" saved, with error code:"+ error.message);
    });
}