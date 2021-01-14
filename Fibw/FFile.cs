using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Fibw
{
    [Serializable]
    class FFile
    {
        bool origin;

        public string filePath;
        public string filename;
        public string hash;

        public string file_content;

        public string owner;

        public DateTime creationDate;
        public DateTime lastModified;

        public Dictionary<string, FFile> versions;

        /*
         * Master FFile
         */
        public FFile(string relativeFilePath)
        {
            this.filePath = relativeFilePath;
            this.filename = Path.GetFileName(relativeFilePath);
            this.creationDate = System.IO.File.GetCreationTime(filePath);
            versions = new Dictionary<string, FFile>();

            origin = true;

            newVersion();

        }

        /*
         * Child FFile
         */
        public FFile()
        {
            versions = new Dictionary<string, FFile>();
            origin = false;

        }

        public bool isChild()
        {
            return !origin;
        }

        public bool hasChanged()
        {
            return computeHash() != hash;
        }

        public string getContent(string fileHash)
        {
            return fileHash == hash ? file_content : versions[fileHash].file_content;
        }

        public void revert(string fileHash)
        {
            string currentVersion = hash;
            File.Delete(filePath);
            FFile aFile;
            if (fileHash == hash)
            {
                aFile = this;
            } else{
                aFile = versions[fileHash];
            }
            
            aFile.origin = true;

            copyVersion(this, aFile);

            File.AppendAllText(filePath, file_content);

            // remove old verrsion
            versions.Remove(currentVersion);
        }

        public void copyVersion(FFile from, FFile to)
        {

            from.filePath = to.filePath;
            from.filename = to.filename;
            from.hash = to.hash;
            from.file_content = to.file_content;
            from.owner = to.owner;
            from.creationDate = to.creationDate;
            from.lastModified = to.lastModified;

        }

        public string update()
        {
            if (hasChanged())
            {
                newVersion();
                return displayVersions();
            }
            else
            {
                return "Nothing changed.";
            }
        }

        public void newVersion()
        {
            string currentFileHash = computeHash();
            hash = currentFileHash;

            lastModified = System.IO.File.GetLastAccessTime(filePath);

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs, Encoding.UTF8);
            file_content = sr.ReadToEnd();

            FFile aFile = new FFile();
            copyVersion(aFile, this);

            try
            {
                versions.Add(hash, aFile);
            }
            catch (Exception)
            {
                sr.Close();
                revert(currentFileHash);
            }

        }

        public string displayVersions()
        {
            string output = string.Format("{0, -30}\t {1, -15}\t {2}\n", "File name", "Last modified", "Hash");

            // Output current version
            output += string.Format("{0, -30}\t {1,-15}\t {2}\n", "["+filename+"]", lastModified, hash);

            // Skip current version
            for (int i = versions.Count-2; i >= 0; i--)
            {
                var version = versions.ElementAt(i);
                output += string.Format("{0, -30}\t {1,-15}\t {2}\n", version.Value.filename, version.Value.lastModified, version.Value.hash);
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
