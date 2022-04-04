using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;


namespace RudoNamespace
{

    public class RudoFight
    {

        public APIGatewayProxyResponse StartFight(ILambdaContext context)
        {
            //RudoLog rudoLog1 = aCalls.GetRudoById(0) as RudoLog;
            //RudoLog rudoLog2 = aCalls.GetRudoById(1) as RudoLog;
            //CombatDynamics c = new CombatDynamics(10,rudoLog1,rudoLog2);
            //c.StartCombat();

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
