using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace MetadataLib
{
    public class MetadataLib
    {
        public static List<String> GetDependencies(string MetadataFilename)
        {
            var DependencyList = new List<String>();
            if (File.Exists(MetadataFilename))
            {
                XDocument xd = XDocument.Parse(File.ReadAllText(MetadataFilename));
                var data = from item in
                               xd.Descendants("metadata")
                               .Descendants("dependencies")
                               .Descendants("file")
                           select item;

                foreach (XElement e in data)
                {                    
                    DependencyList.Add(e.Elements("filename").First().Value);                    
                }
            }
            return DependencyList;
        }


        public static List<String> GetCategories(string MetadataFilename)
        {
            var CategoryList = new List<String>();
            if (File.Exists(MetadataFilename))
            {

                XDocument xd = XDocument.Parse(File.ReadAllText(MetadataFilename));
                var data = from item in
                               xd.Descendants("metadata")
                               .Elements("categories")
                           select item;

                CategoryList = data.First().Value.Split(new char[] { ';', ',' }).ToList();
            }
            return CategoryList;
        }

        public static List<String> GetKeywords(string MetadataFilename)
        {
            var KeywordList = new List<String>();
            if (File.Exists(MetadataFilename))
            {

                XDocument xd = XDocument.Parse(File.ReadAllText(MetadataFilename));
                var data = from item in
                               xd.Descendants("metadata")
                               .Elements("keyword")
                           select item;

                KeywordList = data.First().Value.Split(new char[] { ';', ',' }).ToList();
            }
            return KeywordList;
        }


    }
}
