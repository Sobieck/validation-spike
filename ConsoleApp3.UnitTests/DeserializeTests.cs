using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleApp3.UnitTests
{
    public class DeserializeTests
    {
        [Fact]
        public async Task CanDeserializeJtoken()
        {
            var validatorDefinitionJson = "{\"name\" : \"Address.Street\", \"validators\" : [ { \"name\" : \"length\", \"parameters\" : { \"max\" : 5 }}]}";

            var deserializedFieldResponse = JsonConvert.DeserializeObject<FieldResponse>(validatorDefinitionJson);

            var validator = deserializedFieldResponse.Validators.First();

            var jTokenParam = validator.Parameters["max"];

            Assert.Equal(5, jTokenParam);
        }
    }
}
