// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.NET.TestFramework.Assertions;
using Microsoft.TemplateEngine.TestHelper;
using Xunit;
using Xunit.Abstractions;

namespace Dotnet_new3.IntegrationTests
{
    public class DotnetNewInstantiate
    {
        private readonly ITestOutputHelper _log;

        public DotnetNewInstantiate(ITestOutputHelper log)
        {
            _log = log;
        }

        [Fact]
        public void CanInstantiateTemplate()
        {
            string home = TestUtils.CreateTemporaryFolder("Home");
            string workingDirectory = TestUtils.CreateTemporaryFolder();

            new DotnetNewCommand(_log, "console")
                .WithCustomHive(home)
                .WithWorkingDirectory(workingDirectory)
                .Execute()
                .Should()
                .ExitWith(0)
                .And.NotHaveStdErr()
                .And.HaveStdOutContaining("The template \"Console Application\" was created successfully.");
        }

        [Fact]
        public void CannotInstantiateUnknownTemplate()
        {
            var home = TestUtils.CreateTemporaryFolder("Home");

            new DotnetNewCommand(_log, "webapp", "--quiet")
                .WithCustomHive(home)
                .WithWorkingDirectory(TestUtils.CreateTemporaryFolder())
                .Execute()
                .Should()
                .Fail()
                .And.NotHaveStdOut()
                .And.HaveStdErrContaining("No templates found matching: 'webapp'.")
                .And.HaveStdErrContaining("To list installed templates, run 'dotnet new3 --list'.")
                .And.HaveStdErrContaining("To search for the templates on NuGet.org, run 'dotnet new3 webapp --search'.");
        }

        [Fact]
        public void CanInstantiateTemplateWithSingleNonDefaultLanguageChoice()
        {
            string home = TestUtils.CreateTemporaryFolder("Home");
            string workingDirectory = TestUtils.CreateTemporaryFolder();
            Helpers.InstallTestTemplate("TemplateResolution/DifferentLanguagesGroup/BasicFSharp", _log, workingDirectory, home);

            new DotnetNewCommand(_log, "basic")
                .WithCustomHive(home)
                .WithWorkingDirectory(workingDirectory)
                .Execute()
                .Should()
                .ExitWith(0)
                .And.NotHaveStdErr()
                .And.HaveStdOutContaining("The template \"Basic FSharp\" was created successfully.");
        }

        [Fact]
        public void CannotInstantiateTemplateWhenAmbiguousLanguageChoice()
        {
            string home = TestUtils.CreateTemporaryFolder("Home");
            string workingDirectory = TestUtils.CreateTemporaryFolder();
            Helpers.InstallTestTemplate("TemplateResolution/DifferentLanguagesGroup/BasicFSharp", _log, workingDirectory, home);
            Helpers.InstallTestTemplate("TemplateResolution/DifferentLanguagesGroup/BasicVB", _log, workingDirectory, home);

            new DotnetNewCommand(_log, "basic")
                .WithCustomHive(home)
                .WithWorkingDirectory(workingDirectory)
                .Execute()
                .Should()
                .Fail()
                .And.NotHaveStdOut()
                .And.HaveStdErrContaining("Unable to resolve the template to instantiate, these templates matched your input:")
                .And.HaveStdErrContaining("Re-run the command specifying the language to use with --language option.")
                .And.HaveStdErrContaining("basic").And.HaveStdErrContaining("F#").And.HaveStdErrContaining("VB");
        }

        [Fact]
        public void CannotInstantiateTemplateWhenAmbiguousGroupChoice()
        {
            string home = TestUtils.CreateTemporaryFolder("Home");
            string workingDirectory = TestUtils.CreateTemporaryFolder();

            new DotnetNewCommand(_log, "conf", "--quiet")
                .WithCustomHive(home)
                .WithWorkingDirectory(workingDirectory)
                .Execute()
                .Should()
                .Fail()
                .And.NotHaveStdOut()
                .And.HaveStdErrContaining("Unable to resolve the template to instantiate, these templates matched your input:")
                .And.HaveStdErrContaining("Re-run the command using the template's exact short name.")
                .And.HaveStdErrContaining("webconfig").And.HaveStdErrContaining("nugetconfig").And.NotHaveStdErrContaining("classlib");

            new DotnetNewCommand(_log, "file")
                .WithCustomHive(home)
                .WithWorkingDirectory(workingDirectory)
                .Execute()
                .Should()
                .Fail()
                .And.NotHaveStdOut()
                .And.HaveStdErrContaining("Unable to resolve the template to instantiate, these templates matched your input:")
                .And.HaveStdErrContaining("Re-run the command using the template's exact short name.")
                .And.HaveStdErrContaining("tool-manifest").And.HaveStdErrContaining("sln").And.NotHaveStdErrContaining("console");
        }

