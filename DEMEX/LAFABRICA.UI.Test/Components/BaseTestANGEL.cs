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
        protected WebDriverWait _wait; // Espera principal (larga)
        protected readonly string _appUrl = "http://localhost:8080";

        // Selectores comunes
        private readonly By _emailLocator = By.Id("email");
        private readonly By _passwordLocator = By.Id("password");
        private readonly By _loginBtnLocator = By.Id("loginBtn");
        private readonly By _successLocator = By.Id("welcomeUser");
        private readonly By _errorLocator = By.Id("loginError");

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

            AutenticarUsuario(); // Esta es tu línea 35
        }

        /// <summary>
        /// Lógica de autenticación robusta (V4)
        /// </summary>
        private void AutenticarUsuario()
        {
            // 1. Navegamos a una ruta protegida.
            _driver.Navigate().GoToUrl($"{_appUrl}/ordenes");

            try
            {
                // 2. Esperamos a que aparezca el email (estamos en /login) O
                //    la bienvenida (ya estábamos logueados y nos mandó a /)
                var element = _wait.Until(driver => {
                    var emailInput = driver.FindElements(_emailLocator);
                    var welcomeHeader = driver.FindElements(_successLocator);

                    if (emailInput.Count > 0 && emailInput[0].Displayed)
                    {
                        return emailInput[0]; // Estamos en /login
                    }

                    if (welcomeHeader.Count > 0 && welcomeHeader[0].Displayed)
                    {
                        return welcomeHeader[0]; // Ya estábamos logueados
                    }
                    return null; // Sigue esperando...
                });

                // 3. Si el elemento que apareció es el email, llenamos el login.
                if (element.GetAttribute("id") == "email")
                {
                    // Estamos en la página de Login. Llenamos los campos.
                    var email = element; // Ya sabemos que 'email' existe y es visible.

                    // --- INICIO DE LA MODIFICACIÓN (V4) ---
                    // Usamos _wait.Until para asegurarnos de que estos elementos
                    // también estén listos antes de interactuar con ellos.
                    var pass = _wait.Until(ExpectedConditions.ElementIsVisible(_passwordLocator));
                    var btn = _wait.Until(ExpectedConditions.ElementIsVisible(_loginBtnLocator));
                    // --- FIN DE LA MODIFICACIÓN (V4) ---

                    email.Clear();
                    email.SendKeys("angelbarbozareyes29@gmail.com");
                    pass.Clear();
                    pass.SendKeys("An1105667");
                    btn.Click();

                    // 4. Ahora, esperamos el resultado del login (éxito o fracaso)
                    _wait.Until(driver => {
                        var successEl = driver.FindElements(_successLocator);
                        var errorEl = driver.FindElements(_errorLocator);

                        if (successEl.Count > 0 && successEl[0].Displayed)
                        {
                            return successEl[0]; // Éxito
                        }

                        if (errorEl.Count > 0 && errorEl[0].Displayed)
                        {
                            throw new Exception($"El login falló. Error: '{errorEl[0].Text}'");
                        }
                        return null;
                    });
                }
                // 5. Si el elemento que apareció fue "welcomeUser", no hacemos nada.
            }
            catch (WebDriverTimeoutException)
            {
                throw new Exception($"Timeout: Ni {_emailLocator} (Login) ni {_successLocator} (Home) aparecieron en 15s después de navegar a /ordenes.");
            }
        }

        /// <summary>
        /// Navega a una página y si la sesión expiró, se vuelve a loguear.
        /// </summary>
        protected void SafeNavigate(string relativeUrl)
        {
            _driver.Navigate().GoToUrl($"{_appUrl}{relativeUrl}");

            try
            {
                // Usamos una espera CORTA (3s) para ver si aparece el login
                var shortWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));
                shortWait.Until(ExpectedConditions.ElementIsVisible(_emailLocator));

                // Si no se lanzó la excepción, significa que SÍ apareció el login.
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