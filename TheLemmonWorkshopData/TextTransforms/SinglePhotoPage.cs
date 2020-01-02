﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by JetBrains T4 Processor
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace TheLemmonWorkshopData.TextTransforms
{
    using System;
    #line 2 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
using TheLemmonWorkshopData.Models;
    #line default
    #line hidden
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("JetBrains.ForTea.TextTemplating", "42.42.42.42")]
    public partial class SinglePhotoPage : SinglePhotoPageBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            
            this.Write("\r\n<!doctype html>\r\n\r\n<html lang=\"en\">\r\n<head>\r\n<meta charset=\"utf-8\">\r\n\r\n    <title>");
            
            #line 10 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbEntry.Title));
            
            #line default
            #line hidden
            this.Write("</title>i\r\n    <meta name=\"description\" content=\"");
            
            #line 11 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbEntry.Summary));
            
            #line default
            #line hidden
            this.Write("\">\r\n    <meta name=\"author\" content=\"");
            
            #line 12 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbEntry.CreatedBy));
            
            #line default
            #line hidden
            this.Write("\">\r\n    <meta name=\"keywords\" content=\"HTML, CSS, XML, JavaScript\">\r\n\r\n    <meta property=\"og:site_name\" content=\"");
            
            #line 15 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(SiteName));
            
            #line default
            #line hidden
            this.Write("\" />\r\n    <meta property=\"og:url\" content=\"");
            
            #line 16 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PageUrl));
            
            #line default
            #line hidden
            this.Write("\" />\r\n    <meta property=\"og:type\" content=\"");
            
            #line 17 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(SiteUrl));
            
            #line default
            #line hidden
            this.Write("\" />\r\n    <meta property=\"og:title\" content=\"");
            
            #line 18 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbEntry.Title));
            
            #line default
            #line hidden
            this.Write("\" />\r\n    <meta property=\"og:description\" content=\"");
            
            #line 19 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbEntry.Summary));
            
            #line default
            #line hidden
            this.Write("\" />\r\n    <meta property=\"og:image\" content=\"http:");
            
            #line 20 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DisplayImage.SiteUrl));
            
            #line default
            #line hidden
            this.Write("\" />\r\n    <meta property=\"og:image:secure_url\" content=\"https:");
            
            #line 21 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DisplayImage.SiteUrl));
            
            #line default
            #line hidden
            this.Write("\" />\r\n    <meta property=\"og:image:type\" content=\"image/jpeg\" />\r\n    <meta property=\"og:image:width\" content=\"");
            
            #line 23 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DisplayImage.Width));
            
            #line default
            #line hidden
            this.Write("\" />\r\n    <meta property=\"og:image:height\" content=\"");
            
            #line 24 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DisplayImage.Height));
            
            #line default
            #line hidden
            this.Write("\" />\r\n    <meta property=\"og:image:alt\" content=\"");
            
            #line 25 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbEntry.AltText));
            
            #line default
            #line hidden
            this.Write("\" />\r\n\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n\r\n</head>\r\n\r\n<body>\r\n    <img src=\"");
            
            #line 32 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DisplayImage.SiteUrl));
            
            #line default
            #line hidden
            this.Write("\" srcset=\"");
            #line 32 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"

    SrcSetString(); 
            
            #line default
            #line hidden
            this.Write("\" loading=\"lazy\" width=\"100%\"/>\r\n    <div>Aperature ");
            
            #line 34 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbEntry.Aperture));
            
            #line default
            #line hidden
            this.Write("</div>\r\n    <div>Shutter Speed ");
            
            #line 35 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbEntry.ShutterSpeed));
            
            #line default
            #line hidden
            this.Write("</div>\r\n    <div>Lens ");
            
            #line 36 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbEntry.Lens));
            
            #line default
            #line hidden
            this.Write("</div>\r\n    <div>Camera ");
            
            #line 37 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbEntry.CameraMake));
            
            #line default
            #line hidden
            this.Write("  ");
            
            #line 37 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbEntry.CameraModel));
            
            #line default
            #line hidden
            this.Write(" </div>\r\n    <div>Photographer ");
            
            #line 38 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbEntry.PhotoCreatedBy));
            
            #line default
            #line hidden
            this.Write("</div>\r\n    <div>Taken On ");
            
            #line 39 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbEntry.PhotoCreatedOn));
            
            #line default
            #line hidden
            this.Write("</div>\r\n    <div>License ");
            
            #line 40 "C:\Code\TheLemmonWorkshop02\TheLemmonWorkshopData\TextTransforms\SinglePhotoPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbEntry.License));
            
            #line default
            #line hidden
            this.Write("</div>\r\n</body>\r\n\r\n</html>");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("JetBrains.ForTea.TextTemplating", "42.42.42.42")]
    public class SinglePhotoPageBase
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
