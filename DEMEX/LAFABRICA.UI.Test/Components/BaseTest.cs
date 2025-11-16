using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using System;

namespace LAFABRICA.Tests.Components
{
    public class BaseTest : IDisposable
    {
        public IWebDriver Driver { get; private set; }
        public string BaseUrl = "http://localhost:5290";
        public BaseTest()
        {
            var options = new ChromeOptions();
            //options.AddArguments("--start-maximized"); //tamanho de la ventana en edge
            // PARA JENKINS LUEGO (invisible)
            options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-dev-shm-usage");

            Driver = new ChromeDriver(options);
        }

        public void Dispose()
        {
            Driver.Quit();
            Driver.Dispose();
        }
    }
}
