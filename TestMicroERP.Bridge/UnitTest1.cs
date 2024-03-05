using MicroERP.Bridge;

namespace TestMicroERP.Bridge
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            try
            {
                //MicroERPConnection connection1 = await MicroERPConnection.RegisterAsync("https://localhost:49773", "MicroERPHomolog", "Vinicius1234", "123Hash", "vini1234@gmail.com");

                MicroERPConnection connection = new MicroERPConnection("https://localhost:49773", "MicroERPHomolog", "Vinicius123", "123Hash");

            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}