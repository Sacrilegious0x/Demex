using Xunit;
using OpenQA.Selenium;
using LAFABRICA.UI.Test.Components;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Support.UI;

namespace LAFABRICA.UI.Test.pages.Order
{
    public class OrderTests : BaseTestANGEL
    {
        [Fact]
        public void LoadShowOrdersPage_ShouldDisplayCorrectTitle()
        {
            try
            {
                // Navegar de forma segura
                SafeNavigate("/ordenes");

                // Esperar que renderice el título correcto
                var title = _wait.Until(
                    ExpectedConditions.ElementExists(By.TagName("h3"))
                );

                Assert.Contains("Gestión de Órdenes", title.Text);
            }
            catch (Exception ex)
            {
                Assert.Fail($"La prueba falló: {ex.Message}");
            }
        }
    }
}
