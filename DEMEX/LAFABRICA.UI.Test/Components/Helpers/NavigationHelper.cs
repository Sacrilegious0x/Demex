using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
namespace LAFABRICA.UI.Test.Components.Helpers
{
    public static class NavigationHelper
    {
        public static void Login(IWebDriver driver,string baseUrl, string email, string password)
        {
            driver.Navigate().GoToUrl($"{baseUrl}/login");
            //Espera para blazor cargue 
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            // 1. Esperar a que Razor Components cargue el motor de interactividad
            wait.Until(d => d.FindElements(By.CssSelector("script[src*='blazor.web']")).Count > 0);

            // 2. Asegurar que estamos en la URL correcta
            wait.Until(ExpectedConditions.UrlContains("/login"));

            // 3. Esperar a que el componente de login se hidrate y cree el DOM real
            wait.Until(d => d.FindElements(By.Id("email")).Count > 0);
            var emailInput = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));
            emailInput.SendKeys(email);
            var passInput = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("password")));
            passInput.SendKeys(password);
            driver.FindElement(By.Id("loginBtn")).Click();
            //var loginBtn = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("loginBtn")));
            //((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", loginBtn);

            
            wait.Until(d => d.FindElement(By.Id("welcomeUser")));
        }

        public static void NavigatetoEmployeePayments(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var dropdown = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("pagosDropdown")));
            dropdown.Click();
            var empleados = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("pagosEmpleados")));
            empleados.Click();
            //Espera a que cargue el boton que contiene la tabla
            wait.Until(d => d.Url.Contains("/pagos/empleados"));
        }

        public static void NavigateToSuppliers(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var list = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("suppliersList")));
            list.Click();
            wait.Until(d => d.Url.Contains("/proveedor"));
        }


    }
}
