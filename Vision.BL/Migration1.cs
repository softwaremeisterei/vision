using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vision.BL.Model;
using Softwaremeisterei.Lib;
using System.Linq;

namespace Vision.BL
{

    public class Migration1
    {
        public void Migrate(string oldProjectFilePath, Project project)
        {
            var json = File.ReadAllText(oldProjectFilePath);
            var oldFormat = Serialization.ParseJson<OldFormat>(json);

            Migrate(project.Root, oldFormat.Nodes);
        }

        private void Migrate(FolderNode parentFolder, List<OldFormat.Node> oldNodes)
        {
            if (oldNodes != null)
            {
                foreach (var oldNode in oldNodes)
                {
                    if (oldNode.Nodes.Any())
                    {
                        var folder = new FolderNode { Name = oldNode.Title };
                        if (!string.IsNullOrEmpty(oldNode.Url))
                        {
                            folder.Nodes.Add(new Node { Name = oldNode.Title, Url = oldNode.Url });
                        }
                        parentFolder.Folders.Add(folder);
                        Migrate(folder, oldNode.Nodes);
                    }
                    else
                    {
                        parentFolder.Nodes.Add(new Node { Name = oldNode.Title, Url = oldNode.Url });
                    }
                }
            }
        }

        [DataContract]
        class OldFormat
        {
            [DataMember]
            public List<Node> Nodes { get; set; }

            [DataContract]
            public class Node
            {
                [DataMember]
                public string Title { get; set; }

                [DataMember]
                public string Url { get; set; }

                [DataMember]
                public List<Node> Nodes { get; set; }
            }
        }

    }
}
