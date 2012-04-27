using System;

namespace tournament
{
    public class assemblyNameAndType
    {
        public readonly string AssemblyName;
        public readonly string AssemblyFolder;
        public readonly Type theType;
        public bool isHousePlayer = false;
        public string ownerName = "House";

        public assemblyNameAndType(string newAssName, string newAssFolder, Type newType)
        {
            AssemblyName = newAssName;
            AssemblyFolder = newAssFolder;
            theType = newType;
        }
    }
}