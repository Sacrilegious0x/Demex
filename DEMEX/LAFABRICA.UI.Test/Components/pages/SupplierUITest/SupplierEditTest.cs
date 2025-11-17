using LAFABRICA.UI.Test.Components;
using LAFABRICA.UI.Test.Components.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAFABRICA.UI.Test.Components.pages.SupplierUITest
{
    public class SupplierEditTest : IClassFixture<BaseTest>
    {

        private readonly IWebDriver _driver;
        private readonly string _baseUrl;

        public SupplierEditTest(BaseTest test)
        {
            _baseUrl = test.BaseUrl;
            _driver = test.Driver;
        }

        [Fact]
        public void EditSupplier_Success_UpdatesAndNavigatesToList()
        {
            NavigationHelper.Login(_driver, _baseUrl, "pablo.ramirezugalde@ucr.ac.cr", "1234qwer");
            NavigationHelper.NavigateToSuppliers(_driver);

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            string supplierName = "Selenium";
            string newNotesText = $"Prueba de edición - {DateTime.Now.ToString("ddMMyyHHmmss")}"; // Coso random para la nota

            var editButtonLocator = By.XPath($"//table//tbody//tr[td[2][normalize-space()='{supplierName}']]//button[contains(@class,'btn-edit')]");
            IWebElement editBtn;
            try
            {
                editBtn = wait.Until(ExpectedConditions.ElementToBeClickable(editButtonLocator));
            }
            catch (WebDriverTimeoutException)
            {
                throw new Exception($"El proveedor '{supplierName}' o su botón de edición no se encontraron en la lista.");
            }
            editBtn.Click();

            //  (esperando el campo de Nombre)
            var nameInputLocator = By.Id("name");
            wait.Until(ExpectedConditions.ElementIsVisible(nameInputLocator));

            // nuevamente, para no modificar el .razor se pde tmbn con xpath, muy loco todo
            var notesAreaLocator = By.XPath("//label[contains(text(), 'Notas adicionales')]/following-sibling::div//textarea");

            IWebElement notesInput = wait.Until(ExpectedConditions.ElementIsVisible(notesAreaLocator));

            // Limpiar y nuevo en textaria
            notesInput.Clear();
            notesInput.SendKeys(newNotesText);
            var saveBtn = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("saveSupplierBtn")));
            saveBtn.Click();

            //Esperar a que el contenedor del diálogo aparezca
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[contains(@class, 'modal-dialog')]")));

            //  Localizar el tonto btn de 'Confirmar' por su texto
            // dice "Confirmar".
            var confirmButtonLocator = By.XPath("//div[contains(@class, 'modal-dialog')]//button[normalize-space()='Confirmar']");

            IWebElement confirmYes;
            try
            {
                // Esperar a que sea visible
                confirmYes = wait.Until(ExpectedConditions.ElementIsVisible(confirmButtonLocator));
            }
            catch (WebDriverTimeoutException)
            {

                throw new Exception("El botón de 'Confirmar' (buscado por texto) no se encontró en el diálogo.");
            }

            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("arguments[0].click();", confirmYes);


            //Esperar a que la navegación a la lista de proveedores se complete (después del guardado)
            // El código Blazor tiene un Task.Delay(650) antes de navegar
            wait.Until(d => d.Url.EndsWith("/proveedor"));
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h2[contains(text(), 'Gestión de Proveedores')]")));


            var rowWithNewNotesLocator = By.XPath($"//table//tbody//tr[td[2][normalize-space()='{supplierName}'] and contains(., '{newNotesText}')]");

            // Si la fila con el texto nico existe, la edicioln fue exitosa
            wait.Until(ExpectedConditions.ElementIsVisible(rowWithNewNotesLocator));

            Assert.Contains("/proveedor", _driver.Url);
        }

    }
}

