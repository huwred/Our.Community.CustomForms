using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Umbraco.Extensions;

namespace Our.Community.CustomForm.Extensions
{
    /// <summary>
    /// Helper extensions add Javascript and CSS blocks in partial views
    /// Blocks will be rendered in the master template using RenderPartialViewXXXXBlocks
    /// </summary>
    public static class ScriptBlockExtensions
    {
        #region PartialView Scriptblocks
        private const string SCRIPTBLOCK_BUILDER = "ScriptBlockBuilder";

        /// <summary>
        /// Defines a javascript block for renderring in Partial views into the main template body
        /// 
        /// DO NOT USE THIS FOR CSS FILES
        /// </summary>
        /// <param name="helper">HtmlHelper</param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static ScriptBlock PartialViewScriptBlock(this IHtmlHelper helper,Func<dynamic, HelperResult> template)
        {
            var scriptBuilder = helper.ViewContext.HttpContext.Items[SCRIPTBLOCK_BUILDER] as StringBuilder ?? new StringBuilder();
            scriptBuilder.Append(template(null).ToHtmlString());
            helper.ViewContext.HttpContext.Items[SCRIPTBLOCK_BUILDER] = scriptBuilder;
            return new ScriptBlock(helper.ViewContext);
        }

        /// <summary>
        /// Extension to Render Scriptblocks from child views
        /// </summary>
        /// <param name="helper"></param>
        /// <returns>MvcHtmlString</returns>
        public static IHtmlContent RenderScriptBlocks(this IHtmlHelper helper)
        {
            if (helper.ViewContext.HttpContext.Items.TryGetValue(SCRIPTBLOCK_BUILDER, out var scriptsData) && scriptsData is StringBuilder)
            {
                return new HtmlString(scriptsData.ToString());
            }

            return HtmlString.Empty;
        }

        #endregion

        public class ScriptBlock : IDisposable
        {
            private readonly ViewContext _viewContext;
            private readonly TextWriter _originalWriter;
            private readonly StringWriter _scriptWriter;
            private bool _disposed;

            public ScriptBlock(ViewContext viewContext)
            {
                _viewContext = viewContext;
                _originalWriter = viewContext.Writer;

                // replace writer
                viewContext.Writer = _scriptWriter = new StringWriter();
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                try
                {
                    List<object> scripts = null;
                    if (_viewContext.HttpContext.Items.TryGetValue(SCRIPTBLOCK_BUILDER, out var scriptsData))
                        scripts = scriptsData as List<object>;
                    if (scripts == null)
                        _viewContext.HttpContext.Items[SCRIPTBLOCK_BUILDER] = scripts = new List<object>();

                    scripts.Add(new HtmlString(_scriptWriter.ToString()));
                }
                finally
                {
                    // restore the original writer
                    _viewContext.Writer = _originalWriter;
                    _disposed = true;
                }
            }
        }
    }


}