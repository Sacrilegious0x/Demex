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
                // Navegar ya autenticado
                _driver.Navigate().GoToUrl($"{_appUrl}/ordenes");
                Thread.Sleep(2000);

                // El título es un <h3>
                var pageTitleElement = _driver.FindElement(By.TagName("h3"));

                Assert.Contains("Gestión de Órdenes", pageTitleElement.Text);
            }
            catch (Exception ex)
            {
                Assert.Fail($"La prueba falló: {ex.Message}");
            }
        }
    }
}
