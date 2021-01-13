using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Fibw
{
    [Serializable()]
    class FFile
    {
        public string filePath;
        public string filename { get; set; }
        public string hash;

        string file_content;

        public string owner;

        public DateTime creationDate;
        public DateTime lastModified;

        Dictionary<string, FFile> versions;

        public FFile(string relativeFilePath)
        {
            this.filePath = relativeFilePath;
            this.filename = Path.GetFileName(relativeFilePath);
            this.creationDate = System.IO.File.GetCreationTime(filePath);
            versions = new Dictionary<string, FFile>();

            newVersion();
        }

        
        public bool hasChanged()
        {
            return computeHash() != hash;
        }

        public void newVersion()
        {
            hash = computeHash();
            lastModified = System.IO.File.GetLastWriteTime(filePath);

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs, Encoding.UTF8);
            string file_content = sr.ReadToEnd();

            versions.Add(hash, this);

        }

        public string displayVersions()
        {
            string output = string.Format("{0, -30}\t {1, -15}\t {2}\n", "File name", "Hash", "Last modified");

            foreach (KeyValuePair<string, FFile> version in versions)
            {                
                output += string.Format("{0, -30}\t {1,-15}\t {2}", version.Value.filename, version.Value.lastModified, version.Key);
            }

            return output;
        }

        string computeHash()
        {
            using (var stream = new BufferedStream(File.OpenRead(filePath), 1200000))
            {
                SHA256Managed sha = new SHA256Managed();
                byte[] checksum = sha.ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", String.Empty);
            }
        }

    }
}
