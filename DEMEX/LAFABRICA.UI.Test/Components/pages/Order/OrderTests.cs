using Xunit;
using OpenQA.Selenium;
using LAFABRICA.UI.Test.Components;

namespace LAFABRICA.UI.Test.pages.Order
{
    public class OrderTests : BaseTestANGEL
    {
        [Fact]
        public void LoadShowOrdersPage_ShouldDisplayCorrectTitle()
        {
            try
            {
                // Ruta real del listado de órdenes
                _driver.Navigate().GoToUrl($"{_appUrl}/ordenes");
                Thread.Sleep(2000);

                // El título REAL en tu página es un <h3>, no un <h1>
                var pageTitleElement = _driver.FindElement(By.TagName("h3"));

                // Verificamos que el texto mostrado sea el correcto
                Assert.Contains("Gestión de Órdenes", pageTitleElement.Text);
            }
            catch (Exception ex)
            {
                Assert.Fail($"La prueba falló: {ex.Message}");
            }
        }

        // Aquí pondrás tus otras 4 pruebas para "Órdenes"
    }
}
