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
            // Incrementamos un poco el tiempo de espera para CI
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

            AutenticarUsuario();
        }

        /// <summary>
        /// Este es el método corregido.
        /// Ahora espera por el éxito O por el fracaso.
        /// </summary>
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

                // --- INICIO DE LA MODIFICACIÓN ---
                // Espera inteligente: Espera a que aparezca
                // el elemento de bienvenida (éxito) O el de error (fracaso).

                By successLocator = By.Id("welcomeUser"); // De Home.razor
                By errorLocator = By.Id("loginError");   // De Login.razor

                try
                {
                    _wait.Until(driver => {
                        // Usamos FindElements (plural) para no lanzar excepción
                        var successElement = driver.FindElements(successLocator);
                        var errorElement = driver.FindElements(errorLocator);

                        // CASO 1: Éxito
                        if (successElement.Count > 0 && successElement[0].Displayed)
                        {
                            return successElement[0]; // ¡Éxito!
                        }

                        // CASO 2: Fracaso
                        if (errorElement.Count > 0 && errorElement[0].Displayed)
                        {
                            // ¡Fracaso! Lanza una excepción clara.
                            throw new Exception($"El login falló en Selenium. La aplicación mostró: '{errorElement[0].Text}'");
                        }

                        // Si no se encontró ninguno, sigue esperando...
                        return null;
                    });
                }
                catch (WebDriverTimeoutException)
                {
                    // CASO 3: Timeout
                    throw new Exception($"Timeout: Ni {successLocator} ni {errorLocator} aparecieron en 15 segundos.");
                }
                catch (Exception)
                {
                    // Relanza la excepción del "CASO 2"
                    throw;
                }
                // --- FIN DE LA MODIFICACIÓN ---
            }
            // Si no encontramos "email", asumimos que ya estábamos logueados.
            // Validamos por si acaso
            else
            {
                _wait.Until(ExpectedConditions.ElementExists(By.Id("welcomeUser")));
            }
        }


        /// <summary>
        /// Navega a una página y si redirige al login, se vuelve a loguear.
        /// </summary>
        protected void SafeNavigate(string relativeUrl)
        {
            _driver.Navigate().GoToUrl($"{_appUrl}{relativeUrl}");

            // Si en esa ruta te pide login → loguear
            // (La nueva AutenticarUsuario() manejará el login)
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