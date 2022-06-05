using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;


namespace RudoNamespace
{

    public class RudoFight
    {

        public APIGatewayProxyResponse StartFight(ILambdaContext context)
        {
            Rudo rudo1 = new Rudo(0,0,"rudo1",0,25,25,25,25,new List<Weapon>{new Weapon(0,0,0)}, new Pet(0,0,0), new Shield(0,0,0), new List<int>(){ 1, 0});
            Rudo rudo2 = new Rudo(0,0,"rudo2",0,25,25,25,25,new List<Weapon>{new Weapon(0,1,0)}, new Pet(0,0,0), new Shield(0,0,0), new List<int>(){ 1, 0});
            CombatDynamics c = new CombatDynamics(false,rudo1,rudo2,0);
            c.StartCombat();

            var body = new Dictionary<string, string>();

            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(body),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}
