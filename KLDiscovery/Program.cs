using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace KLDiscovery
{
    class Program
    {
        static void Main(string[] args)
        {
            Program helperMethods = new Program();
            Console.WriteLine("Directory that contains files for analysis:");
            string startDirectory = helperMethods.getInput("directory");
            Console.WriteLine("Path for the output file:");
            string targetFile = helperMethods.getInput("file");
            Console.WriteLine("Include all subdirectories? Y/N:");
            string tempFlag = helperMethods.getInput("flag");

            //could be moved out to a getFilesList method 
            List<string> filesList = new List<string>();
            if (tempFlag == "Y")
            {
                string[] filePaths = Directory.GetFiles(startDirectory, "*", SearchOption.AllDirectories);
                filesList.AddRange(filePaths);
            }
            else
            {
                string[] filePaths = Directory.GetFiles(startDirectory);
                filesList.AddRange(filePaths);
            }

            List<string> csvLines = new List<string>();
            for (int i = 0; i < filesList.Count; i++)
            {
                string fileType = helperMethods.getFileType(filesList[i]);
                if (fileType == "JPG" || fileType== "PDF")
                {
                    string fileHash = helperMethods.getMD5Hash(filesList[i]);
                    csvLines.Add($"{filesList[i]} | {fileType} | {fileHash}");
                }
            }

            using (StreamWriter csvWriter = new StreamWriter(targetFile, true))
            {
                foreach (string csvLine in csvLines)
                {
                    csvWriter.WriteLine(csvLine);
                }
            }
            Console.WriteLine("Analysis complete. Press any key to close this prompt.");
            string close = Console.ReadLine();

        }
        /// <summary>
        /// Helper method to get input from the command line and then validate it based on the  input type

        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        private string getInput(string inputType)
        {
            string inputValue = Console.ReadLine();
            switch (inputType)
            {
                case "directory":
                    {
                        while (!Directory.Exists(inputValue))
                        {
                            Console.WriteLine("Directory does not exist, please try again(remember to use the full path name) :");
                            inputValue = Console.ReadLine();
                        }
                        return inputValue;
                    }
                case "file":
                    {
                        while (!File.Exists(inputValue))
                        {
                            Console.WriteLine("file does not exist, please try again(remember to use the full path name) :");
                            inputValue = Console.ReadLine();
                        }
                        return inputValue;
                    }
                case "flag":
                    {
                        while (inputValue.ToUpper() != "Y" && inputValue.ToUpper() != "N")
                        {
                            Console.WriteLine("Please use Y or N for flag values");
                            inputValue = Console.ReadLine();
                        }
                        return inputValue;
                    }
                default:
                    //no validation
                    return inputValue;
            }
        }
        
        /// <summary>
        /// gets the file type based on the file signature we recieve from the byte array
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string getFileType(string filePath)
        {
            byte[] buffer;
            string fileSignature = "";
            using (BinaryReader br = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read)))
            {
                buffer = br.ReadBytes(4);//we only care about the first 2 for JPG but we need all 4 for PDF 
                fileSignature = BitConverter.ToString(buffer);
                //remove hyphens
                fileSignature = fileSignature.Replace("-", String.Empty);
            }
            string tmp = "";
            //this could also be done as a switch statement
            if (fileSignature == "25504446")
            {
                tmp = "PDF";
            }
            if (fileSignature.StartsWith("FFD8"))
            {
                tmp = "JPG";
            }
            return tmp;
        }
        /// <summary>
        /// Reads the content of the file at the filePath location and returns the MD5 hash in string form
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string getMD5Hash(string filePath)
        {

            MD5 md5Hasher = MD5.Create();
            byte[] md5bytes = md5Hasher.ComputeHash(File.ReadAllBytes(filePath));
            StringBuilder md5HashContent = new StringBuilder();
            for (int i = 0; i < md5bytes.Length; i++)
            {
                md5HashContent.Append(md5bytes[i].ToString("x2"));
            }
            string tmp = md5HashContent.ToString();
            return tmp;
        }
    }
}
