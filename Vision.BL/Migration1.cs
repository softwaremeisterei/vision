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
        public void Migrate(string oldProjectFilePath, BL.Model.Project destProject)
        {
            var xml = File.ReadAllText(oldProjectFilePath);
            var srcProject = Serialization.ParseXml<Migration1.Project>(xml);

            Migrate(destProject.Root, srcProject.Root);
        }

        private void Migrate(Node destParentNode, Migration1.Project.FolderNode srcFolder)
        {
            foreach (var srcSubFolder in srcFolder.Folders)
            {
                var destFolder = new Node { Name = srcSubFolder.Name, NodeType = NodeType.Folder };
                destParentNode.Nodes.Add(destFolder);
                Migrate(destFolder, srcSubFolder);
            }

            foreach (var oldNode in srcFolder.Nodes)
            {
                destParentNode.Nodes.Add(new Node { Name = oldNode.Name, Url = oldNode.Url, NodeType = NodeType.Link });
            }
        }

        [DataContract(Name="Project")]
        public class Project
        {
            [DataMember]
            public FolderNode Root { get; set; }

            [DataContract]
            public class FolderNode
            {
                [DataMember]
                public string Name { get; set; }

                [DataMember]
                public List<FolderNode> Folders { get; set; }

                [DataMember]
                public List<Node> Nodes { get; set; }
            }

            [DataContract]
            public class Node
            {
                [DataMember]
                public string Name { get; set; }

                [DataMember]
                public string Url { get; set; }

                [DataMember]
                public List<Node> Nodes { get; set; }
            }
        }

    }
}
