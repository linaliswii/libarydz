using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using libarydz;

namespace libarydz.Tests
{
    [TestClass]
    public class Class1
    {
        [TestMethod]
        public void AddProject_ShouldAddProjectToList() //проверка на корректное добавление в список нового проекта
        {
            // Arrange
            var projectManager = new ProjectManager();
            string projectName = "TestProject";

            // Act
            projectManager.AddProject(projectName);

            // Assert
            CollectionAssert.Contains(projectManager.GetProjects(), projectName);
        }

        [TestMethod]
        public void RemoveProject_ShouldRemoveProjectFromList() //проверка на то что после удаления проекта он больше не присутствует в списке
        {
            // Arrange
            var projectManager = new ProjectManager();
            string projectName = "TestProject";
            projectManager.AddProject(projectName);

            // Act
            projectManager.RemoveProject(projectName);

            // Assert
            CollectionAssert.DoesNotContain(projectManager.GetProjects(), projectName);
        }

        [TestMethod]
        public void EditProject_NonExistingProject_ShouldNotThrowException() //проверка попытки ред-ия несуществующего проекта
        {
            // Arrange
            var projectManager = new ProjectManager();
            string projectName = "NonExistingProject";
            string newProjectName = "EditedProject";

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => projectManager.EditProject(projectName, newProjectName));
        }

        [TestMethod]
        public void RemoveProject_NonExistingProject_ShouldNotThrowException() //проверка попытки удаления несущ. проекта
        {
            // Arrange
            var projectManager = new ProjectManager();
            string projectName = "NonExistingProject";

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => projectManager.RemoveProject(projectName));
        }

        [TestMethod]
        public void EditProject_ShouldEditProjectName() //тест на редактирование существ. проекта
        {
            // Arrange
            var projectManager = new ProjectManager();
            string projectName = "TestProject";
            string newProjectName = "EditedProject";

            // Act
            projectManager.AddProject(projectName);
            projectManager.EditProject(projectName, newProjectName);

            // Assert
            CollectionAssert.DoesNotContain(projectManager.GetProjects(), projectName);
            CollectionAssert.Contains(projectManager.GetProjects(), newProjectName);
        }

    }
}
