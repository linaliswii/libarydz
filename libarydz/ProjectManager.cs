using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace libarydz
{
    public class ProjectManager
    {
        private List<Project> projects;

        public ProjectManager()
        {
            projects = new List<Project>();
        }

        public void AddProject(string projectName)
        {
            Project newProject = new Project(projectName);
            projects.Add(newProject);
        }

        public void RemoveProject(string projectName)
        {
            Project projectToRemove = projects.Find(p => p.Name == projectName);
            if (projectToRemove != null)
            {
                projects.Remove(projectToRemove);
            }
        }

        public void EditProject(string oldName, string newName)
        {
            Project projectToEdit = projects.Find(p => p.Name == oldName);
            if (projectToEdit != null)
            {
                projectToEdit.Edit(newName);
            }
        }

        public List<string> GetProjects()
        {
            return projects.Select(p => p.Name).ToList();
        }
    }

    public class TaskManager
    {
        private List<Task> tasks;
        private List<Project> projects;

        public TaskManager(List<Project> projects)
        {
            this.projects = projects;
            tasks = new List<Task>();
        }

        public void AddTask(string projectName, string taskName)
        {
            Project project = projects.Find(p => p.Name == projectName);
            if (project != null)
            {
                Task newTask = new Task(taskName);
                project.AddTask(newTask);
            }
        }

        public void MarkTaskAsDone(string projectName, string taskName)
        {
            Project project = projects.Find(p => p.Name == projectName);
            if (project != null)
            {
                Task task = project.Tasks.Find(t => t.Name == taskName);
                if (task != null)
                {
                    task.MarkAsDone();
                }
            }
        }

        public void EditTask(string projectName, string oldTaskName, string newTaskName)
        {
            Project project = projects.Find(p => p.Name == projectName);
            if (project != null)
            {
                Task taskToEdit = project.Tasks.Find(t => t.Name == oldTaskName);
                if (taskToEdit != null)
                {
                    taskToEdit.Edit(newTaskName);
                }
            }
        }

        public List<string> GetTasksForProject(string projectName)
        {
            Project project = projects.Find(p => p.Name == projectName);
            return project?.Tasks.Select(t => t.Name).ToList() ?? new List<string>();
        }
    }

    public class DatabaseManager
    {
        private SqlConnection connection;

        public DatabaseManager(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public void ConnectToDatabase()
        {
            connection.Open();
        }

        public void DisconnectFromDatabase()
        {
            connection.Close();
        }

        public void CreateProjectsTable()
        {
            SqlCommand command = new SqlCommand("CREATE TABLE Projects (Id INT PRIMARY KEY IDENTITY(1,1), Name NVARCHAR(255))", connection);
            command.ExecuteNonQuery();
        }

        public void CreateTasksTable()
        {
            SqlCommand command = new SqlCommand("CREATE TABLE Tasks (Id INT PRIMARY KEY IDENTITY(1,1), ProjectId INT, Name NVARCHAR(255), IsDone BIT)", connection);
            command.ExecuteNonQuery();
        }

        public void AddProjectToDatabase(string projectName)
        {
            SqlCommand command = new SqlCommand("INSERT INTO Projects (Name) VALUES (@Name)", connection);
            command.Parameters.AddWithValue("@Name", projectName);
            command.ExecuteNonQuery();
        }

        public void RemoveProjectFromDatabase(string projectName)
        {
            SqlCommand command = new SqlCommand("DELETE FROM Projects WHERE Name = @Name", connection);
            command.Parameters.AddWithValue("@Name", projectName);
            command.ExecuteNonQuery();
        }

        private List<Project> projects;

        public DatabaseManager(string connectionString, List<Project> projects)
        {
            connection = new SqlConnection(connectionString);
            this.projects = projects;
        }

        public void AddTaskToDatabase(string projectName, string taskName)
        {
            Project project = projects.Find(p => p.Name == projectName);
            if (project != null)
            {
                SqlCommand command = new SqlCommand("INSERT INTO Tasks (ProjectId, Name, IsDone) VALUES (@ProjectId, @Name, @IsDone)", connection);
                command.Parameters.AddWithValue("@ProjectId", project.Id);
                command.Parameters.AddWithValue("@Name", taskName);
                command.Parameters.AddWithValue("@IsDone", 0);
                command.ExecuteNonQuery();
            }
        }

        public void MarkTaskAsDoneInDatabase(string projectName, string taskName)
        {
            Project project = projects.Find(p => p.Name == projectName);
            if (project != null)
            {
                SqlCommand command = new SqlCommand("UPDATE Tasks SET IsDone = 1 WHERE ProjectId = @ProjectId AND Name = @Name", connection);
                command.Parameters.AddWithValue("@ProjectId", project.Id);
                command.Parameters.AddWithValue("@Name", taskName);
                command.ExecuteNonQuery();
            }
        }
    }

    public class InterfaceManager
    {
        private ProjectManager projectManager;
        private TaskManager taskManager;
        private DatabaseManager databaseManager;

        public InterfaceManager(ProjectManager manager, TaskManager taskManager, DatabaseManager databaseManager)
        {
            this.projectManager = manager;
            this.taskManager = taskManager;
            this.databaseManager = databaseManager;
        }

        public List<string> DisplayProjects()
        {
            return projectManager.GetProjects();
        }

        public void AddProjectThroughInterface(string projectName)
        {
            projectManager.AddProject(projectName);
            databaseManager.AddProjectToDatabase(projectName);
        }

        public void AddTaskThroughInterface(string projectName, string taskName)
        {
            taskManager.AddTask(projectName, taskName);
            databaseManager.AddTaskToDatabase(projectName, taskName);
        }
    }

    public class Project
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public List<Task> Tasks { get; private set; }

        public Project(string name)
        {
            Id = GenerateUniqueId();
            Name = name;
            Tasks = new List<Task>();
        }

        public void AddTask(Task task)
        {
            Tasks.Add(task);
        }

        public void Edit(string newName)
        {
            Name = newName;
        }

        private static int GenerateUniqueId()
        {
            return Guid.NewGuid().GetHashCode();
        }
    }

    public class Task
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public bool IsDone { get; private set; }

        public Task(string name)
        {
            Id = GenerateUniqueId();
            Name = name;
            IsDone = false;
        }

        public void MarkAsDone()
        {
            IsDone = true;
        }

        public void Edit(string newName)
        {
            Name = newName;
        }

        private static int GenerateUniqueId()
        {
            return Guid.NewGuid().GetHashCode();
        }
    }
}
