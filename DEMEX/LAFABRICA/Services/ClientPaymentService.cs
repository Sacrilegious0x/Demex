using LAFABRICA.Data.DB;
using LAFABRICA.Models.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LAFABRICA.Services
{
    public class ClientPaymentService : IClientPaymentService
    {
        private readonly AppDbContext _context;

        public ClientPaymentService(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<ClientPayment> CreatePayment(ClientPayment payment)
        {
            // Busca la orden asociada
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == payment.OrderId);
            if (order == null)
            {
                throw new KeyNotFoundException($"La orden con ID {payment.OrderId} no fue encontrada.");
            }

            // Añade el nuevo pago al contexto
            _context.ClientPayments.Add(payment);

            
            // Actualiza el campo Advancement de la orden sumando el nuevo monto
            order.Advancement += payment.Amount;
            _context.Orders.Update(order); // Marca la orden como modificada
            

            // Guarda ambos cambios (el nuevo pago y la actualización de la orden)
            await _context.SaveChangesAsync();
            return payment;
        }

        
        public async Task<bool> DeletePayment(int id)
        {
            // Busca el pago Y su orden asociada
            var payment = await _context.ClientPayments
                                      .Include(p => p.Order) // Incluye la orden para actualizarla
                                      .FirstOrDefaultAsync(p => p.Id == id);
            if (payment == null)
            {
                return false; // No encontrado
            }

           
            // Si la orden existe, resta el monto del pago del adelanto total
            if (payment.Order != null)
            {
                payment.Order.Advancement -= payment.Amount;
                // Asegúrate que no quede negativo (por si acaso)
                if (payment.Order.Advancement < 0)
                {
                    payment.Order.Advancement = 0;
                }
                _context.Orders.Update(payment.Order); // Marca la orden como modificada
            }
           


            _context.ClientPayments.Remove(payment); // Borrado físico del pago
            await _context.SaveChangesAsync(); // Guarda ambos cambios
            return true; 
        }

        public async Task<IEnumerable<ClientPayment>> GetAllPayments()
        {
            // Incluimos Order y Client para que la tabla y el modal tengan los datos
            return await _context.ClientPayments
                                 .Include(p => p.Order)
                                     .ThenInclude(o => o.Client)
                                 .OrderByDescending(p => p.PaymentDate)
                                 .ToListAsync();
        }

        public async Task<ClientPayment?> GetPaymentById(int id)
        {
            // La consulta ya incluía Order y Client, está correcta
            return await _context.ClientPayments
                                 .Include(p => p.Order)
                                     .ThenInclude(o => o.Client)
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<ClientPayment>> GetPaymentsByOrderId(int orderId)
        {
            return await _context.ClientPayments
                                 .Where(p => p.OrderId == orderId)
                                 .OrderByDescending(p => p.PaymentDate)
                                 .ToListAsync();
        }

       
        public async Task<ClientPayment> UpdatePayment(int id, ClientPayment payment)
        {
            // Incluye la orden para poder actualizar el Advancement
            var existingPayment = await _context.ClientPayments
                                              .Include(p => p.Order)
                                              .FirstOrDefaultAsync(p => p.Id == id);

            if (existingPayment == null)
            {
                throw new KeyNotFoundException($"El pago con el id {id} no fue encontrado.");
            }
            if (existingPayment.Order == null)
            {
                // Si por alguna razón la orden asociada es nula, no podemos actualizar Advancement
                throw new InvalidOperationException($"No se encontró la orden asociada al pago {id} para actualizar el adelanto.");
            }

            // Guardamos el monto anterior antes de aplicar los nuevos valores
            decimal oldAmount = existingPayment.Amount;

            // Actualizar propiedades simples del pago
            _context.Entry(existingPayment).CurrentValues.SetValues(payment);
            // Nos aseguramos que el ID y OrderId no cambien (SetValues podría intentar cambiarlos)
            existingPayment.Id = id;
            // existingPayment.OrderId = existingPayment.OrderId; // No debería cambiar OrderId al editar pago


          
            // Calculamos la diferencia y actualizamos el Advancement de la orden
            decimal amountDifference = existingPayment.Amount - oldAmount; // Nuevo Monto - Monto Viejo
            existingPayment.Order.Advancement += amountDifference;
            // Asegúrate que no quede negativo
            if (existingPayment.Order.Advancement < 0)
            {
                existingPayment.Order.Advancement = 0;
            }
            _context.Orders.Update(existingPayment.Order); // Marca la orden como modificada
            


            await _context.SaveChangesAsync(); 
            return existingPayment;
        }
    }
}