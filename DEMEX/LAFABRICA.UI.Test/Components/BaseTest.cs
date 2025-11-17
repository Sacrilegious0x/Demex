using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace LAFABRICA.UI.Test.Components
{
    public class BaseTest : IDisposable
    {
        public IWebDriver Driver { get; private set; }
        public string BaseUrl = "http://localhost:5290";
        public BaseTest()
        {
            var options = new EdgeOptions();
            options.AddArgument("--headless=new");
            //prueha jenkins
            // Estas opciones ayudan a evitar problemas de permisos y memoria en servidores CI
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--window-size=1920,1080");


            Driver = new EdgeDriver(options);
        }

        public void Dispose()
        {
            Driver.Quit();
        }
    }
}
