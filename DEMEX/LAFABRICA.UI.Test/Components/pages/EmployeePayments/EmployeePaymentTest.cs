using LAFABRICA.Tests.Components.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace LAFABRICA.Tests.Components.pages.EmployeePayments
{
    public class EmployeePaymentTest :IClassFixture<BaseTest>
    {
        private readonly IWebDriver _driver;
        private readonly string _baseUrl;
        public EmployeePaymentTest(BaseTest test) 
        {
            _baseUrl = test.BaseUrl;
            _driver = test.Driver;
        }
        [Fact]
        public void NavigateToEmployeePayments()
        {
            NavigationHelper.Login(_driver, _baseUrl, "itsgamc@gmail.com", "1234todo");
            NavigationHelper.NavigatetoEmployeePayments(_driver);
            //Console.WriteLine(_driver.PageSource);
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var button = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("nuevoPagoEmpleado")));
            Assert.Contains("Pago", button.Text);
        }
    }
}
