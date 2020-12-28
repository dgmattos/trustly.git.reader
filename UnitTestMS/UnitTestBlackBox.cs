using bll.lib.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using clihttp = bll.lib.Http;
namespace UnitTestMS
{
    [TestClass]
    public class UnitTestBlackBox
    {
        private clihttp.HttpClient _cli;

        [TestInitialize]
        public void Setup()
        {
            _cli = new clihttp.HttpClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _cli = null;
        }

        [TestMethod]
        public void ApiIsRun()
        {
            var result = _cli._Get("https://maveric.net.br/api/GitHub", "https://github.com/dgmattos/br.net.maveric.util");

            var json = JsonConvert.DeserializeObject<JsonResultDataModel>(result);

            Assert.IsInstanceOfType(json, typeof(JsonResultDataModel));
        }

        [TestMethod]
        public void UrlIsNotGitHub()
        {
            var result = _cli._Get("https://maveric.net.br/api/GitHub", "https://maveric.net.br/api/GitHub");

            JToken jsonResponse = JToken.Parse(result);

            Assert.AreEqual("This url is not a valid github repository.", jsonResponse["Message"]);   
        }

        [TestMethod]
        public void UrlParanNotFound()
        {
            var result = _cli._Get("https://maveric.net.br/api/GitHub", "");

            JToken jsonResponse = JToken.Parse(result);

            Assert.AreEqual("the url parameter does not exist", jsonResponse["Message"]);
        }

        [TestMethod]
        public void UrlParanIsNotUrl()
        {
            var result = _cli._Get("https://maveric.net.br/api/GitHub", "as56d4as654d65as");

            JToken jsonResponse = JToken.Parse(result);

            Assert.AreEqual("Invalid URI: The format of the URI could not be determined.", jsonResponse["Message"]);
        }

        [TestMethod]
        public void UrlNotRepository()
        {
            var result = _cli._Get("https://maveric.net.br/api/GitHub", "https://github.com/");

            JToken jsonResponse = JToken.Parse(result);

            Assert.AreEqual("File name is invalid.", jsonResponse["Message"]);
        }

        [TestMethod]
        public void RepositoryNotFound()
        {
            var result = _cli._Get("https://maveric.net.br/api/GitHub", "https://github.com/dgmattos/MaveriFrameWork");

            JToken jsonResponse = JToken.Parse(result);

            Assert.AreEqual("Repository not found", jsonResponse["Message"]);
        }

        [TestMethod]
        public void directoryNotFound()
        {
            var result = _cli._Get("https://maveric.net.br/api/GitHub", "https://github.com/");

            JToken jsonResponse = JToken.Parse(result);

            Assert.AreNotEqual("Directory not found", jsonResponse["Message"]);
        }
    }
}
