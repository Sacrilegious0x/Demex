using Xunit;
using OpenQA.Selenium;
using LAFABRICA.UI.Test.Components;
using SeleniumExtras.WaitHelpers;

namespace LAFABRICA.UI.Test.pages.Order
{
    public class OrderTests : BaseTestANGEL
    {
        //[Fact]
        //public void LoadShowOrdersPage_ShouldDisplayCorrectTitle()
        //{
        //    // 1. El constructor de BaseTestANGEL ya hizo el login y nos
        //    //    dejó en la página de inicio ("/").

        //    // 2. Navegamos a la página que queremos probar.
        //    SafeNavigate("/ordenes");

        //    // 3. Esperamos que el título H3 sea visible.
        //    //    Usamos ElementIsVisible en lugar de ElementExists
        //    //    porque es más seguro.
        //    var title = _wait.Until(
        //        ExpectedConditions.ElementIsVisible(By.TagName("h3"))
        //    );

        //    // 4. Afirmamos que el texto es correcto.
        //    //    Si esto falla, xUnit marcará la prueba como "roja"
        //    //    automáticamente, no necesitas un try-catch.
        //    Assert.Contains("Gestión de Órdenes", title.Text);
        //}
    }
}