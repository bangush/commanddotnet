# CommandDotNet

[![Build status](https://ci.appveyor.com/api/projects/status/0q3laab22dy66sm7/branch/master?svg=true)](https://ci.appveyor.com/project/bilal-fazlani/commanddotnet/branch/master)

## Installation

From nuget: https://www.nuget.org/packages/CommandDotNet


## Introduction

Let's say you want to create a calculator console application which can perform 2 operations:

1. Addition
2. Subtraction

It prints the results on console.

Let's begin with creating the class

```c#
    public class Calculator
    {
        public void Add(int value1, int value2)
        {
            Console.WriteLine($"Answer:  {value1 + value2}");
        }

        public void Subtract(int value1, int value2)
        {
            Console.WriteLine($"Answer:  {value1 - value2}");
        }
    }
```

Now that we have our calculator ready, let's see about how we can call it from command line.


```c#
    class Program
    {
        static void Main(string[] args)
        {
            AppRunner<Calculator> appRunner = new AppRunner<Calculator>();
            int exitCode = appRunner.Run(args);
            Environment.Exit(exitCode);
        }
    }
```

Assuming our application's name is `example.dll`

let's try and run this app from command line using dotnet

INPUT

```bash
dotnet example.dll --help
```

OUTPUT

```bash
Usage: dotnet example.dll [options] [command]

Options:
  -h | -? | --help  Show help information

Commands:
  Add
  Subtract

Use "dotnet example.dll [command] --help" for more information about a command.

```

Voila!

So, as you might have already guessed, it is detecting methods of the calculator class. How about adding some helpful description.

```c#
        [ApplicationMetadata(Description = "Adds two numbers. duh!")]
        public void Add(int value1, int value2)
        {
            Console.WriteLine($"Answer: {value1 + value2}");
        }
```

This should do it.

Let's see how the help appears now.

```bash
Usage: dotnet example.dll [options] [command]

Options:
  -h | -? | --help  Show help information

Commands:
  Add        Adds two numbers. duh!
  Subtract

Use "dotnet example.dll [command] --help" for more information about a command.

```

Awesome. Descriptions are not required but can be very useful depending upon the complexity of your app and the audience. 

Now let's try to see if we can get further help for the add command.

INPUT

```bash
dotnet example.dll Add --help
```

OUTPUT

```bash
Usage: dotnet example.dll Add [options]

Options:
  -h | -? | --help  Show help information
  --value1          Int32 | Required
  --value2          Int32 | Required
```

tada!

Ok, so here, it show what parameters are required for addition and their type.

Let's try and add two numbers.

INPUT

```bash
dotnet example.dll Add --value1 40 --value2 20
```

OUTPUT

```bash
Answer: 60
```

Cool. You get the gist of this library. Let's move on.

## Constructor parameters

Let's say we want to add a class level field which is useful in both Addtion and Subtraction. So now the class looks something like this-

```c#
    public class Calculator
    {
        private readonly bool _printValues;

        public Calculator(bool printValues)
        {
            _printValues = printValues;
        }
        
        [ApplicationMetadata(Description = "Adds two numbers. duh!")]
        public void Add(int value1, int value2)
        {
            if (_printValues)
            {
                Console.WriteLine($"value1 : {value1}, value2: {value2}");
            }
            Console.WriteLine($"Answer:  {value1 + value2}");
        }

        public void Subtract(int value1, int value2)
        {
            if (_printValues)
            {
                Console.WriteLine($"value1 : {value1}, value2: {value2}");
            }
            Console.WriteLine($"Answer: {value1 - value2}");
        }
    }
```

Let's see what the help command output looks like now

INPUT

```bash
dotnet example.dll --help
```

OUTPUT

```bash
Usage: dotnet example.dll [options] [command]

Options:
  -h | -? | --help  Show help information
  --printValues     Boolean | Required

Commands:
  Add        Adds two numbers. duh!
  Subtract

Use "dotnet example.dll [command] --help" for more information about a command.
```

Let's try and invoke it

INPUT 

```bash
dotnet example.dll --printValues true Subtract --value1 30 --value2 5
```

OUTPUT

```bash
value1 : 30, value2: 5
Answer: 25
```

**Note that you can skip to pass any parameter. It will then fallback to the default value of parameter type**

In this case, for `--printValues` it will fallback to `false` & if you dont pass either `--value1` or `--value2`, it will fallback to `0`.