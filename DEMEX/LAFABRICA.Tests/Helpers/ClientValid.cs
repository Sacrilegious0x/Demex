using LAFABRICA.Data.DB;

namespace LAFABRICA.Tests.Helpers
{
    internal static class ClientValid
    {

        public static Client CreateValidClient(
        string email = "cliente.test@gmail.com",
        string phone = "2222-2222",
        string managerPhone = "8888-8888")
        {
            return new Client
            {
                Name = "Empresa para Test",
                PhoneNumber = phone,
                Manager = "Gabs",
                ManagerPhoneNumber = managerPhone,
                Email = email,
                Location = "Limon",
                SpecificLocation = "Guapiles",
                QuantityPurchase = 0,
                IsFrequent = 0,
                IsActive = 1
            };
        }
        public static Client CreateInactiveClient()
        {
            var client = CreateValidClient();
            client.Email = "clienteInactivo.test@gmail.com";
            client.PhoneNumber = "2121-2121";
            client.ManagerPhoneNumber = "6666-6666";
            client.IsActive = 0;
            return client;
        }
        public static Client CreateFrequentClient()
        {
            var client = CreateValidClient();
            client.IsFrequent = 1;
            return client;
        }


    }
}