        [Fact]
        public void CannotInstantiateTemplateWhenParameterIsInvalid()
        {
            string home = TestUtils.CreateTemporaryFolder("Home");
            string workingDirectory = TestUtils.CreateTemporaryFolder();

            new DotnetNewCommand(_log, "console", "--fake", "--quiet")
                .WithCustomHive(home)
                .WithWorkingDirectory(workingDirectory)
                .Execute()
                .Should()
                .Fail()
                .And.NotHaveStdOut()
                .And.HaveStdErrContaining("Error: Invalid option(s):")
                .And.HaveStdErrContaining("   '--fake' is not a valid option")
                .And.HaveStdErrContaining("For more information, run 'dotnet new3 console --help'.");

            new DotnetNewCommand(_log, "console", "--framework", "fake")
                .WithCustomHive(home)
                .WithWorkingDirectory(workingDirectory)
                .Execute()
                .Should()
                .Fail()
                .And.NotHaveStdOut()
                .And.HaveStdErrContaining("Error: Invalid option(s):")
                .And.HaveStdErrContaining("   'fake' is not a valid value for --framework. The possible values are:")
                .And.HaveStdErrContaining("      net5.0          - Target net5.0")
                .And.HaveStdErrContaining("      netcoreapp3.1   - Target netcoreapp3.1")
                .And.HaveStdErrContaining("For more information, run 'dotnet new3 console --help'.");

            new DotnetNewCommand(_log, "console", "--framework", "netcoreapp")
                .WithCustomHive(home)
                .WithWorkingDirectory(workingDirectory)
                .Execute()
                .Should()
                .Fail()
                .And.NotHaveStdOut()
                .And.HaveStdErrContaining("Error: Invalid option(s):")
                .And.HaveStdErrContaining("   The value 'netcoreapp' is ambiguous for option --framework. The possible values are:")
                .And.HaveStdErrContaining("      netcoreapp2.1   - Target netcoreapp2.1")
                .And.HaveStdErrContaining("      netcoreapp3.1   - Target netcoreapp3.1")
                .And.HaveStdErrContaining("For more information, run 'dotnet new3 console --help'.");

            new DotnetNewCommand(_log, "console", "--framework", "netcoreapp", "--fake")
                .WithCustomHive(home)
                .WithWorkingDirectory(workingDirectory)
                .Execute()
                .Should()
                .Fail()
                .And.NotHaveStdOut()
                .And.HaveStdErrContaining("Error: Invalid option(s):")
                .And.HaveStdErrContaining("   The value 'netcoreapp' is ambiguous for option --framework. The possible values are:")
                .And.HaveStdErrContaining("      netcoreapp2.1   - Target netcoreapp2.1")
                .And.HaveStdErrContaining("      netcoreapp3.1   - Target netcoreapp3.1")
                .And.HaveStdErrContaining("   '--fake' is not a valid option")
                .And.HaveStdErrContaining("For more information, run 'dotnet new3 console --help'.");
        }

        [Fact]
        public void CannotInstantiateTemplateWhenPrecedenceIsSame()
        {
            string home = TestUtils.CreateTemporaryFolder("Home");
            string workingDirectory = TestUtils.CreateTemporaryFolder();
            Helpers.InstallTestTemplate("TemplateResolution/SamePrecedenceGroup/BasicTemplate1", _log, workingDirectory, home);
            Helpers.InstallTestTemplate("TemplateResolution/SamePrecedenceGroup/BasicTemplate2", _log, workingDirectory, home);

            new DotnetNewCommand(_log, "basic")
                .WithCustomHive(home)
                .WithWorkingDirectory(workingDirectory)
                .Execute()
                .Should()
                .Fail()
                .And.NotHaveStdOut()
                .And.HaveStdErrContaining("Unable to resolve the template to instantiate, the following installed templates are conflicting:")
                .And.HaveStdErrContaining("Uninstall the templates or the packages to keep only one template from the list.")
                .And.HaveStdErrContaining("TestAssets.SamePrecedenceGroup.BasicTemplate2")
                .And.HaveStdErrContaining("TestAssets.SamePrecedenceGroup.BasicTemplate1")
                .And.HaveStdErrContaining("basic")
                .And.HaveStdErrContaining("C#")
                .And.HaveStdErrContaining("Test Asset")
                .And.HaveStdErrContaining("100")
                .And.HaveStdErrContaining($"{Path.DirectorySeparatorChar}test_templates{Path.DirectorySeparatorChar}TemplateResolution{Path.DirectorySeparatorChar}SamePrecedenceGroup{Path.DirectorySeparatorChar}BasicTemplate2")
                .And.HaveStdErrContaining($"{Path.DirectorySeparatorChar}test_templates{Path.DirectorySeparatorChar}TemplateResolution{Path.DirectorySeparatorChar}SamePrecedenceGroup{Path.DirectorySeparatorChar}BasicTemplate1");
        }

