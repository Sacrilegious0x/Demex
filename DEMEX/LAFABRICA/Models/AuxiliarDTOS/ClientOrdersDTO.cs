namespace LAFABRICA.Models.AuxiliarDTOS
{
    public class ClientOrdersDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Manager { get; set; } = null!;
        public string ManagerPhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string SpecificLocation { get; set; } = null!;
        public bool IsFrequent { get; set; }
        public int CompletedOrders { get; set; }
    }
}

