using NUnit.Framework;
using StyleCop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StyleCop.CSharp;

namespace StyleCopNewRule.Tests
{
    [TestFixture]
    public class MyOwnCustomAnalyzerTests
    {
        private const string TestProjectPath = @"..\..\Mocks\";
        private CodeProject _project;
        private StyleCopConsole _scConsole;
        private List<Violation> _violations;

        [SetUp]
        public void Init() {
            var dir = Path.GetDirectoryName(typeof(MyOwnCustomAnalyzerTests).Assembly.Location);
            Environment.CurrentDirectory = dir ?? throw new InvalidOperationException("Directory can't be null");
            _scConsole = new StyleCopConsole(TestProjectPath, true, null, null, true, null);
            _project = new CodeProject(1, TestProjectPath, new Configuration(new string[0]));
            _scConsole.ViolationEncountered += Console_ViolationEncountered;
            _scConsole.OutputGenerated += Console_OutputGenerated; ;
        }

        private void Console_OutputGenerated(object sender, OutputEventArgs e)
        {
            Console.WriteLine(e.Output);
        }

        private void Console_ViolationEncountered(object sender, ViolationEventArgs e)
        {
            _violations.Add(e.Violation);
        }

        [Test]
        public void InspectCurrentElement_Inherit_From_MVC_Controller_Shoud_Be_Empty_Violations()
        {
            _violations = new List<Violation>();
            _scConsole.Core.Environment.AddSourceCode(_project, TestProjectPath + "RightController.cs", null);
            _scConsole.Start(new List<CodeProject> { _project }, true);

            Assert.AreEqual(0, _violations.Count);
        }

        [Test]
        public void InspectCurrentElement_Shoud_Thow_Violation_By_Name()
        {
            _violations = new List<Violation>();
            _scConsole.Core.Environment.AddSourceCode(_project, TestProjectPath + "WrongController.cs", null);
            _scConsole.Start(new List<CodeProject> { _project }, true);

            Assert.AreEqual(1, _violations.Count);
            Assert.AreEqual("HE2222", _violations[0].Rule.CheckId);
        }

        [Test]
        public void InspectCurrentElement_Inherit_From_Not_MVC_Controller_Shoud_Be_Empty_Violations()
        {
            _violations = new List<Violation>();
            _scConsole.Core.Environment.AddSourceCode(_project, TestProjectPath + "WrongInheritanceController.cs", null);
            _scConsole.Start(new List<CodeProject> { _project }, true);

            Assert.AreEqual(0, _violations.Count);
        }
    }
}
