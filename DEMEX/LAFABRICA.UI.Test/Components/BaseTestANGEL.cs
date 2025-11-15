using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace LAFABRICA.UI.Test.Components
{
    public abstract class BaseTestANGEL : IDisposable
    {
        protected readonly IWebDriver _driver;

        // URL de tu IIS (dejar como estaba)
        protected readonly string _appUrl = "http://localhost:5290";

        public BaseTestANGEL()
        {
            var options = new EdgeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--window-size=1920,1080");

            // 🚫 Se eliminó WebDriverManager (esto rompe Jenkins)
            // ⬇️ Ahora EdgeDriver funciona directo en Jenkins
            _driver = new EdgeDriver(options);
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
