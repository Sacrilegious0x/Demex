using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace LAFABRICA.Models.AuxiliarDTOS
{
    public class InventoryMaterialDto
    {
        public int materialId {  get; set; }
        public string state {  get; set; }

        public int minimunQuantity { get; set; }

    }
}
