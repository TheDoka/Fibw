using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Fibw
{
    [Serializable]
    class Reposistory
    {

        int version;
        int creator;

        public string hash;
        public long timestamp;
        public string name;

        public string filename;

        public Tree tree { get; set; }

        /*
         * Init the reposistory
         */
        public Reposistory(string repName = "") {

            // If the repository do not have name, we consider it's a new reposistory, so we init it.
            if (repName != "")
            {

                make(repName);

            }

        }

        private void make(string repName)
        {

            hash = Guid.NewGuid().ToString().Replace("-", String.Empty);
            timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            tree = new Tree(hash);
            name = repName;
            filename = Path.GetFileName(Environment.CurrentDirectory) + ".k";

            foreach (string f in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.*", SearchOption.AllDirectories))
            {
                tree.addFile(System.IO.Path.GetFullPath(f).Substring(Directory.GetCurrentDirectory().Length + 1));
            }

            createHeaderFile();




        }

        public bool createHeaderFile()
        {
            bool success = true;

            try
            {
                using (Stream stream = File.Open(filename, FileMode.Create))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    binaryFormatter.Serialize(stream, this);
                }


            }
            catch (Exception)
            {
                success = false;
            }

            return success;
           
        }

        public Reposistory readHeaderFile(string filename)
        {
            using (Stream stream = File.Open(filename, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (Reposistory)binaryFormatter.Deserialize(stream);
            }

        }

        public override string ToString()
        {
            return string.Format("\nFibw file: {0}\t\nName: {1}\nCreated: {2}\nTimestamp: {3}\n\n{4}", 
                                filename,
                                name, 
                                hash, 
                                timestamp,
                                tree.displayTree()
                                );
        }
    }

}
