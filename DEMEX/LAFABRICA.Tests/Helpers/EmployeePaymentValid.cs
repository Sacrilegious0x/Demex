using LAFABRICA.Data.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAFABRICA.Tests.Helpers
{
    public static class EmployeePaymentValid
    {
        public static EmployeePayment CreateValidPayment(
            int employeeId = 1,
            string description = "Pago semanal",
            string state = "Pendiente",
            decimal total = 15000,
            bool isActive = true)
        {
            return new EmployeePayment
            {
                EmployeeId = employeeId,
                Description = description,
                State = state,
                TotalAmount = total,
                Date = DateTime.Now,
                IsActive = isActive
            };
        }
        public static EmployeePayment CreatePaidPayment(
            int employeeId = 2,
            decimal total = 25000)
        {
            return new EmployeePayment
            {
                EmployeeId = employeeId,
                Description = "Pago completado",
                State = "Pagado",
                TotalAmount = total,
                Date = DateTime.Now,
                IsActive = true
            };
        }

        public static EmployeePayment CreateInactivePayment(
            int employeeId = 3,
            string description = "Pago anulado")
        {
            return new EmployeePayment
            {
                EmployeeId = employeeId,
                Description = description,
                State = "Anulado",
                TotalAmount = 0,
                Date = DateTime.Now,
                IsActive = false
            };
        }

        public static EmployeePayment CreatePendingPayment(
            int employeeId = 4,
            decimal total = 10000)
        {
            return new EmployeePayment
            {
                EmployeeId = employeeId,
                Description = "Pago pendiente de aprobación",
                State = "Pendiente",
                TotalAmount = total,
                Date = DateTime.Now,
                IsActive = true
            };
        }
    }
}
