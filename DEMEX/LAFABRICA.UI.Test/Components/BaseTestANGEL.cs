using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace LAFABRICA.UI.Test.Components
{
    public abstract class BaseTestANGEL : IDisposable
    {
        protected readonly IWebDriver _driver;
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
            options.AddArgument("--remote-debugging-port=0");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-infobars");

            var service = EdgeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            _driver = new EdgeDriver(service, options);

            PerformLogin();
        }

        private void PerformLogin()
        {
            _driver.Navigate().GoToUrl($"{_appUrl}/login");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            // Esperar el botón de login
            var loginButton = wait.Until(d => d.FindElement(By.Id("loginBtn")));

            // Llenar email
            var emailInput = _driver.FindElement(By.Id("email"));
            emailInput.Clear();
            emailInput.SendKeys("angelbarbozareyes29@gmail.com");

            // Llenar contraseña
            var passwordInput = _driver.FindElement(By.Id("password"));
            passwordInput.Clear();
            passwordInput.SendKeys("An1105667");

            // Click en login
            loginButton.Click();

            // Esperar a que desaparezca pantalla login (súper robusto)
            wait.Until(driver => driver.Url != $"{_appUrl}/login");
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
