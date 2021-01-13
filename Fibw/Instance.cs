using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Fibw
{

    class Instance
    {
        FConsole cns;
        public Reposistory rep;

        /*
         * Start a user console.
         */
        public Instance()
        {

            cns = new FConsole(this);

            readCurrentReposistory();


            // Wait for end session
            while (cns.isAlive()) {
                System.Threading.Thread.Sleep(1000);
            }

        }

        /*
         * 
         */
        private void readCurrentReposistory()
        {
            string headerFileName = cns.currentFolder + ".k";

            if (File.Exists(headerFileName))
            {
                rep = new Reposistory().readHeaderFile(headerFileName);
                cns.warningPrint("loaded -> " + rep.name);
            } 

        }

        public void check(string filename)
        {
            cns.print(new Reposistory().readHeaderFile(filename).ToString());
        }

        /*
         * Init a Fibw reposistory.
         */
        public void init(string projectName)
        {
            //check if reposistory already exists 
            if (File.Exists(cns.currentFolder + ".k")) {
                cns.warningPrint("A reposistory is already initialized!");
                //return;
            }

            // Init a reposistory
            rep = new Reposistory(projectName);
            
            cns.print(rep.ToString());


        }

        public void commit()
        {

        }

        public void push()
        {

        }

    }

}
