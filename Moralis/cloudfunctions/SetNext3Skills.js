const skillsChances = [0.05,0.20,0.60,1];
const maxSkillsCount = [100,150,200,250];
const availableSkillsCount = [10,15,20,25];

const statsChances = [0.05,0.20,0.60,1];
const totalStatsIncrease = [30,24,18,12];

async function SetNext3Skills(rudoId){
    const choise1 = await RandomStats();
    logger.info("choise1: "+choise1);

    const choise2 = await RandomSkill(rudoId,-1);
    logger.info("choise2: "+choise2);

    var choise3 = new Array(4).fill(0);
    var choiseType3 = 2;
    if(Math.random()>0.9){
        choise3 = await RandomStats();
        choiseType3 = 1;
    }else{
        choise3[0] = await RandomSkill(rudoId,parseInt(choise2));
        choiseType3 = 0;
    }
    logger.info("choise3: "+choise3);

    logger.info("creating httprequest");

    Moralis.Cloud.httpRequest({
        method: "POST",
        url: "https://0x5l594x0e.execute-api.eu-west-1.amazonaws.com/Prod/UpdateSystem",
        headers: {
            'Content-Type': 'application/json'
        },
        body: {
            "id" : rudoId,
            "lvlinc1" : {
                "choiseType" : 1,
                "statsIncrease" : {
                    "stat1": choise1[0],
                    "stat2": choise1[1],
                    "stat3": choise1[2],
                    "stat4": choise1[3]
                }
            },
            "lvlinc2" : {
                "choiseType" : 0,
                "statsIncrease" : {
                    "stat1" : choise2,
                    "stat2" : 0,
                    "stat3" : 0,
                    "stat4" : 0
                }
            },
            "lvlinc3" : {
                "choiseType" : choiseType3,
                "statsIncrease" : {
                    "stat1" : choise3[0],
                    "stat2" : (choiseType3==0 ? choise3[1]:0),
                    "stat3" : (choiseType3==0 ? choise3[2]:0),
                    "stat4" : (choiseType3==0 ? choise3[3]:0)
                }
            }
        }
      }).then(function(httpResponse) {
        console.log(httpResponse.text);
      }, function(httpResponse) {
        console.error('Request failed with response code ' + httpResponse.status);
      });
      logger.info("after httprequest");
}

async function RandomStats(){
    var increasingStats = new Array(4).fill(0);
    var statsRarity = 4;
    const randomNum = Math.random();
    for(var i=0;i<4;i++){

        if(randomNum<statsChances[i]){
            statsRarity = i;
            break;
        }
    }
    const toAssignStats = totalStatsIncrease[statsRarity];
    var singleAssign = parseInt(toAssignStats/4);
    var assignedStats = 0;
    for(var i=0;i<4;i++){
        increasingStats[i] = parseInt(Math.random()*singleAssign);
        assignedStats += increasingStats[i];
    }
    while(assignedStats<toAssignStats){
        let ta = (toAssignStats-assignedStats)%(singleAssign)+1;
        increasingStats[parseInt(Math.random()*4)] = ta;
        assignedStats += ta;
    }
    return increasingStats;
}

async function RandomSkill(rudoId, lastNextSkillId){
    const Rudo = Moralis.Object.extend("Rudo");
    const query = new Moralis.Query(Rudo);
    query.equalTo("rudoId", parseInt(rudoId));   
    const rudo = await query.first();
    const skills = rudo.get("skills");


    const randomNum = Math.random();
    var skillRarity=4;
    var skillIdOffset = 0;
    for(var i=0;i<4;i++){
        if(randomNum<skillsChances[i]){
            skillRarity = i;
            break;
        }
    }
    switch(skillRarity){
        case 0:
            var possibleSkills = new Array(maxSkillsCount[0]).fill(0);
            break;
        case 1:
            skillIdOffset += maxSkillsCount[0];
            var possibleSkills = new Array(maxSkillsCount[1]).fill(0);
            break;
        case 2:
            skillIdOffset += maxSkillsCount[0] + maxSkillsCount[1];
            var possibleSkills = new Array(maxSkillsCount[2]).fill(0);
            break;
        case 3:
            skillIdOffset += maxSkillsCount[0] + maxSkillsCount[1] + maxSkillsCount[2];
            var possibleSkills = new Array(maxSkillsCount[3]).fill(0);
            break;
    }

    var ownedSkills = 0;
    if(skills!=null){
        skills.forEach(element => {
            if(element < skillIdOffset + possibleSkills.length){
                possibleSkills[element-skillIdOffset] = 1;
                ownedSkills++;
            }
        });
    }

    if(lastNextSkillId >= 0 && lastNextSkillId < skillIdOffset + possibleSkills.length && lastNextSkillId > skillIdOffset){
        possibleSkills[lastNextSkillId-skillIdOffset] = 1;
    }

    const unlearnedSkills = availableSkillsCount[skillRarity] - ownedSkills;
    var unlearnedSkillsIndex = 0;
    const randomUnlearnedSkill = parseInt(Math.random()*unlearnedSkills);
    for(var i=0;i<availableSkillsCount[skillRarity];i++){
        if(possibleSkills[i]>0){
            continue;
        }else{
            if(unlearnedSkillsIndex == randomUnlearnedSkill){
                return i + skillIdOffset;
            }else{
                unlearnedSkillsIndex++;
            }
        }
    }

    logger.error("SIN SKILL"); //OPTIMIZE tenemos que pasar a otra rarity
}