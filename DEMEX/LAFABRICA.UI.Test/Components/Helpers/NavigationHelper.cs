using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
namespace LAFABRICA.Tests.Components.Helpers
{
    public static class NavigationHelper
    {
        public static void Login(IWebDriver driver,string baseUrl, string email, string password)
        {
            driver.Navigate().GoToUrl($"{baseUrl}/login");
            //Espera para blazor cargue 
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
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
