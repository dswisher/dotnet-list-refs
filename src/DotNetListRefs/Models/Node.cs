// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;

namespace DotNetListRefs.Models
{
    public abstract class Node
    {
        private static int nextId = 1;

        protected Node(string name)
        {
            Name = name;
            Id = nextId++;
        }


        public int Id { get; private set; }
        public string Name { get; private set; }

        public abstract void Write(StreamWriter writer);
    }
}
