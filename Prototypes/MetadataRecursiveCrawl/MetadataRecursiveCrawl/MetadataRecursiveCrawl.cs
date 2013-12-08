///////////////////////////////////////////////////////////////////////////////
// MetadataRecursiveCrawl.cs - Recursive metadata search with public         //
//  delegate                                                                 //
//                                                                           //
// Matthew Synborski, CSE681 - Software Modeling and Analysis, Fall 2013     //
///////////////////////////////////////////////////////////////////////////////
/*
 *  Package Operations:
 *  -------------------
 *  This package contains three classes:
 *  someEventArgs
 *    Serves as a container class for the content files located by 
 *    MetadataRecursiveCrawl.
 *  MetadataRecursiveCrawl
 *    Is a class for the MetadataRecursiveCrawl.  This recursively searches metadata 
 *    and fires an event on a file listed in the metadata dependency tags.
 *  
 *  Required Files:
 *  - MetadataRecursiveCrawl.cs  Defines MetadataCrawl behavior
 *
 *  Required References:
 *  - System.Linq
 *  - System.Threading.Tasks;
 *  - System.IO;
 *  - MetadataLib;
 *
 *  Maintenance History:
 *  --------------------
 *  ver 1.0 : Nov 22, 2013 
 *  - first release
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataLib
{
    public class metadataEventArgs : EventArgs
    {
        private string _msg;

        public metadataEventArgs(string msg)
        {
            _msg = msg;
        }

        public string msg
        {
            get
            {
                return _msg;
            }
            set
            {
                _msg = value;
            }
        }

    }

    public class MetadataRecursiveCrawl
    {
        private List<String> dependencies = new List<String>();
        
        public delegate void metadataEventHandler(object sender, EventArgs seva);
        public event metadataEventHandler metadataEvent;

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (String s in dependencies)
            {
                sb.Append(s + ",");
            }
            return sb.ToString();
        }

        public void StartCrawl(String rootFileName)
        {
            if (File.Exists(rootFileName + ".metadata"))
            {
                dependencies.Add(rootFileName);
                metadataEventArgs seva
                    = new metadataEventArgs(rootFileName);
                if (metadataEvent != null)
                    metadataEvent(this, seva);
                Crawl(rootFileName);
            }
        }

        private void Crawl(String rootFileName)
        {
            if (File.Exists(rootFileName + ".metadata"))
            {
                var deps = MetadataLib.GetDependencies(rootFileName + ".metadata");
                dependencies.AddRange(deps);
                foreach (String s in deps)
                {
                    metadataEventArgs seva
                        = new metadataEventArgs(s);
                    if (metadataEvent != null)
                        metadataEvent(this, seva);
                    Crawl(s);
                }
            }
        }

        static int Main()
        {
            var listCrawl = new MetadataRecursiveCrawl();
            listCrawl.StartCrawl("AbstractCommunicator.cs");
            Console.WriteLine(listCrawl.ToString());
            return 0;
        }
    }
}
