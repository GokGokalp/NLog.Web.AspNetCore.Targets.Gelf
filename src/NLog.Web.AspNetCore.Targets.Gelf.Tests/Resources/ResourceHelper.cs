using System.IO;
using System.Reflection;
using Xunit;

namespace NLog.Web.AspNetCore.Targets.Gelf.Tests.Resources
{
    internal class ResourceHelper
    {
        internal static TextReader GetResource(string filename)
        {
            Assert.NotNull(filename);
            var thisAssembly =  typeof(ResourceHelper).GetTypeInfo().Assembly;
            var resourceFullName = typeof (ResourceHelper).Namespace + "." + filename;
            var manifestResourceStream = thisAssembly.GetManifestResourceStream(resourceFullName);

            Assert.NotNull(manifestResourceStream);

            return new StreamReader(manifestResourceStream);
        }
    }
}