using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;

namespace FormatHTMLForGDNP
{
    class Program
    {
        static void Main(string[] args)
        {
            // [!NOTE]
            Regex noteRegex = new Regex("\\[!NOTE\\]");
            string file = File.ReadAllText(args[0]);
            file = file.Insert(0, @"<xml>" + "\n");
            file = string.Concat(file, @"</xml>");
            file = noteRegex.Replace(file, @"<strong>Note</strong>");

            // Load doc and nodes
            XmlDocument doc = new XmlDocument();
            
            try
            {
                doc.LoadXml(file);
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Failed to open the file. Do you have a single root element?");
            }

            XmlNodeList imgList = doc.GetElementsByTagName("img");
            XmlNodeList aList = doc.GetElementsByTagName("a");

            // Go through img nodes
            foreach (XmlNode node in imgList)
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if (attribute.Name == "src")
                    {
                        string imageName = attribute.Value;
                        char delimiter = '/';
                        string[] words = imageName.Split(delimiter);
                        string newName = words.Last();
                        attribute.Value = "/en-us/windows/documentation/PublishingImages/" + newName;
                    }
                }
            }

            // Go through a nodes
            foreach (XmlNode node in aList)
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    // ./create-and-manage-optional-packages.htm
                    // en-us/windows/documentation/Pages/something.aspx
                    if (attribute.Name == "href")
                    {
                        if (attribute.Value.StartsWith("./"))
                        {
                            string linkName = attribute.Value;
                            char[] delimiters = { '/', '.' };
                            string[] words = linkName.Split(delimiters);
                            string newName = words[words.Length - 2];
                            attribute.Value = "/en-us/windows/documentation/Pages/" + newName + ".aspx";
                        }
                    }
                }
            }

            doc.Save("out.html");
        }
    }
}
