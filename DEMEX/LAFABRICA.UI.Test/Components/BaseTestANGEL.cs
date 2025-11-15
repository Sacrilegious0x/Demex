using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace LAFABRICA.UI.Test.Components
{
    public abstract class BaseTestANGEL : IDisposable
    {
        protected readonly IWebDriver _driver;
        protected WebDriverWait _wait; // Espera principal (larga)
        protected readonly string _appUrl = "http://localhost:8080";

        // Selectores comunes
        private readonly By _emailLocator = By.Id("email");
        private readonly By _passwordLocator = By.Id("password");
        private readonly By _loginBtnLocator = By.Id("loginBtn");
        private readonly By _successLocator = By.Id("welcomeUser");
        private readonly By _errorLocator = By.Id("loginError");

        // Selector para el botón en Home.razor <NotAuthorized>
        private readonly By _goToLoginBtnLocator = By.Id("goToLoginBtn");

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
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

            AutenticarUsuario();
        }

        private void TakeScreenshot(string stepName)
        {
         
        }

        /// <summary>
        /// Lógica de autenticación robusta (V6)
        /// </summary>
        private void AutenticarUsuario()
        {
            // 1. Navegamos a una ruta protegida.
            _driver.Navigate().GoToUrl($"{_appUrl}/ordenes");

            try
            {
                // 2. ESPERA INICIAL (3 POSIBILIDADES)
                var element = _wait.Until(driver => {
                    var emailInput = driver.FindElements(_emailLocator);
                    var welcomeHeader = driver.FindElements(_successLocator);
                    var restrictedAccessBtn = driver.FindElements(_goToLoginBtnLocator); // <-- NUEVO

                    if (emailInput.Count > 0 && emailInput[0].Displayed)
                        return emailInput[0]; // Estamos en /login

                    if (welcomeHeader.Count > 0 && welcomeHeader[0].Displayed)
                        return welcomeHeader[0]; // Ya estábamos logueados

                    if (restrictedAccessBtn.Count > 0 && restrictedAccessBtn[0].Displayed)
                        return restrictedAccessBtn[0]; // Estamos en Home (Acceso Restringido)

                    return null; // Sigue esperando...
                });

                string elementId = element.GetAttribute("id");
                bool needsLogin = false;

                // 3. CASO: ESTAMOS EN HOME (ACCESO RESTRINGIDO)
                if (elementId == "goToLoginBtn")
                {
                    element.Click(); // Clic en "Ir al inicio de sesión"
                    needsLogin = true;
                }

                // 4. CASO: ESTAMOS EN /LOGIN (o acabamos de llegar desde 'goToLoginBtn')
                if (elementId == "email" || needsLogin)
                {
                    // Esperamos (o re-esperamos) a que los elementos del login estén listos
                    var email = _wait.Until(ExpectedConditions.ElementIsVisible(_emailLocator));
                    var pass = _wait.Until(ExpectedConditions.ElementIsVisible(_passwordLocator));
                    var btn = _wait.Until(ExpectedConditions.ElementIsVisible(_loginBtnLocator));

                    email.Clear();
                    email.SendKeys("angelbarbozareyes29@gmail.com");
                    pass.Clear();
                    pass.SendKeys("An1105667");
                    btn.Click();

                    // 5. Esperar resultado del login (éxito o fracaso)
                    _wait.Until(driver => {
                        var successEl = driver.FindElements(_successLocator);
                        var errorEl = driver.FindElements(_errorLocator);

                        if (successEl.Count > 0 && successEl[0].Displayed)
                            return successEl[0]; // Éxito

                        if (errorEl.Count > 0 && errorEl[0].Displayed)
                            throw new Exception($"El login falló. Error: '{errorEl[0].Text}'");

                        return null;
                    });
                }

                // 6. CASO: ESTAMOS EN HOME (YA LOGUEADOS)
                // Si elementId == "welcomeUser", no hacemos nada. El constructor termina.
            }
            catch (WebDriverTimeoutException)
            {
                TakeScreenshot("Timeout_Auth_Flow");
                throw new Exception($"Timeout: Ni {_emailLocator}, ni {_successLocator}, ni {_goToLoginBtnLocator} aparecieron en 15s. Se tomó screenshot.");
            }
        }


        protected void SafeNavigate(string relativeUrl)
        {
            _driver.Navigate().GoToUrl($"{_appUrl}{relativeUrl}");
            try
            {
                // Usamos una espera CORTA (3s) para ver si aparece el login o la pág de acceso restringido
                var shortWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));
                shortWait.Until(driver =>
                    driver.FindElement(_emailLocator).Displayed ||
                    driver.FindElement(_goToLoginBtnLocator).Displayed
                );

                // Si no se lanzó la excepción, significa que SÍ apareció una pantalla de login/acceso.
                AutenticarUsuario();

                // Reintentamos la navegación original
                _driver.Navigate().GoToUrl($"{_appUrl}{relativeUrl}");
            }
            catch (WebDriverTimeoutException)
            {
                // No apareció el login en 3s. Asumimos que la navegación fue exitosa.
            }
        }

        protected bool IsElementPresent(By by)
        {
            try
            {
                _driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
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