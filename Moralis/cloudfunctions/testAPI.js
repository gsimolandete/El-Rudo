Moralis.Cloud.define("Hello", (request) => {
    SetNext3Skills(15);
    return `Hello ${request.params.name}! Cloud functions are cool!`
    });