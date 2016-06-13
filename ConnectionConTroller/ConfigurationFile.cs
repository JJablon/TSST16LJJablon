using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Forms;

namespace ConnectionConTroller
{
    public static class ConfigurationFile
    {
        public static string GetProperty(string xmlFilePath, string element)
        {
            try
            {
                List<string> list = new List<string>();
                XDocument document = XDocument.Load(xmlFilePath);
                var root = document.Root;

                return (string)root.Element(element);
            }
            catch (Exception) { MessageBox.Show("parameter not found"); return null; }
        }
    }
}
