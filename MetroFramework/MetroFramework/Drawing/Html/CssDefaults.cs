using System;
using System.Drawing;

namespace MetroFramework.Drawing.Html
{
	public class CssDefaults
	{
		public const string DefaultStyleSheet = "\r\n\r\n        \r\n        html, address,\r\n        blockquote,\r\n        body, dd, div,\r\n        dl, dt, fieldset, form,\r\n        frame, frameset,\r\n        h1, h2, h3, h4,\r\n        h5, h6, noframes,\r\n        ol, p, ul, center,\r\n        dir, hr, menu, pre   { display: block }\r\n        li              { display: list-item }\r\n        head            { display: none }\r\n        table           { display: table }\r\n        tr              { display: table-row }\r\n        thead           { display: table-header-group }\r\n        tbody           { display: table-row-group }\r\n        tfoot           { display: table-footer-group }\r\n        col             { display: table-column }\r\n        colgroup        { display: table-column-group }\r\n        td, th          { display: table-cell }\r\n        caption         { display: table-caption }\r\n        th              { font-weight: bolder; text-align: center }\r\n        caption         { text-align: center }\r\n        body            { margin: 8px }\r\n        h1              { font-size: 2em; margin: .67em 0 }\r\n        h2              { font-size: 1.5em; margin: .75em 0 }\r\n        h3              { font-size: 1.17em; margin: .83em 0 }\r\n        h4, p,\r\n        blockquote, ul,\r\n        fieldset, form,\r\n        ol, dl, dir,\r\n        menu            { margin: 1.12em 0 }\r\n        h5              { font-size: .83em; margin: 1.5em 0 }\r\n        h6              { font-size: .75em; margin: 1.67em 0 }\r\n        h1, h2, h3, h4,\r\n        h5, h6, b,\r\n        strong          { font-weight: bolder; }\r\n        blockquote      { margin-left: 40px; margin-right: 40px }\r\n        i, cite, em,\r\n        var, address    { font-style: italic }\r\n        pre, tt, code,\r\n        kbd, samp       { font-family: monospace }\r\n        pre             { white-space: pre }\r\n        button, textarea,\r\n        input, select   { display: inline-block }\r\n        big             { font-size: 1.17em }\r\n        small, sub, sup { font-size: .83em }\r\n        sub             { vertical-align: sub }\r\n        sup             { vertical-align: super }\r\n        table           { border-spacing: 2px; }\r\n        thead, tbody,\r\n        tfoot           { vertical-align: middle }\r\n        td, th          { vertical-align: inherit }\r\n        s, strike, del  { text-decoration: line-through }\r\n        hr              { border: 1px inset }\r\n        ol, ul, dir,\r\n        menu, dd        { margin-left: 40px }\r\n        ol              { list-style-type: decimal }\r\n        ol ul, ul ol,\r\n        ul ul, ol ol    { margin-top: 0; margin-bottom: 0 }\r\n        u, ins          { text-decoration: underline }\r\n        br:before       { content: \"\\A\" }\r\n        :before, :after { white-space: pre-line }\r\n        center          { text-align: center }\r\n        :link, :visited { text-decoration: underline }\r\n        :focus          { outline: thin dotted invert }\r\n\r\n        /* Begin bidirectionality settings (do not change) */\r\n        BDO[DIR=\"ltr\"]  { direction: ltr; unicode-bidi: bidi-override }\r\n        BDO[DIR=\"rtl\"]  { direction: rtl; unicode-bidi: bidi-override }\r\n\r\n        *[DIR=\"ltr\"]    { direction: ltr; unicode-bidi: embed }\r\n        *[DIR=\"rtl\"]    { direction: rtl; unicode-bidi: embed }\r\n\r\n        @media print {\r\n          h1            { page-break-before: always }\r\n          h1, h2, h3,\r\n          h4, h5, h6    { page-break-after: avoid }\r\n          ul, ol, dl    { page-break-before: avoid }\r\n        }\r\n\r\n        /* Not in the specification but necessary */\r\n        a               { color:blue; text-decoration:underline }\r\n        table           { border-color:#dfdfdf; border-style:outset; }\r\n        td, th          { border-color:#dfdfdf; border-style:inset; }\r\n        style, title,\r\n        script, link,\r\n        meta, area,\r\n        base, param     { display:none }\r\n        hr              { border-color: #ccc }  \r\n        pre             { font-size:10pt }\r\n        \r\n        /*This is the background of the HtmlToolTip*/\r\n        .htmltooltipbackground {\r\n              border:solid 1px #767676;\r\n              corner-radius:3px;\r\n              background-color:#white;\r\n              background-gradient:#E4E5F0;\r\n        }\r\n\r\n        ";

		public const string ErrorOnImageIcon = "\r\n        <style>\r\n          table { \r\n\r\n               border-bottom:1px solid #bbb;\r\n               border-right:1px solid #bbb;\r\n               border-spacing:0;\r\n          }\r\n          td { \r\n               border:1px solid #555;\r\n               font:bold 9pt Arial;\r\n               padding:3px;\r\n               color:red;\r\n               background-color:#fbfbfb;\r\n           }\r\n        </style>\r\n        <table>\r\n        <tr>\r\n        <td>X</td>\r\n        </tr>\r\n        </table>";

		public const string ErrorOnObjectIcon = "\r\n        <style>\r\n          table { \r\n\r\n               border-bottom:1px solid #bbb;\r\n               border-right:1px solid #bbb;\r\n               border-spacing:0;\r\n          }\r\n          td { \r\n               border:1px solid #555;\r\n               font:bold 7pt Arial;\r\n               padding:3px;\r\n               color:red;\r\n               background-color:#fbfbfb;\r\n           }\r\n        </style>\r\n        <table>\r\n        <tr>\r\n        <td>X</td>\r\n        </tr>\r\n        </table>";

		public static float FontSize;

		public static string FontSerif;

		public static string FontSansSerif;

		public static string FontCursive;

		public static string FontFantasy;

		public static string FontMonospace;

		static CssDefaults()
		{
			CssDefaults.FontSize = 12f;
			CssDefaults.FontSerif = FontFamily.GenericSerif.Name;
			CssDefaults.FontSansSerif = FontFamily.GenericSansSerif.Name;
			CssDefaults.FontCursive = "Monotype Corsiva";
			CssDefaults.FontFantasy = "Comic Sans MS";
			CssDefaults.FontMonospace = FontFamily.GenericMonospace.Name;
		}

		public CssDefaults()
		{
		}
	}
}