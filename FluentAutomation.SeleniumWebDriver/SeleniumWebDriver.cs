using System.Reflection;
using FluentAutomation.Interfaces;
using OpenQA.Selenium;

namespace FluentAutomation
{
    /// <summary>
    /// Selenium WebDriver FluentAutomation Provider
    /// </summary>
    public class SeleniumWebDriver
    {
        /// <summary>
        /// Supported browsers for the FluentAutomation Selenium provider.
        /// </summary>
        public enum Browser
        {
            /// <summary>
            /// Internet Explorer. Before using, make sure to set ProtectedMode settings to be the same for all zones.
            /// </summary>
            InternetExplorer = 1,

            /// <summary>
            /// Mozilla Firefox
            /// </summary>
            Firefox = 2,

            /// <summary>
            /// Google Chrome
            /// </summary>
            Chrome = 4
        }

        public enum RuntimeType
        {
            x86,
            x64
        }

        /// <summary>
        /// Currently selected <see cref="Browser"/>.
        /// </summary>
        public static Browser SelectedBrowser;

        /// <summary>
        /// Bootstrap Selenium provider and utilize Firefox.
        /// </summary>
        public static void Bootstrap()
        {
            Bootstrap(Browser.Firefox, RuntimeType.x64);
        }

        /// <summary>
        /// Bootstrap Selenium provider and utilize the specified <paramref name="browser"/>.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="runtime"></param>
        public static void Bootstrap(Browser browser, RuntimeType runtime)
        {
            SeleniumWebDriver.SelectedBrowser = browser;

            FluentAutomation.Settings.Registration = (container) =>
            {
                container.Register<ICommandProvider, CommandProvider>();
                container.Register<IExpectProvider, ExpectProvider>();
                container.Register<IFileStoreProvider, LocalFileStoreProvider>();
            
                switch (SeleniumWebDriver.SelectedBrowser)
                {
                    case Browser.InternetExplorer:
                        EmbeddedResources.UnpackFromAssembly(GetIEDriverExecutableName(runtime), Assembly.GetAssembly(typeof (SeleniumWebDriver)));
                        container.Register<IWebDriver, Wrappers.IEDriverWrapper>().AsMultiInstance();
                        break;
                    case Browser.Firefox:
                        container.Register<IWebDriver, OpenQA.Selenium.Firefox.FirefoxDriver>().AsMultiInstance();
                        break;
                    case Browser.Chrome:
                        EmbeddedResources.UnpackFromAssembly("chromedriver.exe", Assembly.GetAssembly(typeof(SeleniumWebDriver)));
                        container.Register<IWebDriver, OpenQA.Selenium.Chrome.ChromeDriver>().AsMultiInstance();
                        break;
                }
            };
        }

        private static string GetIEDriverExecutableName(RuntimeType runtime)
        {
            return runtime == RuntimeType.x86 ? "IEDriverServer_x86.exe" : "IEDriverServer_x64.exe";
        }
    }
}
