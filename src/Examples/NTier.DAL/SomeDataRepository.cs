using System;
using System.Collections.Specialized;

namespace NTier.DAL
{
    public class SomeDataRepository
    {
        public string GetSomeData(NameValueCollection loggingContext)
        {
            var localContext = new NameValueCollection { ["SomeKey"] = "SomeValue" };

            localContext.Add(loggingContext);

            try
            {
                throw new NotImplementedException("Uh oh we forgot to write this method!");
            }
            catch (Exception ex)
            {
                NameValueCollection currentInfo = localContext;

                string route = currentInfo["Route"];

                Console.WriteLine($"Exception encountered in route: {route}");

                ex.Data.Add("Route", route);

                Console.WriteLine(ex.ToString());

                throw;
            }
        }

        public string GetSomeData()
        {
            using (Contextually.Relevant.Info(new NameValueCollection { ["SomeKey"] = "SomeValue" }))
            {
                using (Contextually.Relevant.Info(new NameValueCollection { ["SomeKey"] = "SomeValue2" }))
                {
                    try
                    {
                        throw new NotImplementedException("Uh oh we forgot to write this method!");
                    }
                    catch (Exception ex)
                    {
                        NameValueCollection currentInfo = Contextually.Relevant.Info();

                        string route = currentInfo["Route"];

                        Console.WriteLine($"Exception encountered in route: {route}");

                        Console.WriteLine($"OurKeys: {currentInfo["SomeKey"]}");

                        Console.WriteLine($"Directory: {currentInfo["ExecutingDirectory"]}");

                        ex.Data.Add("Route", route);

                        Console.WriteLine(ex.ToString());

                        throw;
                    }

                }
            }
        }
    }
}
