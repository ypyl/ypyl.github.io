---
layout: post
title: "Style Cop I don't like SA 1201"
date: 2011-12-08

tags: dotnet
categories: programming
---
[StyleCop]() is a good application to take your source code in good state. There are a lot of rules and most of all are very useful, but...

Unfortunately, the rule SA1201 is not so good for me. So i just want to create the similar rule, but a little else.

You can see the rule that changes the order of document's element to the next:

```xml
<element name="File" order="0"></element>
<element name="Root" order="1"></element>
<element name="ExternAliasDirective" order="2"></element>
<element name="UsingDirective" order="3"></element>
<element name="AssemblyAttribute" order="4"></element>
<element name="Namespace" order="5"></element>
<element name="Field" order="6"></element>
<element name="Constructor" order="10"></element>
<element name="Destructor" order="11"></element>
<element name="Delegate" order="8"></element>
<element name="Event" order="9"></element>
<element name="Enum" order="13"></element>
<element name="Interface" order="14"></element>
<element name="Property" order="7"></element>
<element name="Accessor" order="15"></element>
<element name="Indexer" order="16"></element>
<element name="Method" order="12"></element>
<element name="Struct" order="17"></element>
<element name="EnumItem" order="18"></element>
<element name="ConstructorInitializer" order="19"></element>
<element name="EmptyElement" order="20"></element>
```

