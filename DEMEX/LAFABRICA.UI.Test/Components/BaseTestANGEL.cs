using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace LAFABRICA.UI.Test.Components
{
    public abstract class BaseTestANGEL : IDisposable
    {
        protected readonly IWebDriver _driver;

        // URL DE TU APLICACIÓN
        protected readonly string _appUrl = "http://localhost:8080";

        public BaseTestANGEL()
        {
            var options = new EdgeOptions();

            // HEADLESS
            options.AddArgument("headless=new");

            // NECESARIO PARA JENKINS / SERVIDOR
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-software-rasterizer");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-infobars");

            var service = EdgeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            _driver = new EdgeDriver(service, options);

            AutenticarUsuario(); // 👈 SE AGREGA LOGIN AUTOMÁTICO
        }

        private void AutenticarUsuario()
        {
            _driver.Navigate().GoToUrl($"{_appUrl}/login");

            // Email
            var emailInput = _driver.FindElement(By.Id("email"));
            emailInput.SendKeys("angelbarbozareyes29@gmail.com");

            // Password
            var passInput = _driver.FindElement(By.Id("password"));
            passInput.SendKeys("An1105667");

            // Botón login
            var loginBtn = _driver.FindElement(By.Id("loginBtn"));
            loginBtn.Click();

            // Esperamos hasta que deje de estar en login
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            wait.Until(driver => !driver.Url.Contains("login"));
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
