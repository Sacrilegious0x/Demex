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
                _driver.Navigate().GoToUrl($"{_appUrl}/ordenes");
                Thread.Sleep(2000); // Blazor tarda un poquito

                // Tu página usa <h3>
                var pageTitleElement = _driver.FindElement(By.TagName("h3"));

                // Texto REAL de tu página
                Assert.Contains("Gestión de Órdenes", pageTitleElement.Text);
            }
            catch (Exception ex)
            {
                Assert.Fail($"La prueba falló: {ex.Message}");
            }
        }

        // Aquí van las otras pruebas de Órdenes
    }
}
