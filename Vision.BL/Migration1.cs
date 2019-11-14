using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vision.BL.Model;
using Softwaremeisterei.Lib;
using System.Linq;
using System.Collections.ObjectModel;

namespace Vision.BL
{
    public class Migration1
    {
        public void Migrate(string oldProjectFilePath, List<Node> result)
        {
            var xml = File.ReadAllText(oldProjectFilePath);
            var srcProject = Serialization.ParseXml<Migration1.Project>(xml);

            Migrate(result, srcProject.Root, new string[]{});
        }

        private void Migrate(List<Node> destNodes, Migration1.Project.Node migNode, string[] tags)
        {
            if (migNode.NodeType == Project.NodeType.Link)
            {
                var destNode = new Node
                {
                    Name = migNode.Name,
                    Url = migNode.Url,
                    IsFavorite = migNode.IsFavorite,
                    Tags = new ObservableCollection<string>(tags)
                };
                destNodes.Add(destNode);
            }

            foreach (var migSubNode in migNode.Nodes)
            {
                Migrate(destNodes, migSubNode, tags.Union(new[] { migNode.Name }).ToArray());
            }
        }

        [DataContract(Name = "Project")]
        public class Project
        {
            [DataMember]
            public Node Root { get; set; }

            [DataContract]
            public class Node
            {
                [DataMember]
                public NodeType NodeType { get; set; }

                [DataMember]
                public string Name { get; set; }

                [DataMember]
                public string Url { get; set; }

                [DataMember]
                public bool IsFavorite { get; set; }

                [DataMember]
                public List<Node> Nodes { get; set; }
            }

            public enum NodeType
            {
                Folder,
                Link
            }
        }

    }
}
