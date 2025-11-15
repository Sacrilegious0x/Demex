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
            options.AddArgument("headless=new");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-software-rasterizer");
            options.AddArgument("--window-size=1920,1080");

            var service = EdgeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            _driver = new EdgeDriver(service, options);

            // 🔥 IMPORTANTE: Esperar a que el navegador esté listo antes del login
            Thread.Sleep(1500);

            PerformLogin();
        }


        private void PerformLogin()
{
    _driver.Navigate().GoToUrl($"{_appUrl}/login");

    var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

    // Esperar a que cargue el botón
    var loginBtn = wait.Until(d => d.FindElement(By.Id("loginBtn")));

    var emailInput = wait.Until(d => d.FindElement(By.Id("email")));
    var passInput = wait.Until(d => d.FindElement(By.Id("password")));

    emailInput.SendKeys("Angelbarbozareyes29@gmail.com");
    passInput.SendKeys("An1105667");

    loginBtn.Click();

    // Esperar redirección después de login
    Thread.Sleep(2000);
}

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
