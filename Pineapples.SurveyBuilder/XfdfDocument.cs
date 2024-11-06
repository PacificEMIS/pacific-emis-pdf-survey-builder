using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace surveybuilder
{
	public class XfdfDocument
	{
		private XmlNamespaceManager nspm;

		public static XNamespace xfdfNamespace = "http://ns.adobe.com/xfdf/";

		public XfdfDocument(XDocument xfdf)
		{
			nspm = new XmlNamespaceManager(new NameTable());
			nspm.AddNamespace("x", "http://ns.adobe.com/xfdf/");

			Document = xfdf;
			FieldsNode = xfdf.Root.XPathSelectElement("x:fields", nspm);
		}

		public XDocument Document { get; private set; }
		public XElement FieldsNode { get; private set; }

		public XElement getElement(string fieldPath)
		{
			string xp = fieldToXPath(fieldPath);
			return FieldsNode.XPathSelectElement(xp, nspm);
		}
		private string fieldToXPath(string fieldPath)
		{
			// first split the input string to an array, 
			string[] nodes = fieldPath.Split('.');

			for (int i = 0; i < nodes.Length; i++)
			{
				string n = nodes[i];
				nodes[i] = string.Format("x:field[@name=\"{0}\"]", n);
			}
			return string.Join("/", nodes);
		}

	}
}