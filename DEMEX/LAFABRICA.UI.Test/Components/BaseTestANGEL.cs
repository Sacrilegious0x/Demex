using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using WebDriverManager; // <-- El paquete que instalaste
using WebDriverManager.DriverConfigs.Impl; // <-- El paquete que instalaste

// Asegúrate de que el namespace sea el correcto
namespace LAFABRICA.UI.Test.Components
{
    // 'abstract' es una buena práctica y 'IDisposable' es para limpiar
    public abstract class BaseTestANGEL : IDisposable
    {
        // 'protected' significa que solo esta clase y las que heredan (OrderTests)
        // pueden ver estas variables.
        protected readonly IWebDriver _driver;

        // 🔴 ¡OJO 1!
        // Vi que el BaseTest de tu compañero usaba '5290'. 
        // Pon aquí la URL correcta de tu IIS.
        protected readonly string _appUrl = "http://localhost:5290";

        public BaseTestANGEL()
        {
            // 1. Configura el modo Headless (sin pantalla)
            var options = new EdgeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--window-size=1920,1080");

            // 2. WebDriverManager descarga el driver correcto
            new DriverManager().SetUpDriver(new EdgeConfig());

            // 3. Inicializa el driver
            _driver = new EdgeDriver(options);
        }

        public void Dispose()
        {
            // 4. Cierra el navegador al terminar la prueba
            _driver.Quit();
            _driver.Dispose();
        }
    }
}