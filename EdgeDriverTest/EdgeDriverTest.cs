using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.Linq;
using Newtonsoft.Json;
using bll.lib.Model;
using System.Threading;

namespace EdgeDriverTest
{
    [TestClass]
    public class EdgeDriverTest
    {
        // In order to run the below test(s), 
        // please follow the instructions from http://go.microsoft.com/fwlink/?LinkId=619687
        // to install Microsoft WebDriver.

        private EdgeDriver _driver;
        
        private readonly string _srv_url = @"https://maveric.net.br/";

        private readonly string dir_WebDriver = @"E:\WebDrivers";

        [TestInitialize]
        public void EdgeDriverInitialize()
        {
            // Initialize edge driver 
            var options = new EdgeOptions
            {
                PageLoadStrategy = PageLoadStrategy.Normal,
                
                
            };
            
           

            _driver = new EdgeDriver(dir_WebDriver,options);
        }

        [TestMethod]
        public void VerifyServerIsRun()
        {
            // Replace with your own test logic
            _driver.Url = _srv_url+ "api/GitHub?url=https://github.com/dgmattos/br.net.maveric.util";
            
            var ed = _driver.FindElement(By.TagName("pre"));

            var json = JsonConvert.DeserializeObject<JsonResultDataModel>(ed.Text);

            //Assert.AreEqual("This url is not a valid github repository.", ed);

            Assert.IsInstanceOfType(json, typeof(JsonResultDataModel));
        }

        [TestMethod]
        public void VerifyIsNotGitHub()
        {
            // Replace with your own test logic
            _driver.Url = _srv_url + "api/GitHub?url="+ "http://dgmattos.com.br/";
            
            IWebElement el = _driver.FindElement(By.XPath("//*[@id=\"webkit-xml-viewer-source-xml\"]/Error/Message"));

            Assert.AreEqual("This url is not a valid github repository.", el);
        }

        [TestCleanup]
        public void EdgeDriverCleanup()
        {
            _driver.Quit();
        }
    }
}
