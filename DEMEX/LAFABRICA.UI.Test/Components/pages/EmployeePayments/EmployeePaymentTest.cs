using LAFABRICA.UI.Test.Components.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace LAFABRICA.UI.Test.Components.pages.EmployeePayments
{
    public class EmployeePaymentTest :IClassFixture<BaseTest>
    {
        private readonly IWebDriver _driver;
        private readonly string _baseUrl;
        public EmployeePaymentTest(BaseTest test) 
        {
            _baseUrl = test.BaseUrl;
            _driver = test.Driver;
           
        }
        [Fact]
        public void AddPaymentTest()
        {

            string successText = "Pago registrado correctamente";
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            NavigationHelper.Login(_driver, _baseUrl, "itsgamc@gmail.com", "1234todo");
            NavigationHelper.NavigatetoEmployeePayments(_driver);

            var nuevoPagoBtn = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("nuevoPagoEmpleado")));
            nuevoPagoBtn.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("selectEmployee")));

            // Empleado 
            var selectEmpleado = new SelectElement(_driver.FindElement(By.Id("selectEmployee")));
            selectEmpleado.SelectByValue("6"); //id del usuario carlos

            // Fecha
            var dateInput = _driver.FindElement(By.Id("inputDate"));
            string isoDate = DateTime.Now.ToString("yyyy-MM-dd");

            ((IJavaScriptExecutor)_driver).ExecuteScript(
                "arguments[0].value = arguments[1];",
                dateInput,
                isoDate
            );

            //Reemplaza el valor viejo por el nuevo
            ((IJavaScriptExecutor)_driver).ExecuteScript(
                "arguments[0].dispatchEvent(new Event('change', { bubbles: true }));",
                dateInput
            );
            // Estado 
            var selectEstado = new SelectElement(_driver.FindElement(By.Id("selectState")));
            selectEstado.SelectByText("Pendiente");

            // Descripción
            _driver.FindElement(By.Id("inputDescription")).SendKeys("Pago prueba Selenium");

            // Agregar Producto 1
            var selectProducto = new SelectElement(_driver.FindElement(By.Id("selectProduct")));
            selectProducto.SelectByValue("1"); // id del producto

            _driver.FindElement(By.Id("inputQuantity")).SendKeys("100");
            _driver.FindElement(By.Id("inputUnitPrice")).SendKeys("500");
            _driver.FindElement(By.Id("btnAddProduct")).Click();

            // Agregar Producto 2

            // Espera que se resetee el capo
            wait.Until(ExpectedConditions.TextToBePresentInElementValue(_driver.FindElement(By.Id("inputQuantity")), ""));

            selectProducto = new SelectElement(_driver.FindElement(By.Id("selectProduct")));
            selectProducto.SelectByValue("2");

            _driver.FindElement(By.Id("inputQuantity")).SendKeys("2");
            _driver.FindElement(By.Id("inputUnitPrice")).SendKeys("700");
            _driver.FindElement(By.Id("btnAddProduct")).Click();

            //  Guardar el pago
            var saveBtn = _driver.FindElement(By.Id("btnSave"));
            //click de js mas seguro
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", saveBtn);


            // busca en el div del toast el texto
            var successToastBody = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath($"//div[@id='successToast']//div[@class='toast-body' and contains(text(), '{successText}')]")
            ));

            
            Assert.Contains(successText, successToastBody.Text);

            wait.Until(d => d.Url.Contains("/pagos/empleados"));
            Assert.Contains("/pagos/empleados", _driver.Url);
        }

        [Fact]
        public void AddInvalidPaymentTestWithOutProduct()
        {

            string expectedError = "Debe agregar al menos un producto.";
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));


            NavigationHelper.Login(_driver, _baseUrl, "itsgamc@gmail.com", "1234todo");
            NavigationHelper.NavigatetoEmployeePayments(_driver);


            var nuevoPagoBtn = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("nuevoPagoEmpleado")));
            nuevoPagoBtn.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("selectEmployee")));



            // Empleado
            var selectEmpleado = new SelectElement(_driver.FindElement(By.Id("selectEmployee")));
            selectEmpleado.SelectByValue("6"); // id del usuario carlos

            // Fecha
            var dateInput = _driver.FindElement(By.Id("inputDate"));
            string isoDate = DateTime.Now.ToString("yyyy-MM-dd");
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].value = arguments[1];", dateInput, isoDate);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].dispatchEvent(new Event('change', { bubbles: true }));", dateInput);

            // Estado
            var selectEstado = new SelectElement(_driver.FindElement(By.Id("selectState")));
            selectEstado.SelectByText("Pendiente");

            // Descripción 
            _driver.FindElement(By.Id("inputDescription")).SendKeys("Intento de pago fallido");



            // Guardar el pago 
            var saveBtn = _driver.FindElement(By.Id("btnSave"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", saveBtn);



            var errorElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("errorMessage")));


            Assert.Contains(expectedError, errorElement.Text);

            // Verifica que NO hubo redirección
            Assert.Contains("/pagos/empleados/crear", _driver.Url);
        }

        [Fact]
        public void AddInvalidPaymentTestWithoutEmployee()
        {

            string expectedError = "Debe seleccionar un empleado.";
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));


            NavigationHelper.Login(_driver, _baseUrl, "itsgamc@gmail.com", "1234todo");
            NavigationHelper.NavigatetoEmployeePayments(_driver);

            var nuevoPagoBtn = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("nuevoPagoEmpleado")));
            nuevoPagoBtn.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("selectEmployee")));


            // Fecha 
            var dateInput = _driver.FindElement(By.Id("inputDate"));
            string isoDate = DateTime.Now.ToString("yyyy-MM-dd");
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].value = arguments[1];", dateInput, isoDate);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].dispatchEvent(new Event('change', { bubbles: true }));", dateInput);

            // Estado 
            var selectEstado = new SelectElement(_driver.FindElement(By.Id("selectState")));
            selectEstado.SelectByText("Pendiente");

            // Descripción 
            _driver.FindElement(By.Id("inputDescription")).SendKeys("Intento de pago fallido por falta de empleado.");

            // Agregar Producto 
            var selectProducto = new SelectElement(_driver.FindElement(By.Id("selectProduct")));
            selectProducto.SelectByValue("1"); // id del producto
            _driver.FindElement(By.Id("inputQuantity")).SendKeys("10");
            _driver.FindElement(By.Id("inputUnitPrice")).SendKeys("100");
            _driver.FindElement(By.Id("btnAddProduct")).Click();

            //  Guardar el pago 
            var saveBtn = _driver.FindElement(By.Id("btnSave"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", saveBtn);


            var errorElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("errorMessage")));


            Assert.Contains(expectedError, errorElement.Text);

            // Verificar que NO hubo redirección
            Assert.Contains("/pagos/empleados/crear", _driver.Url);
        }
        [Fact]
        public void EditPaymentTestPay()
        {

            string successText = "Pago actualizado correctamente.";
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            NavigationHelper.Login(_driver, _baseUrl, "itsgamc@gmail.com", "1234todo");
            NavigationHelper.NavigatetoEmployeePayments(_driver);


            var editElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("edit-26")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", editElement);
          
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("selectState")));
            // Estado 
            var selectEstado = new SelectElement(_driver.FindElement(By.Id("selectState")));
            selectEstado.SelectByText("Pagado");

            // Descripción
            var inputDescription = _driver.FindElement(By.Id("inputDescription"));
            inputDescription.Clear();
            inputDescription.SendKeys("Pago editar 26 Selenium - Pagado");
            inputDescription.SendKeys(Keys.Tab);

            var saveBtn = _driver.FindElement(By.Id("btnSave"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", saveBtn);

            //// busca en el div del toast el texto
            var successToastBody = wait.Until(ExpectedConditions.ElementIsVisible(
                          By.XPath($"//div[@id='successToast']//div[@class='toast-body' and contains(text(), '{successText}')]")
                      ));
            Assert.Contains(successText, successToastBody.Text);

            wait.Until(d => d.Url.Contains("/pagos/empleados"));
            Assert.Contains("/pagos/empleados", _driver.Url);
        }

        [Fact]
        public void EditPaymentTestCancel()
        {

            string successText = "Pago actualizado correctamente.";
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            NavigationHelper.Login(_driver, _baseUrl, "itsgamc@gmail.com", "1234todo");
            NavigationHelper.NavigatetoEmployeePayments(_driver);


            var editElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("edit-28")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", editElement);
           
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("selectState")));
            // Estado 
            var selectEstado = new SelectElement(_driver.FindElement(By.Id("selectState")));
            selectEstado.SelectByText("Anulado");

            // Descripción
            var inputDescription = _driver.FindElement(By.Id("inputDescription"));
            inputDescription.Clear();
            inputDescription.SendKeys("Pago editar 28 Selenium - anulado");
            inputDescription.SendKeys(Keys.Tab);

            var saveBtn = _driver.FindElement(By.Id("btnSave"));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", saveBtn);

            //// busca en el div del toast el texto
            var successToastBody = wait.Until(ExpectedConditions.ElementIsVisible(
              By.XPath($"//div[@id='successToast']//div[@class='toast-body' and contains(text(), '{successText}')]")
          ));
            Assert.Contains(successText, successToastBody.Text);
            wait.Until(d => d.Url.Contains("/pagos/empleados"));
            Assert.Contains("/pagos/empleados", _driver.Url);
        }

        //[Fact]
        //public void EditPaymentTestDeleteProduct()
        //{
        //    string successText = "Pago actualizado correctamente.";
        //    var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

        //    // 1. Login y navegación
        //    NavigationHelper.Login(_driver, _baseUrl, "itsgamc@gmail.com", "1234todo");
        //    NavigationHelper.NavigatetoEmployeePayments(_driver);

        //    // 2. Abrir Edición del pago 23
        //    var editElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("edit-23")));
        //    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", editElement);

        //    // 3. Esperar la carga de un producto 
        //    var productItemToDeleteId = "deleteProduct_1";
        //    var productItemToDelete = wait.Until(ExpectedConditions.ElementIsVisible(By.Id(productItemToDeleteId)));

        //    // ELIMINAR
        //    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", productItemToDelete);

        //    // Valida que el producto desapareció 
        //    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id(productItemToDeleteId)));
           
        //    var inputDescription = _driver.FindElement(By.Id("inputDescription"));
        //    inputDescription.Clear();
        //    inputDescription.SendKeys("Eliminado un producto del 23");
        //    inputDescription.SendKeys(Keys.Tab);

        //    var saveBtn = _driver.FindElement(By.Id("btnSave"));
        //    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", saveBtn);

        //    // busca en el div del toast el texto
        //    var successToastBody = wait.Until(ExpectedConditions.ElementIsVisible(
        //      By.XPath($"//div[@id='successToast']//div[@class='toast-body' and contains(text(), '{successText}')]")
        //  ));
        //    Assert.Contains(successText, successToastBody.Text);
        //    wait.Until(d => d.Url.Contains("/pagos/empleados"));
        //    Assert.Contains("/pagos/empleados", _driver.Url);
        //}

    }
}
