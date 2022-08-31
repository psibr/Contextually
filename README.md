# Contextually

Enables contextual information to _flow_ with your code:
```csharp
using System;
using Contextually;

namespace Sample
{
  class App
  {
    static void Main(string[] args)
    {
      // Setup top level context of the entire app.
      var systemContext = new NameValueCollection
      {
        ["MachineName"] = System.Environment.MachineName
      };
      
      using (Relevant.Info(systemContext))
      {
        // Shows that the context flows even if you start a new thread.
        Task.Run(AppMainLoop).Wait();
      }
      // This is the end of the context scope.
    }
    
    void AppMainLoop()
    {
      Console.WriteLine("Enter your name:");
      var userName = Console.ReadLine();

      var appContext = new NameValueCollection
      {
        ["UserName"] = userName
      };
      
      // Setup a sub-context of relevant information.
      using (Relevant.Info(appContext)
      {
        while (...)
          DoWork();
      }
      // End of the sub-scope. The "UserName" is not
      // available anymore, but 'MachineName' still is.
    }

    void DoWork()
    {
      try
      {
        ...
      }
      catch (Exception ex)
      {
        // Now you can collect all contextual information
        // from all open scopes and sub-scopes.
        var aggregatedContext = Relevant.Info();
        // This helps with analysis of 'dynamic' data,
        // which is not known at compile time.
        Logger.LogError(ex, aggregatedContext);
      }
    }
  }
}
```