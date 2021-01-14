using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Fibw
{
    [Serializable]
    class Tree
    {

        string               reposistoryHash;
        public string        hash;

        public List<FFile> files;
        public List<Commit> commits;

        public Tree(string childOf)
        {

            reposistoryHash = childOf;
            hash = Guid.NewGuid().ToString().Replace("-", String.Empty);

            files = new List<FFile>();
            commits = new List<Commit>();

        }


        public string displayChanges()
        {

            List<FFile> before = files;
            List<FFile> now    = new List<FFile>();

            string output = string.Format("{0, -30}\t {1}\n", "File", "Changed");

            foreach (FFile aFile in before)
            {
                output += string.Format("{0, -30}\t {1}\n", aFile.filePath, aFile.hasChanged()? "yes":"no");
            }

            return output;

        }

        public string displayVersions(string filenameOrHash)
        {
            int i = 0;
            // While we're searching for the file.
            while (i < files.Count && (files[i].filename != filenameOrHash && files[i].filePath != filenameOrHash))
            {
                i++;
            }

            if (i < files.Count)
            {
                return files[i].displayVersions();
            }
            return "File not found.";
        }

        public string cat(string filenameOrHash, string fileVersionHashTo)
        {
            int i = 0;
            // While we're searching for the file.
            while (i < files.Count && (files[i].filename != filenameOrHash && files[i].filePath != filenameOrHash)) { i++; }

            if (i < files.Count)
            {
                return files[i].getContent(fileVersionHashTo);
            }
            return "File not found.";
        }
        public string cat(string filenameOrHash)
        {
            int i = 0;
            // While we're searching for the file.
            while (i < files.Count && (files[i].filename != filenameOrHash && files[i].filePath != filenameOrHash)) { i++; }

            if (i < files.Count)
            {
                return files[i].getContent(files[i].hash);
            }
            return "File not found.";
        }

        public string revert(string filenameOrHash, string versionHash)
        {
            int i = 0;
            // While we're searching for the file.
            while (i < files.Count && (files[i].filename != filenameOrHash && files[i].filePath != filenameOrHash)) { i++; }

            if (i < files.Count)
            {
                Console.WriteLine("{0} {1} -> {2}", files[i].filePath, versionHash, files[i].hash);
                files[i].revert(versionHash);
                return files[i].displayVersions();
            }
            return "File not found.";
        }


        public string update(string filenameOrHash)
        {
            int i = 0;
            // While we're searching for the file.
            while (i < files.Count && (files[i].filename != filenameOrHash && files[i].filePath != filenameOrHash))
            {
                i++;
            }

            if (i < files.Count)
            {
                return files[i].update();

            }
            return "File not found.";
        }

        public bool addFile(string relativeFilePath)
        {
            files.Add(new FFile(relativeFilePath));
            return true;
        }

        public bool removeFile(FFile record)
        {
            files.Remove(record);
            return true;
        }

        public int treeLength()
        {
            return files.Count;
        }

        public string displayTree()
        {
            string output = string.Format("- Tree {0} -\n", hash); 

            foreach (FFile aFile in files)
            {
                output += string.Format("{0, -30}\t {1}\n",aFile.filePath, aFile.hash);
            }
            return output;
        }

    }
}