It saves in configuration file, so it is possible to change it in any time. 
Further the source code of my rule. You can see the original source code of SA1201 using the [DotPeek](http://www.jetbrains.com/decompiler/) for example:

```cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Configuration;
using System.Linq;
 
using StyleCop;
using StyleCop.CSharp;
using System.Xml.Serialization;
using System.IO;
 
 
namespace Wsg.Rules
{
    /// <summary>
    /// Wsg StyleCOp custom rules
    /// </summary>
    [SourceAnalyzer(typeof(CsParser))]
    public class WsgStyleCopCustomRules : SourceAnalyzer
    {
        private Rule ws1000 = null;
 
        public Rule Ws1000
        {
            get
            {
                if (ws1000 == null)
                {
                    lock (ws1000)
                    {
                        if (ws1000 == null)
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(Rule));
 
                            TextReader tr = new StreamReader(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "WS1000.config"));
                            ws1000 = (Rule)serializer.Deserialize(tr);
                            tr.Close();
                        }
                    }
                }
 
                return ws1000;
            }
        }
 
        public override void AnalyzeDocument(CodeDocument document)
        {
            Param.RequireNotNull((object)document, "document");
            CsDocument csDocument = (CsDocument)document;
            if (csDocument.RootElement == null)
                return;
            bool checkGeneratedCode = true;
            if (document.Settings != null)
            {
                BooleanProperty booleanProperty = this.GetSetting(document.Settings, "GeneratedCodeElementOrder") as BooleanProperty;
                if (booleanProperty != null)
                    checkGeneratedCode = booleanProperty.Value;
                this.ProcessElements((CsElement)csDocument.RootElement, checkGeneratedCode);
            }
            this.CheckUsingDirectiveOrder((CsElement)csDocument.RootElement);
        }
 
        public override bool DoAnalysis(CodeDocument document)
        {
            Param.RequireNotNull((object)document, "document");
            CsDocument csDocument = (CsDocument)document;
            if (csDocument.FileHeader != null)
                return !csDocument.FileHeader.UnStyled;
            else
                return true;
        }
 
        private bool ProcessElements(CsElement element, bool checkGeneratedCode)
        {
            if (this.Cancel)
                return false;
            this.CheckElementOrder(element, checkGeneratedCode);
            return true;
        }
 
        private void CheckElementOrder(CsElement element, bool checkGeneratedCode)
        {
            if (!element.Generated && (element.ElementType == ElementType.Class || element.ElementType == ElementType.Field || (element.ElementType == ElementType.Enum || element.ElementType == ElementType.Struct) || (element.ElementType == ElementType.Interface || element.ElementType == ElementType.Delegate || (element.ElementType == ElementType.Event || element.ElementType == ElementType.Property)) || (element.ElementType == ElementType.Indexer || element.ElementType == ElementType.Method || (element.ElementType == ElementType.Constructor || element.ElementType == ElementType.Accessor))))
                this.CheckDeclarationKeywordOrder(element);
            this.CheckUsingDirectivePlacement(element);
            this.CheckChildElementOrdering(element, checkGeneratedCode);
        }
 
        private void CheckChildElementOrdering(CsElement element, bool checkGeneratedCode)
        {
            if (element.ChildElements.Count <= 0)
                return;
            bool flag = true;
            CsElement[] array = new CsElement[element.ChildElements.Count];
            element.ChildElements.CopyTo(array, 0);
            for (int index1 = 0; index1 < array.Length; ++index1)
            {
                CsElement csElement = array[index1];
               
                for (int index2 = index1 + 1; index2 < array.Length; ++index2)
                {
                    CsElement second = array[index2];
                    if ((checkGeneratedCode && (!csElement.Generated || !second.Generated) || !checkGeneratedCode && !csElement.Generated && !second.Generated))
                    {
                        if (!this.CompareItems(csElement, second, !flag))
                        {
                            if (flag)
                                csElement.AnalyzerTag = (object)false;
                            else
                                second.AnalyzerTag = (object)false;
                        }
                        else if (flag)
                            flag = false;
                        if (csElement.ElementType == ElementType.Accessor && second.ElementType == ElementType.Accessor)
                        {
                            Accessor accessor1 = (Accessor)csElement;
                            Accessor accessor2 = (Accessor)second;
                            if (accessor1.AccessorType == AccessorType.Set && accessor2.AccessorType == AccessorType.Get)
                                this.AddViolation((ICodeElement)csElement, "PropertyAccessorsMustFollowOrder", new object[0]);
                            else if (accessor1.AccessorType == AccessorType.Remove && accessor2.AccessorType == AccessorType.Add)
                                this.AddViolation((ICodeElement)csElement, "EventAccessorsMustFollowOrder", new object[0]);
                        }
                    }
                }
 
                this.CheckElementOrder(csElement, checkGeneratedCode);
            }
        }
 
        public bool CompareElementType(ElementType first, ElementType second)
        {
            if (Ws1000 != null)
            {
                return Ws1000.Order.First(x => x.name == System.Enum.GetName(typeof(ElementType), first)).order > Ws1000.Order.First(x => x.name == System.Enum.GetName(typeof(ElementType), second)).order;
            }
            return false;
        }
 
        private bool CompareItems(CsElement first, CsElement second, bool foundFirst)
        {
            if (first.ElementType != ElementType.EmptyElement && second.ElementType != ElementType.EmptyElement && (first.ElementType != ElementType.Accessor || second.ElementType != ElementType.Accessor))
            {
                CsElement csElement1 = second;
                if (!foundFirst)
                    csElement1 = first;
                if (CompareElementType(first.ElementType, second.ElementType))
                {
                    this.AddViolation((ICodeElement)first, csElement1.LineNumber, "ElementsMustAppearInTheCorrectOrder", (object)first.FriendlyPluralTypeText, (object)second.FriendlyPluralTypeText);
                    return false;
                }
                else if (first.ElementType == second.ElementType && first.Declaration != null && second.Declaration != null)
                {
                    if (first.Declaration.AccessModifierType < second.Declaration.AccessModifierType)
                    {
                        if (first.ElementType == ElementType.Constructor && second.ElementType == ElementType.Constructor)
                        {
                            if (first.Declaration.ContainsModifier(new CsTokenType[1] { CsTokenType.Static}))
                            {
                                if (!second.Declaration.ContainsModifier(new CsTokenType[1] { CsTokenType.Static}))
                                    goto label_31;
                            }
                        }
                        if (!first.Declaration.AccessModifier && first.ElementType != ElementType.Method)
                        {
                            if (first.Declaration.ContainsModifier(new CsTokenType[1] {CsTokenType.Partial}))
                                goto label_14;
                        }
                        if (!second.Declaration.AccessModifier && second.ElementType != ElementType.Method)
                        {
                            if (second.Declaration.ContainsModifier(new CsTokenType[1] { CsTokenType.Partial }))
                                goto label_14;
                        }
                        this.AddViolation((ICodeElement)first, csElement1.LineNumber, "ElementsMustBeOrderedByAccess", (object)WsgStyleCopCustomRules.AccessModifierTypeString(first.Declaration.AccessModifierType), (object)first.FriendlyPluralTypeText, (object)WsgStyleCopCustomRules.AccessModifierTypeString(second.Declaration.AccessModifierType), (object)second.FriendlyPluralTypeText);
                        goto label_19;
                    label_14:
                        CsElement csElement2 = first;
                        if (!first.Declaration.AccessModifier)
                        {
                            if (first.Declaration.ContainsModifier(new CsTokenType[1]{  CsTokenType.Partial}))
                                goto label_17;
                        }
                        csElement2 = second;
                    label_17:
                        this.AddViolation((ICodeElement)csElement2, "PartialElementsMustDeclareAccess", new object[2]
                                                    {
                                                      (object) csElement2.FriendlyTypeText,
                                                      (object) csElement2.FriendlyPluralTypeText
                                                    });
                    label_19:
                        return false;
                    }
                    else if (first.Declaration.AccessModifierType == second.Declaration.AccessModifierType)
                    {
                        bool flag1 = false;
                        bool flag2 = false;
                        Field field1 = first as Field;
                        Field field2 = second as Field;
                        if (field1 != null && field2 != null)
                        {
                            flag1 = field1.Const;
                            flag2 = field1.Readonly;
                            if ((field2.Const || field2.Readonly) && (!field1.Const && !field1.Readonly))
                            {
                                this.AddViolation((ICodeElement)first, csElement1.LineNumber, "ConstantsMustAppearBeforeFields", (object)WsgStyleCopCustomRules.AccessModifierTypeString(first.Declaration.AccessModifierType), (object)first.FriendlyPluralTypeText, (object)WsgStyleCopCustomRules.AccessModifierTypeString(second.Declaration.AccessModifierType), (object)second.FriendlyPluralTypeText);
                                return false;
                            }
                        }
                        if (second.Declaration.ContainsModifier(new CsTokenType[1] {  CsTokenType.Static }))
                        {
                            if (!first.Declaration.ContainsModifier(new CsTokenType[1] { CsTokenType.Static}) && !flag1 && !flag2)
                            {
                                this.AddViolation((ICodeElement)first, csElement1.LineNumber, "StaticElementsMustAppearBeforeInstanceElements", (object)WsgStyleCopCustomRules.AccessModifierTypeString(first.Declaration.AccessModifierType), (object)first.FriendlyPluralTypeText, (object)WsgStyleCopCustomRules.AccessModifierTypeString(second.Declaration.AccessModifierType), (object)second.FriendlyPluralTypeText);
                                return false;
                            }
                        }
                        if (first.Declaration.ElementType != ElementType.UsingDirective && second.Declaration.ElementType != ElementType.UsingDirective)
                        {
                            if (string.Compare(first.Declaration.Name, second.Declaration.Name) > 0)
                            {
                                this.AddViolation((ICodeElement)first, csElement1.LineNumber, "UsingNamesMustBeOrderedAlphabetically", (object)WsgStyleCopCustomRules.AccessModifierTypeString(first.Declaration.AccessModifierType), (object)first.FriendlyPluralTypeText, (object)WsgStyleCopCustomRules.AccessModifierTypeString(second.Declaration.AccessModifierType), (object)second.FriendlyPluralTypeText);
                                return false;
                            }
                        }
                    }
                    else if (first.ElementType == ElementType.Constructor && second.ElementType == ElementType.Constructor)
                    {
                        if (second.Declaration.ContainsModifier(new CsTokenType[1] { CsTokenType.Static }))
                        {
                            if (!first.Declaration.ContainsModifier(new CsTokenType[1]{ CsTokenType.Static}))
                            {
                                this.AddViolation((ICodeElement)first, csElement1.LineNumber, "StaticElementsMustAppearBeforeInstanceElements", (object)WsgStyleCopCustomRules.AccessModifierTypeString(first.Declaration.AccessModifierType), (object)first.FriendlyPluralTypeText, (object)WsgStyleCopCustomRules.AccessModifierTypeString(second.Declaration.AccessModifierType), (object)second.FriendlyPluralTypeText);
                                return false;
                            }
                        }
                    }
                }
            }
        label_31:
            return true;
        }
 
        private static string AccessModifierTypeString(AccessModifierType type)
        {
            switch (type)
            {
                case AccessModifierType.Public:
                    return "public";
                case AccessModifierType.Internal:
                    return "internal";
                case AccessModifierType.ProtectedInternal:
                    return "protected internal";
                case AccessModifierType.Protected:
                    return "protected";
                case AccessModifierType.Private:
                    return "private";
                default:
                    throw new InvalidOperationException();
            }
        }
 
        private void CheckUsingDirectivePlacement(CsElement element)
        {
            if (element.Generated || element.ElementType != ElementType.UsingDirective)
                return;
            CsElement parentElement = ICodePartExtensions.FindParentElement((ICodePart)element);
            if (parentElement == null)
                return;
            if (parentElement.ElementType != ElementType.Root)
            {
                this.AddViolation((ICodeElement)element, "UsingDirectivesMustBePlacedWithoutNamespace", new object[0]);
            }
        }
 
        private void CheckDeclarationKeywordOrder(CsElement element)
        {
            int num1 = -1;
            int num2 = -1;
            int num3 = -1;
            int num4 = 0;
            foreach (CsToken csToken in (ItemList<CsToken>)element.Declaration.Tokens)
            {
                switch (csToken.CsTokenType)
                {
                    case CsTokenType.Private:
                    case CsTokenType.Public:
                    case CsTokenType.Protected:
                    case CsTokenType.Internal:
                        if (num1 == -1)
                        {
                            num1 = num4++;
                            continue;
                        }
                        else
                            continue;
                    case CsTokenType.Static:
                        if (num2 == -1)
                        {
                            num2 = num4++;
                            continue;
                        }
                        else
                            continue;
                    case CsTokenType.WhiteSpace:
                    case CsTokenType.EndOfLine:
                    case CsTokenType.SingleLineComment:
                    case CsTokenType.MultiLineComment:
                        continue;
                    default:
                        if (num3 == -1)
                        {
                            num3 = num4++;
                            continue;
                        }
                        else
                            continue;
                }
            }
            if (num1 != -1)
            {
                if (num2 > -1 && num2 < num1)
                {
                    WsgStyleCopCustomRules orderingRules = this;
                    CsElement csElement = element;
                    // ISSUE: variable of a boxed type
                    var local = "DeclarationKeywordsMustFollowOrder";
                    object[] objArray1 = new object[2] {(object) "AccessModifier", null};
                    objArray1[1] = (object)string.Format((IFormatProvider)CultureInfo.InvariantCulture, "'{0}'", new object[1] { (object) "Static" });
                    object[] objArray2 = objArray1;
                    orderingRules.AddViolation((ICodeElement)csElement, local, objArray2);
                }
                if (num3 > -1 && num3 < num1)
                {
                    WsgStyleCopCustomRules orderingRules = this;
                    CsElement csElement = element;
                    // ISSUE: variable of a boxed type
                    var local = "DeclarationKeywordsMustFollowOrder";
                    object[] objArray1 = new object[2] { (object)"AccessModifier", null };
                    objArray1[1] = (object)string.Format((IFormatProvider)CultureInfo.InvariantCulture, "'{0}'", new object[1] { (object) "Other" });
                    object[] objArray2 = objArray1;
                    orderingRules.AddViolation((ICodeElement)csElement, local, objArray2);
                }
            }
            if (num2 > -1 && num3 > -1 && num3 < num2)
                this.AddViolation((ICodeElement)element, "DeclarationKeywordsMustFollowOrder", new object[2] {
                      (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}'", new object[1]
                      {
                        (object) "Static"
                      }),
                      (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}'", new object[1]
                      {
                        (object) "Other"
                      })
                    });
            if (element.Declaration.AccessModifierType != AccessModifierType.ProtectedInternal)
                return;
            bool flag = false;
            foreach (CsToken csToken in (ItemList<CsToken>)element.Declaration.Tokens)
            {
                if (flag)
                {
                    if (csToken.CsTokenType == CsTokenType.Internal)
                        break;
                    if (csToken.CsTokenType != CsTokenType.WhiteSpace)
                    {
                        this.AddViolation((ICodeElement)element, "ProtectedMustComeBeforeInternal", new object[0]);
                        break;
                    }
                }
                else if (csToken.CsTokenType == CsTokenType.Protected)
                    flag = true;
                else if (csToken.CsTokenType == CsTokenType.Internal)
                {
                    this.AddViolation((ICodeElement)element, "ProtectedMustComeBeforeInternal", new object[0]);
                    break;
                }
            }
        }
 
        private void CheckUsingDirectiveOrder(CsElement rootElement)
        {
            if (rootElement.Generated)
                return;
            this.CheckOrderOfUsingDirectivesUnderElement(rootElement);
            foreach (CsElement rootElement1 in (IEnumerable<CsElement>)rootElement.ChildElements)
            {
                if (rootElement1.ElementType == ElementType.Namespace)
                    this.CheckUsingDirectiveOrder(rootElement1);
            }
        }
 
        private void CheckOrderOfUsingDirectivesUnderElement(CsElement element)
        {
            List<UsingDirective> usings = (List<UsingDirective>)null;
            foreach (CsElement csElement in (IEnumerable<CsElement>)element.ChildElements)
            {
                if (csElement.ElementType == ElementType.UsingDirective)
                {
                    if (usings == null)
                        usings = new List<UsingDirective>();
                    usings.Add((UsingDirective)csElement);
                }
                else if (csElement.ElementType != ElementType.ExternAliasDirective)
                    break;
            }
            if (usings == null)
                return;
            this.CheckOrderOfUsingDirectivesInList(usings);
        }
 
        private void CheckOrderOfUsingDirectivesInList(List<UsingDirective> usings)
        {
            for (int index1 = 0; index1 < usings.Count; ++index1)
            {
                UsingDirective firstUsing = usings[index1];
                for (int index2 = index1 + 1; index2 < usings.Count; ++index2)
                {
                    UsingDirective secondUsing = usings[index2];
                    if (!this.CompareOrderOfUsingDirectives(firstUsing, secondUsing))
                        break;
                }
            }
        }
 
        private bool CompareOrderOfUsingDirectives(UsingDirective firstUsing, UsingDirective secondUsing)
        {
            if (string.IsNullOrEmpty(firstUsing.Alias))
            {
                if (string.IsNullOrEmpty(secondUsing.Alias))
                {
                    bool flag1 = firstUsing.NamespaceType.StartsWith("System", StringComparison.Ordinal) || firstUsing.NamespaceType.StartsWith("global::System", StringComparison.Ordinal);
                    bool flag2 = secondUsing.NamespaceType.StartsWith("System", StringComparison.Ordinal) || secondUsing.NamespaceType.StartsWith("global::System", StringComparison.Ordinal);
                    if (flag2 && !flag1)
                    {
                        this.AddViolation((ICodeElement)secondUsing, "SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives", new object[0]);
                        return false;
                    }
                    else if ((flag1 && flag2 || !flag1 && !flag2) && !WsgStyleCopCustomRules.CheckNamespaceOrdering(firstUsing.NamespaceType, secondUsing.NamespaceType))
                    {
                        this.AddViolation((ICodeElement)firstUsing, "UsingDirectivesMustBeOrderedAlphabeticallyByNamespace", new object[0]);
                        return false;
                    }
                }
            }
            else if (string.IsNullOrEmpty(secondUsing.Alias))
            {
                this.AddViolation((ICodeElement)firstUsing, "UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives", new object[0]);
                return false;
            }
            else if (string.Compare(firstUsing.Alias, secondUsing.Alias, StringComparison.OrdinalIgnoreCase) > 0)
            {
                this.AddViolation((ICodeElement)firstUsing, "UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName", new object[0]);
                return false;
            }
            return true;
        }
 
        private static bool CheckNamespaceOrdering(string namespace1, string namespace2)
        {
            string[] strArray1 = namespace1.Split(new char[1] {'.'});
            string[] strArray2 = namespace2.Split(new char[1] {'.'});
            strArray1[0] = StringExtensions.SubstringAfter(strArray1[0], "global::", StringComparison.InvariantCulture);
            strArray2[0] = StringExtensions.SubstringAfter(strArray2[0], "global::", StringComparison.InvariantCulture);
            int num1 = Math.Min(strArray1.Length, strArray2.Length);
            for (int index = 0; index < num1; ++index)
            {
                int num2 = string.Compare(strArray1[index], strArray2[index], StringComparison.InvariantCultureIgnoreCase);
                if (num2 < 0)
                    return true;
                if (num2 > 0)
                    return false;
                int num3 = string.Compare(strArray1[index], strArray2[index], StringComparison.InvariantCulture);
                if (num3 < 0)
                    return true;
                if (num3 > 0)
                    return false;
            }
            if (strArray1.Length == strArray2.Length)
                return true;
            else
                return strArray1.Length < strArray2.Length;
        }
    }
}
```

Methods AnalyzeDocument и DoAnalysis are the most important, so i suggest to start the learning code from them. The project in VS2010 is StyleCopOrder.Thanks.