using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fibw
{
    class FConsole
    {
        Instance aFibw;
        public string currentFolder;
        string[] commands = new string[] { "init", "commit", "push", "quit", "pause", "info", "check", 
                                            "cd", "ls", "clear", "diff", "versions", "update",
                                            "revert", "cat", "tree"
                                          };
        int status = 1;

        public FConsole(Instance fibw)
        {
            aFibw = fibw;
            currentFolder = Path.GetFileName(Environment.CurrentDirectory);
            cd("project");
            start();
            
        }

        public void start()
        {
            Task.Factory.StartNew(() =>
            {
                string command = "";

                while (status != 0)
                {

                    while (status == 2) // On pause
                    {
                        System.Threading.Thread.Sleep(1000);
                    }

                    print();
                    command = Console.ReadLine();

                    if (commands.Any(command.Split(' ')[0].Equals))
                    {

                        commandRoute(command);

                    }
                    else
                    {
                        if (command.Trim() != "")
                        {
                            warningPrint("Unknown command!");
                        }
                        
                    }
                }
            });

        }

        private void commandRoute(string command)
        {
            string[] arguments = command.Split(' ');

            switch (command.Split(' ')[0])
            {
                case "init":
                    if (arguments.Length < 2) { print("Invalid command."); }
                    else
                    {
                        aFibw.init(arguments[1]);
                    }
                    
                    
                    break;
                case "commit": aFibw.commit(); break;
                case "push": aFibw.push(); break;
                case "tree":
                    Console.WriteLine(aFibw.rep.tree.displayTree());
                    
                    break;
                case "cat":
                    if (arguments.Length < 2) { print("Invalid command."); }
                    else
                    {
                        if (arguments.Length == 3)
                        {
                            Console.WriteLine(aFibw.rep.tree.cat(arguments[1], arguments[2]));
                        } else {
                            Console.WriteLine(aFibw.rep.tree.cat(arguments[1]));
                        }
                        
                    } 
                    break;
                case "revert":
                    if (arguments.Length < 3) { print("Invalid command."); } else
                    {
                        Console.WriteLine(aFibw.rep.tree.revert(arguments[1], arguments[2]));
                    }
                    break;
                case "update":
                    Console.WriteLine(aFibw.rep.tree.update(arguments[1]));
                    aFibw.rep.createHeaderFile();

                        break;
                case "diff": Console.WriteLine(aFibw.rep.tree.displayChanges()); break;
                case "versions":
                    if (arguments.Length < 2) { print("Invalid command."); } else{
                        Console.WriteLine(aFibw.rep.tree.displayVersions(arguments[1]));
                    }
                    break;
                case "check":

                    if (arguments.Length < 2) { print("Invalid command."); } else {
                        aFibw.check(arguments[1]);
                    }

                    break;
                case "cd":

                    if (arguments.Length < 2) { print("Invalid command."); }
                    else
                    {
                        cd(arguments[1]);
                    }

                    break;
                case "ls": ls(); break;
                case "quit": status = 0; break;
                case "pause": status = 2; break;
                case "clear": clear(); break;
                case "info": Console.WriteLine(aFibw.rep.ToString()); break;
                
            }
        }

        void ls()
        {
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());

            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                Console.WriteLine("{0, -30}\t Directory", d.Name);
            }

            foreach (FileInfo f in dir.GetFiles())
            {
                if (f.Name.Contains(".k"))
                {
                    Console.WriteLine("{0, -30}\t HeaderFile", f.Name);
                } else {
                    Console.WriteLine("{0, -30}\t File", f.Name);
                }
                
            }
        }

        void cd(string path)
        {

            if (Directory.Exists(path))
            {
                Directory.SetCurrentDirectory(path);
                currentFolder = Path.GetFileName(Environment.CurrentDirectory);
            }


        }
        void clear()
        {
            Console.Clear();
        }

        public void pause()
        {
            status = 2;
        }
        
        public void resume()
        {
            status = 1;
        }

        public bool isAlive()
        {
            return status != 0;
        }
        
        private void kill()
        {
            status = 0;
        }

        public string ask(string question)
        {
            Console.Write("\r[{0}] {1}", DateTime.Now.ToString("HH:mm:ss"), question);
            return Console.ReadLine();
        }

        public void print(string message)
        {
            Console.WriteLine("[{0}]fibw~{1}: {2}", DateTime.Now.ToString("HH:mm:ss"), currentFolder, message);
        }

        void print()
        {
            Console.Write("\r[{0}]fibw~{1}: ", DateTime.Now.ToString("HH:mm:ss"), currentFolder);
        }

        public void infoPrint(string message)
        {
            Console.WriteLine("[{0}]fibw~{1} {2}", DateTime.Now.ToString("HH:mm:ss"), currentFolder, message);
        }

        public void warningPrint(string message)
        {
            Console.WriteLine(message);
        }

    }
}
