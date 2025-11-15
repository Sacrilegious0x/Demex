using LAFABRICA.Tests.Components;
using OpenQA.Selenium;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAFABRICA.UI.Test.Components.pages.Order
{
    // ¡Importante! Heredamos de BaseTest
    public class OrderTests : BaseTest
    {
        // 🔴 ¡OJO 1! CAMBIA ESTO por la URL de tu app en IIS
        private readonly string _appUrl = "http://localhost:8080";

        [Fact]
        // PRUEBA 1: Verificar que la página de "Mostrar Órdenes" carga
        public void LoadShowOrdersPage_ShouldDisplayCorrectTitle()
        {
            try
            {
                // 🔴 ¡OJO 2! CAMBIA ESTA RUTA si la URL es diferente
                Driver.Navigate().GoToUrl($"{_appUrl}/Order/ShowOrders");
                Thread.Sleep(2000);

                // 🔴 ¡OJO 3! CAMBIA ESTO por un ID o Texto real de tu página
                var pageTitleElement = Driver.FindElement(By.TagName("h1"));
                Assert.Equal("Lista de Órdenes", pageTitleElement.Text);
            }
            catch (Exception ex)
            {
                Assert.Fail($"La prueba falló: {ex.Message}");
            }
        }
    }
}