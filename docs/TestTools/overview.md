# Overview

#### TLDR, How to enable 

nuget package: [CommandDotNet.TestTools](https://www.nuget.org/packages/CommandDotNet.TestTools)

=== ".NET CLI"

    ```
    dotnet add package CommandDotNet.TestTools
    ```
    
=== "Nuget Package Manager"

    ```
    Install-Package CommandDotNet.TestTools

    ```

## Testing Console Apps

One of the perks of using this framework is that commands are just methods and methods are easily unit tested. Most of your tests can be unit tests, as is best-practice.

These tools enable you to provide end-to-end testing as if running the app in a console.

If you're using the [.UseDefaultMiddleware()](../OtherFeatures/default-middleware.md) method, testing as this layer will help identify bugs on upgrade due to new opt-in features.

!!! Note
    These test tools are used to test all of the CommandDotNet features.<br/>They are well suited to testing middleware and other extensibility components. 

## Testing the AppRunner

The tool provides two extension methods to execute an AppRunner in memory and collect the results.

=== "RunInMem"

    ```c#
    public class PipedInputTests
    {
        [Test]
        public void PipedInput_Should_UnionWithUserSuppliedValues()
        {
            var result = new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .RunInMem("List aaa bbb", pipedInput: new[] { "ccc", "ddd" });

            result.ExitCode.Should().Be(0);
            result.OutputShouldBe(@"aaa
    bbb
    ccc
    ddd
    ");
        }

        private class App
        {
            public void List(IConsole console, List<string> args) =>
                console.WriteLine(string.Join(Environment.NewLine, args));
        }
    }
    ```

=== "BDD Verify"

    ```c#
    public class PipedInputTests
    {
        [Test]
        public void PipedInput_Should_UnionWithUserSuppliedValues()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .Verify(new Scenario
                {
                    When = 
                    {
                        Args = "List aaa bbb",
                        PipedInput = new[] { "ccc", "ddd" } 
                    },
                    Then =
                    {
                        Output = @"aaa
    bbb
    ccc
    ddd
    "
                    }
                });
        }

        private class App
        {
            public void List(IConsole console, List<string> args) =>
                console.WriteLine(string.Join(Environment.NewLine, args));
        }
    }
    ```

[RunInMem](Harness/run-in-mem.md) will run the runner and collect results. Assertions will need to be executed after.

[Verify](Harness/bdd.md) wraps `RunInMem` with declarative BDD style setup and assertions.

## Testing your application

When testing an application, use the same method to generate and configure the AppRunner for the console and tests. In this example, the `GetAppRunner()` method is made public so tests can verify the exact config used in the application.

```c#
public class Program
{
        static int Main(string[] args)
        {
            Debugger.AttachIfDebugDirective(args);

            return GetAppRunner().Run(args);
        }

        public static AppRunner GetAppRunner()
        {    
            new AppRunner<Git>()
                .UseDefaultMiddleware()
                .UseNameCasing(Case.KebabCase)
                .UseFluentValidation();
        }
}
```

```c#
[TestFixture]
public class ProgramTests
{
    [Test]
    public void Checkout_NewBranch_WithoutBranchFlag_Fails()
    {
        Program.GetAppRunner()
            .Verify(new Scenario
            {
                When = { Args = "checkout lala" },
                Then = { 
                    Output = "error: pathspec 'lala' did not match any file(s) known to git" 
                }
            });
    }

    [Test]
    public void Checkout_NewBranch_BranchFlag_Succeeds()
    {
        Program.GetAppRunner()
            .Verify(new Scenario
            {
                When = { Args = "checkout -b lala" },
                Then = { 
                    Output = "Switched to a new branch 'lala'" 
                }
            });
    }
}
```

## Included test tools

The framework includes the following tools that can be used independently of the BDD Framework.

### [TestConsole](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TestConsole.cs)

* capture output for assertions
* provide piped input
* handle ReadLine and ReadToEnd

### [TempFiles](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TempFiles.cs)

* Creates temp files, give content and receive a file path.
* Removes created files on dispose
* Examples: [ResponseFileTests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/ResponseFileTests.cs)

### [TestDependencyResolver](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TestDependencyResolver.cs) 

```c#
new AppRunner<App>()
    .UseDependencyResolver(new TestDependencyResolver { dbSvc, httpSvc })
    .VerifyScenario(scenario);
```

### [TestCaptures](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TestCaptures.cs)

`TestCaptures` can be added as a property to a test app and will be automatically injected by the test framework.

This is useful for testing middleware and other extensions that can populate or modify arguments, by allowing you to capture those arguments and assert them in tests.

### [CaptureState](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/AppRunnerTestExtensions.cs#L20)

* `appRunner.CaptureState(...)` extension method that can be used to capture the point-in-time state of an object within the middleware pipeline.
* Examples: [DefaultArityTests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/Arguments/DefaultArityTests.cs) and [Options_Name_Tests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/Arguments/Options_Name_Tests.cs)
