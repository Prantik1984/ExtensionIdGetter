using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ExtensionIdGetter
{
    class Program
    {
        #region vars
        static string _rootDir = ""; //directory to be parsed
        static bool _vs2010 = false; // to be created for VS2010
        static bool _vs2012 = false; // to be created for VS2012
        static bool _vs2013 = false; // to be created for VS2013
        static bool _vs2015 = false; // to be created for VS2015
        static string _outFile = "";  // Output file
        #endregion

        static void Main(string[] args)
        {
            ReadArgs(args);
            if (Directory.Exists(_rootDir))
            {
                var rDoc = new XDocument();
                var rElem = new XElement("vsixes");
                foreach (var extnFl in Directory.GetFiles(_rootDir, "extension.vsixmanifest", SearchOption.AllDirectories))
                {
                    var xDoc = XDocument.Parse(File.ReadAllText(extnFl, Encoding.UTF8));
                    var nmSpace = xDoc.Root.GetDefaultNamespace();
                    if (!xDoc.Root.Descendants(nmSpace + "Identity").Any()) continue;
                    var idElem = xDoc.Root.Descendants(nmSpace + "Identity").FirstOrDefault();
                    if (_vs2010)
                    {
                        var tIdElem2010 = new XElement("vsix", new XAttribute("vsversion", "10.0"), new XCData(idElem.Attribute("Id").Value));
                        rElem.Add(tIdElem2010);
                    }
                    if (_vs2012)
                    {
                        var tIdElem2012 = new XElement("vsix", new XAttribute("vsversion", "11.0"), new XCData(idElem.Attribute("Id").Value));
                        rElem.Add(tIdElem2012);
                    }

                    if (_vs2013)
                    {
                        var tIdElem2013 = new XElement("vsix", new XAttribute("vsversion", "12.0"), new XCData(idElem.Attribute("Id").Value));
                        rElem.Add(tIdElem2013);
                    }
                    if (_vs2015)
                    {
                        var tIdElem2014 = new XElement("vsix", new XAttribute("vsversion", "14.0"), new XCData(idElem.Attribute("Id").Value));
                        rElem.Add(tIdElem2014);
                    }



                }
                rDoc.Add(rElem);
                if (!string.IsNullOrEmpty(_outFile)) rDoc.Save(_outFile);
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        /// <summary>
        /// reading arguments
        /// </summary>
        /// <param name="args"></param>
        static void ReadArgs(string[] args)
        {
            var vsvers = "";
            foreach (var arg in args)
            {
                if (arg.StartsWith("-r")) _rootDir = arg.Substring(2);
                if (arg.StartsWith("-v")) vsvers = arg.Substring(2).Trim();
                if (arg.StartsWith("-o")) _outFile = arg.Substring(2).Trim();
            }

            if (!string.IsNullOrEmpty(vsvers))
            {
                var vsversions = vsvers.Split(new char[] { ';' }, StringSplitOptions.None);
                if (vsversions.Contains("10")) _vs2010 = true;
                if (vsversions.Contains("12")) _vs2012 = true;
                if (vsversions.Contains("13")) _vs2013 = true;
                if (vsversions.Contains("15")) _vs2015 = true;
            }
        }
    }
}
