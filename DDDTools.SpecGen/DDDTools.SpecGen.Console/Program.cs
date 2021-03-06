﻿using DDDTools.SpecGen.Spec;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDTools.SpecGen.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.LogAction = m => System.Console.WriteLine(m);

            if(HasSwitch("?", args))
            {
                Log.LogMessage("Usage:");
                Log.LogMessage("-p <path>        | Specifies the path to the directory to use (defaults to currect directory)");
                Log.LogMessage("-c <name>        | Specifies the company name");
                Log.LogMessage("-n <name>        | Specifies the product name");
                Log.LogMessage("-d <description> | Specifies the product description");
                Log.LogMessage("-s               | Adds a default stylesheet");
                return;
            }

            var path = GetArgument("p", args) ?? @"\";
            var addStyleSheet = HasSwitch("s", args);
            var companyName = GetArgument("c", args) ?? "Default Company Name";
            var productName = GetArgument("n", args) ?? "Product Name";
            var productDescription = GetArgument("d", args) ?? "Product Description";

            var specification = ProductSpecification.Create(companyName, productName, productDescription, path);
            var formatter = new HtmlFormatter(specification);
            var html = formatter.FormatDomains();

            var htmlFile = new FileInfo("specification.html");
            if (!htmlFile.Exists)
                htmlFile.Create().Dispose();

            using (var fw = new StreamWriter(htmlFile.Open(FileMode.Truncate, FileAccess.Write)))
            {
                fw.Write(html);
            }

            if (addStyleSheet)
                FileHelper.SaveCssFile();

            Log.LogMessage("Press any key to exit");
            System.Console.ReadKey();
        }

        private static string GetArgument(string @switch, string[] args)
        {
            var switches = new [] { "-" + @switch, "/" + @switch };
            var arguments = args.ToList();

            var index1 = arguments.IndexOf(switches[0]);
            var index2 = arguments.IndexOf(switches[1]);
            if (index1 < 0 && index2 < 0)
                return null;

            var index = index1 >= 0 ? index1 : index2;
            if (arguments.Count <= index + 1)
                return null;

            return args[index + 1];
        }

        private static bool HasSwitch(string @switch, string[] args)
        {
            var switches = new[] { "-" + @switch, "/" + @switch };
            var arguments = args.ToList();

            var index1 = arguments.IndexOf(switches[0]);
            var index2 = arguments.IndexOf(switches[1]);
            return index1 >= 0 || index2 >= 0;
        }
    }
}
