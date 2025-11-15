using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace LAFABRICA.UI.Test.Components
{
    public abstract class BaseTestANGEL : IDisposable
    {
        protected readonly IWebDriver _driver;
        protected WebDriverWait _wait;
        protected readonly string _appUrl = "http://localhost:8080";

        public BaseTestANGEL()
        {
            var options = new EdgeOptions();
            options.AddArgument("headless=new");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--window-size=1920,1080");

            var service = EdgeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            _driver = new EdgeDriver(service, options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(12));

            AutenticarUsuario();
        }

        private void AutenticarUsuario()
        {
            // Fuerza a IIS a redirigirnos a /login
            _driver.Navigate().GoToUrl($"{_appUrl}/ordenes");

            // Si aparece el login, lo llenamos
            if (IsElementPresent(By.Id("email")))
            {
                var email = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email")));
                var pass = _driver.FindElement(By.Id("password"));
                var btn = _driver.FindElement(By.Id("loginBtn"));

                email.Clear();
                email.SendKeys("angelbarbozareyes29@gmail.com");

                pass.Clear();
                pass.SendKeys("An1105667");

                btn.Click();
            }

            // Validar que ahora estamos en el home
            _wait.Until(ExpectedConditions.ElementExists(By.Id("welcomeUser")));
        }

        /// <summary>
        /// Navega a una página y si redirige al login, se vuelve a loguear.
        /// </summary>
        protected void SafeNavigate(string relativeUrl)
        {
            _driver.Navigate().GoToUrl($"{_appUrl}{relativeUrl}");

            // Si en esa ruta te pide login → loguear
            if (IsElementPresent(By.Id("email")))
            {
                AutenticarUsuario();
                _driver.Navigate().GoToUrl($"{_appUrl}{relativeUrl}");
            }
        }

        protected bool IsElementPresent(By by)
        {
            try
            {
                _driver.FindElement(by);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            try
            {
                _driver.Quit();
                _driver.Dispose();
            }
            catch { }
        }
    }
}
