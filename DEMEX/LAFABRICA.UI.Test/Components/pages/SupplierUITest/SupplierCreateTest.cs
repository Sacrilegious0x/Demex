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
    public class SupplierCreateTest : IClassFixture<BaseTest>
    {
        private readonly IWebDriver _driver;
        private readonly string _baseUrl;


        public SupplierCreateTest(BaseTest test)
        {
            _baseUrl = test.BaseUrl;
            _driver = test.Driver;
        }

        [Fact]
        public void CreateSupplier_FromCreatePage_Works()
        {
            NavigationHelper.Login(_driver, _baseUrl, "pablo.ramirezugalde@ucr.ac.cr", "1234qwer");
            NavigationHelper.NavigateToSuppliers(_driver);

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
            var newBtn = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("newSupplierBtn")));
            newBtn.Click();

            // Esperar a que la página de crear muestre el input del nombre
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("supplierName")));

            // Generar datos únicos para evitar colisiones
            var uniqueName = "ProveedorTest-" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var email = $"{uniqueName}@example.com";
            var phone = "9090-6776";

            // Rellenar formulario
            var nameInput = _driver.FindElement(By.Id("supplierName"));
            nameInput.Clear();
            nameInput.SendKeys(uniqueName);

            var emailInput = _driver.FindElement(By.Id("supplierEmail"));
            emailInput.Clear();
            emailInput.SendKeys(email);

            var phoneInput = _driver.FindElement(By.Id("supplierPhone"));
            phoneInput.Clear();
            phoneInput.SendKeys(phone);

            var addressInput = _driver.FindElement(By.Id("supplierAddress"));
            addressInput.Clear();
            addressInput.SendKeys("Calle de Prueba 123");

            var notesInput = _driver.FindElement(By.Id("supplierNotes"));
            notesInput.Clear();
            notesInput.SendKeys("Nota de prueba");

            // Click en Guardar
            var saveBtn = _driver.FindElement(By.Id("saveSupplierBtn"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", saveBtn);
            wait.Until(ExpectedConditions.ElementToBeClickable(saveBtn));
            saveBtn.Click();

            bool toastFound = false;
            try
            {
                toastFound = wait.Until(d =>
                {
                    try
                    {
                        var toast = d.FindElement(By.Id("toastContainer"));
                        return toast.Text.Contains("creado correctamente") || toast.Text.Contains(uniqueName);
                    }
                    catch { return false; }
                });
            }
            catch (WebDriverTimeoutException)
            {
                throw new WebDriverTimeoutException("El toast de exito nunca aparecio");
            }

            // Esperar que navegue de vuelta al listado /proveedor
            wait.Until(d => d.Url.Contains("/proveedor"));


            try
            {
                wait.Until(d =>
                {
                    try
                    {
                        // Busca el cuerpo de la tabla
                        var tableBody = d.FindElement(By.CssSelector("table tbody"));
                        // Comprueba si su texto interno contiene el nombre
                        return tableBody.Text.Contains(uniqueName);
                    }
                    catch (NoSuchElementException)
                    {
                        // Si el tbody aún no existe (Blazor está cargando),
                        // retorna false para que la espera continúe
                        return false;
                    }
                });
            }
            catch (WebDriverTimeoutException)
            {

                Console.WriteLine("DEBUG: No se encontró el texto en la tabla. PageSource (parcial):");
                Console.WriteLine(_driver.PageSource.Substring(0, Math.Min(_driver.PageSource.Length, 4000)));
                throw new Exception($"El proveedor '{uniqueName}' no se encontró en la tabla después de guardar.");
            }

            var rows = _driver.FindElements(By.CssSelector("table tbody tr"));

            Assert.Contains(rows, r => r.Text.Contains(uniqueName));



        }

    }
}
