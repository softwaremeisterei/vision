using Softwaremeisterei.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.BL.Model;
using Vision.Data;

namespace Vision.BL
{
    public class Persistor
    {
        public static string ProjectFileExtension = "vsx";
        public static string RecentProjectsFilePath;

        Storage _storage;

        public Persistor()
        {
            _storage = new Storage();

            RecentProjectsFilePath = Path.Combine(Paths.GetAppDirectory(), "RecentProjects.xml");
        }

        public void SaveProject(Project project)
        {
            _storage.Save(project, project.Path);
        }

        public Project LoadProject(string filename)
        {
            var result = _storage.Load<Project>(filename);
            return result;
        }

        public RecentProjects LoadRecentProjects()
        {
            if (File.Exists(RecentProjectsFilePath))
            {
                var xml = File.ReadAllText(RecentProjectsFilePath);
                var recentProjects = Serialization.ParseXml<RecentProjects>(xml);
                return recentProjects;
            }
            return new RecentProjects();
        }

        public void AddRecentProject(string filePath)
        {
            var recentProjects = LoadRecentProjects();
            recentProjects.Projects.Add(new RecentProject { Name = Path.GetFileName(filePath), Path = filePath, LastUsageDate = DateTime.Now });
            SaveRecentProjects(recentProjects);
        }

        public void UpateRecentProject(RecentProject recentProject)
        {
            var recentProjects = LoadRecentProjects();
            recentProjects.Projects.RemoveAll(p => string.Equals(p.Path, recentProject.Path, StringComparison.OrdinalIgnoreCase));
            recentProjects.Projects.Add(recentProject);
            SaveRecentProjects(recentProjects);
        }

        private void SaveRecentProjects(RecentProjects recentProjects)
        {
            var xml = Serialization.ToXml(recentProjects);
            File.WriteAllText(RecentProjectsFilePath, xml);
        }

        public void RemoveRecentProject(RecentProject recentProject)
        {
            var recentProjects = LoadRecentProjects();
            recentProjects.Projects.RemoveAll(p => string.Equals(p.Path, recentProject.Path, StringComparison.OrdinalIgnoreCase));
            SaveRecentProjects(recentProjects);
        }
    }
}
