// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.NET.TestFramework.Assertions;
using Microsoft.NET.TestFramework.Commands;
using Microsoft.TemplateEngine.TestHelper;
using Xunit;
using Xunit.Abstractions;

namespace Dotnet_new3.IntegrationTests
{
    public class CommonTemplatesTests : IClassFixture<SharedHomeDirectory>
    {
        private readonly SharedHomeDirectory _fixture;
        private readonly ITestOutputHelper _log;

        public CommonTemplatesTests(SharedHomeDirectory fixture, ITestOutputHelper log)
        {
            _fixture = fixture;
            _log = log;
        }

        [Theory]
        [InlineData("Console Application", "console")]
        [InlineData("Console Application", "console", "C#")]
        [InlineData("Console Application", "console", "F#")]
        [InlineData("Console Application", "console", "VB")]
        [InlineData("Console Application", "console", "C#", "net6.0")]
        [InlineData("Console Application", "console", "F#", "net6.0")]
        [InlineData("Console Application", "console", "VB", "net6.0")]
        [InlineData("Console Application", "console", "C#", "net5.0")]
        [InlineData("Console Application", "console", "F#", "net5.0")]
        [InlineData("Console Application", "console", "VB", "net5.0")]
        [InlineData("Console Application", "console", "C#", "netcoreapp3.1")]
        [InlineData("Console Application", "console", "F#", "netcoreapp3.1")]
        [InlineData("Console Application", "console", "VB", "netcoreapp3.1")]
        [InlineData("Console Application", "console", "C#", "netcoreapp2.1")]
        [InlineData("Console Application", "console", "F#", "netcoreapp2.1")]
        [InlineData("Console Application", "console", "VB", "netcoreapp2.1")]

        [InlineData("Class Library", "classlib")]
        [InlineData("Class Library", "classlib", "C#")]
        [InlineData("Class Library", "classlib", "F#")]
        [InlineData("Class Library", "classlib", "VB")]
        [InlineData("Class Library", "classlib", "C#", "net6.0")]
        [InlineData("Class Library", "classlib", "F#", "net6.0")]
        [InlineData("Class Library", "classlib", "VB", "net6.0")]
        [InlineData("Class Library", "classlib", "C#", "net5.0")]
        [InlineData("Class Library", "classlib", "F#", "net5.0")]
        [InlineData("Class Library", "classlib", "VB", "net5.0")]
        [InlineData("Class Library", "classlib", "C#", "netcoreapp3.1")]
        [InlineData("Class Library", "classlib", "F#", "netcoreapp3.1")]
        [InlineData("Class Library", "classlib", "VB", "netcoreapp3.1")]
        [InlineData("Class Library", "classlib", "C#", "netcoreapp2.1")]
        [InlineData("Class Library", "classlib", "F#", "netcoreapp2.1")]
        [InlineData("Class Library", "classlib", "VB", "netcoreapp2.1")]
        public void AllCommonProjectsCreateRestoreAndBuild(string expectedTemplateName, string templateShortName, string? language = null, string? framework = null)
        {
            string workingDir = TestUtils.CreateTemporaryFolder();
            string workingDirName = Path.GetFileName(workingDir);
            string extension = language switch
            {
                "F#" => "fsproj",
                "VB" => "vbproj",
                _ => "csproj"
            };
            string finalProjectName = Regex.Escape(Path.Combine(workingDir, $"{workingDirName}.{extension}"));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                //on OSX path in restore starts from /private for some reason
                finalProjectName = "/private" + finalProjectName;
            }
            Console.WriteLine($"Expected project location: {finalProjectName}");

            List<string> args = new List<string>() { templateShortName };
            if (!string.IsNullOrWhiteSpace(language))
            {
                args.Add("--language");
                args.Add(language);
            }
            if (!string.IsNullOrWhiteSpace(framework))
            {
                args.Add("--framework");
                args.Add(framework);
            }

            new DotnetNewCommand(_log, args.ToArray())
                .WithCustomHive(_fixture.HomeDirectory)
                .WithWorkingDirectory(workingDir)
                .Execute()
                .Should()
                .ExitWith(0)
                .And.NotHaveStdErr()
                .And.HaveStdOutMatching(
$@"The template ""{expectedTemplateName}"" was created successfully\.

Processing post-creation actions\.\.\.
Running 'dotnet restore' on ({finalProjectName})\.\.\.
  Determining projects to restore\.\.\.
  Restored ({finalProjectName}) \(in \d{{1,3}} ms|\d(\.\d{{1,3}}){{0,1}} sec\)\.

Restore succeeded\.");

            new DotnetCommand(_log, "restore")
                .WithWorkingDirectory(workingDir)
                .Execute()
                .Should()
                .ExitWith(0)
                .And
                .NotHaveStdErr();

            new DotnetCommand(_log, "build")
                .WithWorkingDirectory(workingDir)
                .Execute()
                .Should()
                .ExitWith(0)
                .And
                .NotHaveStdErr();

            Directory.Delete(workingDir, true);
        }

