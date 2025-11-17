using LAFABRICA.UI.Test.Components.Helpers;
using LAFABRICA.UI.Test.Components;
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
    public class SupplierListTest : IClassFixture<BaseTest>
    {
        private readonly IWebDriver _driver;
        private readonly string _baseUrl;

        public SupplierListTest(BaseTest test)
        {
            _baseUrl = test.BaseUrl;
            _driver = test.Driver;
        }

        [Fact]
        public void NavigateToSupplierList()
        {
            NavigationHelper.Login(_driver, _baseUrl, "pablo.ramirezugalde@ucr.ac.cr", "1234qwer");
            NavigationHelper.NavigateToSuppliers(_driver);
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var button = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("newSupplierBtn")));
            Assert.Contains("Nuevo", button.Text);
        }

        [Fact]
        public void NewButton_NavigatesToCreate()
        {
            NavigationHelper.Login(_driver, _baseUrl, "pablo.ramirezugalde@ucr.ac.cr", "1234qwer");
            NavigationHelper.NavigateToSuppliers(_driver);

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var newBtn = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("newSupplierBtn")));
            newBtn.Click();

            wait.Until(d => d.Url.Contains("/proveedor/crear"));
            Assert.Contains("/proveedor/crear", _driver.Url);
        }

        [Fact]
        public void Search_FiltersSupplierList()
        {
            NavigationHelper.Login(_driver, _baseUrl, "pablo.ramirezugalde@ucr.ac.cr", "1234qwer");
            NavigationHelper.NavigateToSuppliers(_driver);

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var search = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[placeholder='Buscar proveedores...']")));

            // esto escribe un termino en la barra
            search.Clear();
            search.SendKeys("NuevoProv");
            //  pequeño lapso para que Blazor procese oninput 
            wait.Until(d => d.FindElements(By.CssSelector("table tbody tr")).Count > 0);

            var rows = _driver.FindElements(By.CssSelector("table tbody tr"));
            Assert.True(rows.Any(), "No se encontraron filas tras buscar.");
            // al menos una fila contiene el texto buscado
            Assert.Contains(rows, r => r.Text.Contains("NuevoProv"));
        }

        [Fact]
        public void DeleteSupplier_ShowsConfirmation_AndRemovesRow()
        {
            NavigationHelper.Login(_driver, _baseUrl, "pablo.ramirezugalde@ucr.ac.cr", "1234qwer");
            NavigationHelper.NavigateToSuppliers(_driver);

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            string supplierName = "COMOARROZ";

            // Esperar a que el botón de eliminar se puea clikiarr
            var deleteButtonLocator = By.XPath($"//table//tbody//tr[td[2][normalize-space()='{supplierName}']]//button[contains(@class,'btn-delete')]");
            IWebElement deleteBtn;
            try
            {
                deleteBtn = wait.Until(ExpectedConditions.ElementToBeClickable(deleteButtonLocator));
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine($"DEBUG: No se pudo encontrar la fila o el botón de eliminar para '{supplierName}'.");
                Console.WriteLine(_driver.PageSource.Substring(0, Math.Min(_driver.PageSource.Length, 4000)));
                throw new Exception($"El proveedor '{supplierName}' no se encontró en la tabla.");
            }

            deleteBtn.Click();

            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("confirmDialogComponent")));
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("DEBUG: El contenedor del diálogo 'confirmDialogComponent' nunca apareció.");
                Console.WriteLine(_driver.PageSource);
                throw new Exception("El diálogo de confirmación (confirmDialogComponent) no se mostró.");
            }

            IWebElement confirmYes;
            //    Esto busca un btn con el texto "Eliminar" DENTRO del diálogo
            var confirmButtonLocator = By.XPath($"//div[@id='confirmDialogComponent']//button[normalize-space()='Eliminar']");

            try
            {
                confirmYes = wait.Until(ExpectedConditions.ElementIsVisible(confirmButtonLocator));
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("DEBUG: No se pudo encontrar el botón de 'Eliminar' dentro del diálogo.");
                Console.WriteLine(_driver.PageSource);
                throw new Exception("El botón 'Eliminar' (buscado por texto) no se encontró o no está visible.");
            }

            //FORZAR EL CLIC USANDO JAVASCRIPT EXECUTOR (muy caballo)
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("arguments[0].click();", confirmYes);


            wait.Until(d => d.FindElements(By.XPath($"//table//tbody//tr[td[2][normalize-space()='{supplierName}']]")).Count == 0);

            var rows = _driver.FindElements(By.CssSelector("table tbody tr"));
            Assert.DoesNotContain(rows, r => r.Text.Contains(supplierName));
        }

        [Fact]
        public void EditSupplier_Cancel_NavigatesToList()
        {
            NavigationHelper.Login(_driver, _baseUrl, "pablo.ramirezugalde@ucr.ac.cr", "1234qwer");
            NavigationHelper.NavigateToSuppliers(_driver);

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            string supplierName = "Nice nature";

            // aqui en ves del id, fue un xpath
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

            // esperar que el InputText del Nombre sea visible.
            var nameInputLocator = By.Id("name");
            wait.Until(ExpectedConditions.ElementIsVisible(nameInputLocator));


            var cancelBtn = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("cancelSupplierBtn")));
            cancelBtn.Click();


            wait.Until(d => d.Url.EndsWith("/proveedor"));
            Assert.Contains("/proveedor", _driver.Url);

            // encabezado de la lista es visible para confirmar la carga
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//h2[contains(text(), 'Gestión de Proveedores')]")));
        }
    }
}
