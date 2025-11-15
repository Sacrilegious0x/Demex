using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace LAFABRICA.UI.Test.Components
{
    public abstract class BaseTestANGEL : IDisposable
    {
        protected readonly IWebDriver _driver;

        // URL de tu IIS (ajusta si usas otro puerto)
        protected readonly string _appUrl = "http://localhost:8080";

        public BaseTestANGEL()
        {
            var options = new EdgeOptions();

            // HEADLESS NUEVO (Edge moderno)
            options.AddArgument("headless=new");

            // Recomendados para Jenkins / servidores
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-software-rasterizer");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--remote-debugging-port=0");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-infobars");

            var service = EdgeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            _driver = new EdgeDriver(service, options);

            // 🔥 LOGIN AUTOMÁTICO
            PerformLogin();
        }

        private void PerformLogin()
        {
            _driver.Navigate().GoToUrl($"{_appUrl}/login");
            Thread.Sleep(1500);

            // Email
            var emailInput = _driver.FindElement(By.Id("email"));
            emailInput.Clear();
            emailInput.SendKeys("Angelbarbozareyes29@gmail.com"); // <-- CAMBIAR SI TU USUARIO ES OTRO

            // Password
            var passInput = _driver.FindElement(By.Id("password"));
            passInput.Clear();
            passInput.SendKeys("An1105667"); // <-- CAMBIAR SI TU CONTRASEÑA ES OTRA

            // Botón
            var loginBtn = _driver.FindElement(By.Id("loginBtn"));
            loginBtn.Click();

            Thread.Sleep(2000); // Espera mínima para Blazor auth
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
