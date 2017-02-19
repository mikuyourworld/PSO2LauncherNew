using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace i_dont_like_how_tweaker_work_some_how
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {

            }

            StringWriter writer = new StringWriter();
            Console.SetOut(writer);
            Console.WriteLine("hello world");

            StringReader reader = new StringReader(writer.ToString());
            string str = reader.ReadToEnd();

            ProcessStartInfo asd = new ProcessStartInfo("");
            asd.Arguments = "";
            asd.UseShellExecute = false;
            asd.Verb = "runas";
            
            Process myProc = new Process();
            myProc.StartInfo = asd;
            
        }
    }
}
