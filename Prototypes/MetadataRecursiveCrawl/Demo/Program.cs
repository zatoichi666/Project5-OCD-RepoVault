using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using MetadataLib;
using TextAnalyzer;

namespace Demo
{
    class Program
    {
        private List<XmlSearchResult_c> _xsr = new List<XmlSearchResult_c>();

        public List<XmlSearchResult_c> GetSearchResults()
        {
            return _xsr;

        }
        public Program(MetadataRecursiveCrawl metRecurse)
        {
            //metRecurse.metadataEvent
            //    += new MetadataRecursiveCrawl.metadataEventHandler(DisplayContents);

            metRecurse.metadataEvent
                += new MetadataRecursiveCrawl.metadataEventHandler(TextSearch);            



        }

        public void DisplayContents(object obj, EventArgs seva)
        {            
            String filename = ((someEventArgs)seva).msg;

            Console.WriteLine("{" + filename + "}");
            Console.Write(File.ReadAllText(filename));
            Console.WriteLine();            
        }

        public void TextSearch(object obj, EventArgs seva)
        {
            String filename = ((someEventArgs)seva).msg;
            List<String> ft = new List<String>();
            ft.Add(filename);

            _xsr.AddRange(TextQuery.run(ft, new String[] { "namespace" }, false, new String[]{} ));
            
        }

        static void Main(string[] args)
        {
            var listCrawl = new MetadataRecursiveCrawl();
            var sub = new Program(listCrawl);
            listCrawl.StartCrawl("AbstractCommunicator.cs");            
            foreach (XmlSearchResult_c xsr in sub._xsr)
            {
                Console.WriteLine(xsr.filename);
                Console.WriteLine(xsr.ToString());
            }
            Console.WriteLine("Done");
        }
    }
}
