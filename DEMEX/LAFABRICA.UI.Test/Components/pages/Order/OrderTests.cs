using Xunit;
using OpenQA.Selenium;
using LAFABRICA.UI.Test.Components; // <-- Asegúrate de incluir el namespace de tu BaseTestANGEL

namespace LAFABRICA.UI.Test.pages.Order
{
    // ¡LA MAGIA! Heredamos de BaseTestANGEL en lugar de BaseTest
    public class OrderTests : BaseTestANGEL
    {
        // Ya no necesitamos definir _driver ni _appUrl aquí,
        // los heredamos de BaseTestANGEL.

        [Fact]
        public void LoadShowOrdersPage_ShouldDisplayCorrectTitle()
        {
            try
            {
                // 🔴 ¡OJO 2! 
                // Asegúrate de que esta RUTA sea correcta.
                _driver.Navigate().GoToUrl($"{_appUrl}/Order/ShowOrders");
                Thread.Sleep(2000);

                // 🔴 ¡OJO 3! 
                // Asegúrate de que este ELEMENTO exista en tu página.
                var pageTitleElement = _driver.FindElement(By.TagName("h1"));
                Assert.Equal("Lista de Órdenes", pageTitleElement.Text);
            }
            catch (Exception ex)
            {
                Assert.Fail($"La prueba falló: {ex.Message}");
            }
        }

        // Aquí pondrás tus otras 4 pruebas para "Órdenes"
    }
}