        [Fact]
        public void CanInstantiateTemplateWithAlias()
        {
            string home = TestUtils.CreateTemporaryFolder("Home");
            string workingDirectory = TestUtils.CreateTemporaryFolder();

            new DotnetNewCommand(_log, "console", "--alias", "csharpconsole")
                .WithCustomHive(home)
                .WithWorkingDirectory(workingDirectory)
                .Execute()
                .Should()
                .ExitWith(0)
                .And.NotHaveStdErr()
                .And.HaveStdOutContaining("Successfully created alias named 'csharpconsole' with value 'console'");

            new DotnetNewCommand(_log, "console", "-n", "MyConsole", "-o", "no-alias")
             .WithCustomHive(home)
             .WithWorkingDirectory(workingDirectory)
             .Execute()
             .Should()
             .ExitWith(0)
             .And.NotHaveStdErr()
             .And.HaveStdOutContaining("The template \"Console Application\" was created successfully.");

            new DotnetNewCommand(_log, "csharpconsole", "-n", "MyConsole", "-o", "alias")
               .WithCustomHive(home)
               .WithWorkingDirectory(workingDirectory)
               .Execute()
               .Should()
               .ExitWith(0)
               .And.NotHaveStdErr()
               .And.HaveStdOutContaining("The template \"Console Application\" was created successfully.")
               .And.HaveStdOutContaining("After expanding aliases, the command is:")
               .And.HaveStdOutContaining("dotnet new3 console -n MyConsole -o alias");

            Assert.Equal(
                new DirectoryInfo(Path.Combine(workingDirectory, "no-alias")).EnumerateFileSystemInfos().Select(fi => fi.Name),
                new DirectoryInfo(Path.Combine(workingDirectory, "alias")).EnumerateFileSystemInfos().Select(fi => fi.Name));

        }
        
        [Fact]
        public void CannotOverwriteFilesWithoutForce()
        {
            string home = TestUtils.CreateTemporaryFolder("Home");
            string workingDirectory = TestUtils.CreateTemporaryFolder();

            new DotnetNewCommand(_log, "console")
                .WithCustomHive(home)
                .WithWorkingDirectory(workingDirectory)
                .Execute()
                .Should()
                .ExitWith(0)
                .And.NotHaveStdErr()
                .And.HaveStdOutContaining("The template \"Console Application\" was created successfully.");

            new DotnetNewCommand(_log, "console")
                .WithCustomHive(home)
                .WithWorkingDirectory(workingDirectory)
                .Execute()
                .Should().Fail()
                .And.HaveStdErrContaining("Creating this template will make changes to existing files:")
                .And.HaveStdErrMatching(@$"  Overwrite   \.[\\\/]{Path.GetFileName(workingDirectory)}\.csproj")
                .And.HaveStdErrMatching(@"  Overwrite   \.[\\\/]Program\.cs")
                .And.HaveStdErrContaining("Rerun the command and pass --force to accept and create.");
        }

        [Fact]
        public void CanOverwriteFilesWithForce()
        {
            string home = TestUtils.CreateTemporaryFolder("Home");
            string workingDirectory = TestUtils.CreateTemporaryFolder();

            var commandResult = new DotnetNewCommand(_log, "console", "--no-restore")
                .WithCustomHive(home).Quietly()
                .WithWorkingDirectory(workingDirectory)
                .Execute();

            commandResult.Should()
                .ExitWith(0)
                .And.NotHaveStdErr()
                .And.HaveStdOutContaining("The template \"Console Application\" was created successfully.");

            var forceCommandResult = new DotnetNewCommand(_log, "console", "--no-restore", "--force")
                .WithCustomHive(home)
                .WithWorkingDirectory(workingDirectory)
                .Execute();

            forceCommandResult.Should()
                .ExitWith(0)
                .And.NotHaveStdErr()
                .And.HaveStdOutContaining("The template \"Console Application\" was created successfully.");

            Assert.Equal(commandResult.StdOut, forceCommandResult.StdOut);
        }    
    }
}