        [Theory]
        [InlineData("Console Application", "console")]
        [InlineData("Console Application", "console", "C#")]
        [InlineData("Console Application", "console", "F#")]
        [InlineData("Console Application", "console", "VB")]
        [InlineData("Console Application", "console", "C#", "net6.0")]
        [InlineData("Console Application", "console", "F#", "net6.0")]
        [InlineData("Console Application", "console", "VB", "net6.0")]
        [InlineData("Console Application", "console", "C#", "net5.0")]
        [InlineData("Console Application", "console", "F#", "net5.0")]
        [InlineData("Console Application", "console", "VB", "net5.0")]
        [InlineData("Console Application", "console", "C#", "netcoreapp3.1")]
        [InlineData("Console Application", "console", "F#", "netcoreapp3.1")]
        [InlineData("Console Application", "console", "VB", "netcoreapp3.1")]
        [InlineData("Console Application", "console", "C#", "netcoreapp2.1")]
        [InlineData("Console Application", "console", "F#", "netcoreapp2.1")]
        [InlineData("Console Application", "console", "VB", "netcoreapp2.1")]

        [InlineData("Class Library", "classlib")]
        [InlineData("Class Library", "classlib", "C#")]
        [InlineData("Class Library", "classlib", "F#")]
        [InlineData("Class Library", "classlib", "VB")]
        [InlineData("Class Library", "classlib", "C#", "net6.0")]
        [InlineData("Class Library", "classlib", "F#", "net6.0")]
        [InlineData("Class Library", "classlib", "VB", "net6.0")]
        [InlineData("Class Library", "classlib", "C#", "net5.0")]
        [InlineData("Class Library", "classlib", "F#", "net5.0")]
        [InlineData("Class Library", "classlib", "VB", "net5.0")]
        [InlineData("Class Library", "classlib", "C#", "netcoreapp3.1")]
        [InlineData("Class Library", "classlib", "F#", "netcoreapp3.1")]
        [InlineData("Class Library", "classlib", "VB", "netcoreapp3.1")]
        [InlineData("Class Library", "classlib", "C#", "netcoreapp2.1")]
        [InlineData("Class Library", "classlib", "F#", "netcoreapp2.1")]
        [InlineData("Class Library", "classlib", "VB", "netcoreapp2.1")]
        public void AllCommonProjectsCreate_NoRestore(string expectedTemplateName, string templateShortName, string? language = null, string? framework = null)
        {
            string workingDir = TestUtils.CreateTemporaryFolder();

            List<string> args = new List<string>() { templateShortName, "--no-restore" };
            if (!string.IsNullOrWhiteSpace(language))
            {
                args.Add("--language");
                args.Add(language);
            }
            if (!string.IsNullOrWhiteSpace(framework))
            {
                args.Add("--framework");
                args.Add(framework);
            }

            new DotnetNewCommand(_log, args.ToArray())
                .WithCustomHive(_fixture.HomeDirectory)
                .WithWorkingDirectory(workingDir)
                .Execute()
                .Should()
                .ExitWith(0)
                .And.NotHaveStdErr()
                .And.HaveStdOut($@"The template ""{expectedTemplateName}"" was created successfully.");

            Directory.Delete(workingDir, true);
        }

        [Theory]
        [InlineData("dotnet gitignore file", "gitignore")]
        [InlineData("global.json file", "globaljson")]
        [InlineData("NuGet Config", "nugetconfig")]
        [InlineData("Solution File", "sln")]
        [InlineData("Dotnet local tool manifest file", "tool-manifest")]
        [InlineData("Web Config", "webconfig")]
        public void AllCommonItemsCreate(string expectedTemplateName, string templateShortName)
        {
            string workingDir = TestUtils.CreateTemporaryFolder();

            new DotnetNewCommand(_log, templateShortName)
                .WithCustomHive(_fixture.HomeDirectory)
                .WithWorkingDirectory(workingDir)
                .Execute()
                .Should()
                .ExitWith(0)
                .And.NotHaveStdErr()
                .And.HaveStdOut($@"The template ""{expectedTemplateName}"" was created successfully.");

            Directory.Delete(workingDir, true);
        }
    }
}
