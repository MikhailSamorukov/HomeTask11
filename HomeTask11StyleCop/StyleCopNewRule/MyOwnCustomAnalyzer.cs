using System;
using System.Collections.Generic;
using System.Linq;

namespace StyleCopNewRule
{
    using StyleCop;
    using StyleCop.CSharp;

    [SourceAnalyzer(typeof(CsParser))]
    public class MyOwnCustomAnalyzer : SourceAnalyzer
    {
        public override void AnalyzeDocument(CodeDocument currentCodeDocument)
        {
            var codeDocument = (CsDocument)currentCodeDocument;
            if (codeDocument.RootElement != null && !codeDocument.RootElement.Generated)
            {
                codeDocument.WalkDocument(new CodeWalkerElementVisitor<object>(this.InspectCurrentElement), null, null);
            }
        }

        private bool InspectCurrentElement(CsElement element, CsElement parentElement, object context)
        {
            if (!(element is Class csElement) || !csElement.BaseClass.Contains("Controller")) return true;

            var isHasMvcUsing = isHasMVSNamespace(parentElement?.ChildElements) ||
                                isHasMVSNamespace(parentElement.FindParentElement()?.ChildElements);

            var isEndsWithController = element.Name.EndsWith("Controller");

            if (isHasMvcUsing && !isEndsWithController)
                AddViolation(parentElement, "MyOwnCustomRule", "class should ends with 'Controller'");

            return true;
        }

        private bool isHasMVSNamespace(IEnumerable<CsElement> elements)
        {
            return elements.Any(namespaces =>
                namespaces.ElementType == ElementType.UsingDirective &&
                namespaces.Declaration.Name == "System.Web.Mvc");
        }
    }
}