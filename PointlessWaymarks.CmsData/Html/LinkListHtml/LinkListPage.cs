﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace PointlessWaymarks.CmsData.Html.LinkListHtml
{
    using PointlessWaymarks.CmsData.Html.CommonHtml;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Code\PointlessWaymarksCms01\PointlessWaymarks.CmsData\Html\LinkListHtml\LinkListPage.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class LinkListPage : LinkListPageBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head data-generationversion=\"");
            
            #line 6 "C:\Code\PointlessWaymarksCms01\PointlessWaymarks.CmsData\Html\LinkListHtml\LinkListPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerationVersion?.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff") ?? string.Empty));
            
            #line default
            #line hidden
            this.Write("\">\r\n    <meta charset=\"utf-8\">\r\n    <title>");
            
            #line 8 "C:\Code\PointlessWaymarksCms01\PointlessWaymarks.CmsData\Html\LinkListHtml\LinkListPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ListTitle.HtmlEncode()));
            
            #line default
            #line hidden
            this.Write("</title>\r\n\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=" +
                    "1.0\">\r\n    \r\n    <link rel=\"alternate\" type=\"application/rss+xml\" \r\n      title=" +
                    "\"");
            
            #line 13 "C:\Code\PointlessWaymarksCms01\PointlessWaymarks.CmsData\Html\LinkListHtml\LinkListPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture($"RSS Feed for {UserSettingsSingleton.CurrentSettings().SiteName} - {ListTitle}".HtmlEncode()));
            
            #line default
            #line hidden
            this.Write("\"     \r\n      href=\"https:");
            
            #line 15 "C:\Code\PointlessWaymarksCms01\PointlessWaymarks.CmsData\Html\LinkListHtml\LinkListPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(RssUrl));
            
            #line default
            #line hidden
            this.Write("\" />\r\n\r\n    ");
            
            #line 17 "C:\Code\PointlessWaymarksCms01\PointlessWaymarks.CmsData\Html\LinkListHtml\LinkListPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Tags.CssStyleFileString()));
            
            #line default
            #line hidden
            this.Write("\r\n    ");
            
            #line 18 "C:\Code\PointlessWaymarksCms01\PointlessWaymarks.CmsData\Html\LinkListHtml\LinkListPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Tags.FavIconFileString()));
            
            #line default
            #line hidden
            this.Write("\r\n</head>\r\n\r\n<body>\r\n    ");
            
            #line 22 "C:\Code\PointlessWaymarksCms01\PointlessWaymarks.CmsData\Html\LinkListHtml\LinkListPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Tags.StandardHeader().ToString()));
            
            #line default
            #line hidden
            this.Write("\r\n    ");
            
            #line 23 "C:\Code\PointlessWaymarksCms01\PointlessWaymarks.CmsData\Html\LinkListHtml\LinkListPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(HorizontalRule.StandardRule()));
            
            #line default
            #line hidden
            this.Write("\r\n    <h1 class=\"index-title-content\">");
            
            #line 24 "C:\Code\PointlessWaymarksCms01\PointlessWaymarks.CmsData\Html\LinkListHtml\LinkListPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture($"{ListTitle}"));
            
            #line default
            #line hidden
            this.Write("</h1>\r\n    <div class=\"search-input-container\">\r\n        <input type=\"text\" class" +
                    "=\"search-input\" id=\"userSearchText\" onkeyup=\"searchContent()\" placeholder=\"Searc" +
                    "h ");
            
            #line 26 "C:\Code\PointlessWaymarksCms01\PointlessWaymarks.CmsData\Html\LinkListHtml\LinkListPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ListTitle));
            
            #line default
            #line hidden
            this.Write("...\">\r\n    </div>\r\n    ");
            
            #line 28 "C:\Code\PointlessWaymarksCms01\PointlessWaymarks.CmsData\Html\LinkListHtml\LinkListPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(LinkTableTag()));
            
            #line default
            #line hidden
            this.Write("\r\n    ");
            
            #line 29 "C:\Code\PointlessWaymarksCms01\PointlessWaymarks.CmsData\Html\LinkListHtml\LinkListPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Footer.StandardFooterDiv()));
            
            #line default
            #line hidden
            this.Write(@"

<script>
    function searchContent() {
      // Declare variables
      var input, filterText, contentDivs, loopDiv, i, divDataText;
      input = document.getElementById('userSearchText');
      filterText = input.value.toUpperCase();
      contentDivs = document.getElementsByClassName('content-list-item-container');

      // Loop through all list items, and hide those who don't match the search query
      for (i = 0; i < contentDivs.length; i++) {
        loopDiv = contentDivs[i];
	    divDataText = loopDiv.getAttribute('data-title').concat(
		    loopDiv.getAttribute('data-summary'),
		    loopDiv.getAttribute('data-tags')).toUpperCase();

        if (divDataText.indexOf(filterText) > -1) {
		    loopDiv.style.display = """";
        } else {
		    loopDiv.style.display = ""none"";
        }
      }
    }
</script>

</body>

</html>");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public class LinkListPageBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
