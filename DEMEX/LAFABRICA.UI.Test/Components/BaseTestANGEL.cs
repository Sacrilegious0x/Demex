using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace LAFABRICA.UI.Test.Components
{
    public abstract class BaseTestANGEL : IDisposable
    {
        protected readonly IWebDriver _driver;

        // URL de tu IIS
        protected readonly string _appUrl = "http://localhost:5290";

        public BaseTestANGEL()
        {
            var options = new EdgeOptions();

            // 👇 MODO HEADLESS ESTABLE PARA CI / JENKINS
            options.AddArgument("headless=new");  // Reemplaza el viejo "--headless"
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--remote-debugging-port=0");
            options.AddArgument("--disable-software-rasterizer");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-infobars");

            // 👇 IMPORTANTE EN JENKINS: usar driver del PATH o bin carpeta
            var service = EdgeDriverService.CreateDefaultService();
            service.EnableVerboseLogging = false;
            service.HideCommandPromptWindow = true;

            _driver = new EdgeDriver(service, options);
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
