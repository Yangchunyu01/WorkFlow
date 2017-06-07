using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ProjectServer.Client;
using Project.Performance.Model;
using Project.Performance.Utility;

namespace Project.Performance.Repository
{
    public class ProjectCSOMClient : PerformanceBaseClient, ICSOMClient<ProjectModel>
    {
        #region Properties
        private IEnumerable<EnterpriseResource> availableResources;
        #endregion

        #region Constructors
        public ProjectCSOMClient()
        {
        }
        #endregion

        #region ICSOMClient
        public bool Create(ProjectModel model)
        {
            if (model == null)
            {
                throw new ArgumentException("The ProjectModel is null.");
            }
            else
            {
                IEnumerable<PublishedProject> projects = GetEntities();
                this.availableResources = new ResourceCSOMClient().GetEntities();
                for (int i = 0; i < model.Count; i++)
                {
                    string projectName = model.NamePrefix + i.ToString();
                    if (projects.Any(item => item.Name == projectName))
                    {
                        // No need to do anything currently
                    }
                    else
                    {
                        try
                        {
                            // Create an empty project
                            PublishedProject project = CreateEmptyProject(projectName);

                            // Check out the project
                            DraftProject draftProject = CheckOutProject(project);

                            // Get resource for project
                            List<EnterpriseResource> projectResources = new List<EnterpriseResource>();
                            int[] resourcesIndex = RandomHelper.GetRandomIndex(model.ProjectTeamCount, this.availableResources.Count() - 1);
                            foreach (int index in resourcesIndex)
                            {
                                projectResources.Add(this.availableResources.ElementAt(index));
                            }

                            // Build team
                            BuildTeam(draftProject, projectResources);

                            // Create task
                            IEnumerable<IEnumerable<TaskCreationInformation>> tasks = CreateTask(model.TaskModel);

                            // Add task
                            AddTaskToProject(draftProject, tasks, projectResources, model.TaskModel.Assignments);

                            // Publish
                            PublishProject(draftProject);
                        }
                        catch (Exception ex)
                        {
                            LoggerHelper.Instance.Fail(string.Format("Filed to create Project: {0}.\nMessage: {1}. \nMore:{2} \nRetrying...", projectName, ex.Message, ex.StackTrace));
                        }
                    }
                }
            }

            return true;
        }

        public bool Delete(ProjectModel model)
        {
            IEnumerable<PublishedProject> projects = BaseProjectContext.LoadQuery(BaseProjectContext.Projects.Where(item => item.Name.StartsWith(model.NamePrefix)));
            BaseProjectContext.ExecuteQuery();

            foreach (PublishedProject project in projects)
            {
                BaseProjectContext.Load(project);
                BaseProjectContext.ExecuteQuery();
                if (project.IsCheckedOut)
                {
                    QueueJob job = project.Draft.CheckIn(true);
                    BaseProjectContext.ExecuteQuery();
                    WaitForJobComplete(job);
                }
                project.DeleteObject();
                BaseProjectContext.ExecuteQuery();
            }
            return true;
        }

        public int GetCount(XMLModel model)
        {
            if (model != null && model.ProjectModelList != null)
            {
                return GetEntities().Count(item => model.ProjectModelList.Any(obj => item.Name.StartsWith(obj.NamePrefix)));
            }
            else
            {
                return 0;
            }
        }

        public IEnumerable<PublishedProject> GetEntities()
        {
            IEnumerable<PublishedProject> projects = null;
            ExcuteJobWithRetries(() =>
            {
                projects = BaseProjectContext.LoadQuery(BaseProjectContext.Projects);
                BaseProjectContext.ExecuteQuery();
            },
            "Query Project");
            return projects ?? new List<PublishedProject>();
        }
        #endregion

        #region Private Methods
        private PublishedProject CreateEmptyProject(string projectName)
        {
            PublishedProject project = null;

            ProjectCreationInformation creationInfo = new ProjectCreationInformation()
            {
                Name = projectName,
                Start = DateTime.Now
            };

            ExcuteJobWithRetries(() =>
            {
                LoggerHelper.Instance.Comment("About to Create Project with Name: " + creationInfo.Name);
                project = BaseProjectContext.Projects.Add(creationInfo);
                QueueJob job = BaseProjectContext.Projects.Update();
                this.WaitForJobComplete(job);

                BaseProjectContext.Load(project);
                BaseProjectContext.ExecuteQuery();
                LoggerHelper.Instance.Comment("Finish Creating empty Project with Name: " + creationInfo.Name);
            },
            "Create Project");
            return project;
        }

        private DraftProject CheckOutProject(PublishedProject project)
        {
            DraftProject draftProject = null;
            ExcuteJobWithRetries(() =>
            {
                LoggerHelper.Instance.Comment("About to Checkout Project with Name: " + project.Name);
                draftProject = project.CheckOut();
                BaseProjectContext.Load(draftProject);
                BaseProjectContext.ExecuteQuery();
                LoggerHelper.Instance.Comment("Finish Checkout Project with Name: " + project.Name);
            },
            "Checkout Project");
            return draftProject;
        }

        private void BuildTeam(DraftProject project, IEnumerable<EnterpriseResource> resources)
        {
            foreach (EnterpriseResource resource in resources)
            {
                LoggerHelper.Instance.Comment(string.Format("About to Add Resource: {0} to Project: {1}", resource.Name, project.Name));
                project.ProjectResources.AddEnterpriseResource(resource);
            }

            ExcuteJobWithRetries(() =>
            {
                QueueJob job = project.Update();
                WaitForJobComplete(job);
                BaseProjectContext.ExecuteQuery();
                LoggerHelper.Instance.Comment(string.Format("Finish Adding Resources to Project: {0}", project.Name));
            },
            "Add Resource to Project");
        }

        private IEnumerable<IEnumerable<TaskCreationInformation>> CreateTask(TaskModel model)
        {
            // Save different level task: lstTask[0,...], the index = level
            List<List<TaskCreationInformation>> lstTask = new List<List<TaskCreationInformation>>();
            for (int i = 0; i < model.Count; i++)
            {
                int levelDepth = RandomHelper.Random(0, lstTask.Count);
                Guid parentId = levelDepth == 0 ? default(Guid) : lstTask[levelDepth - 1][RandomHelper.Random(0, lstTask[levelDepth - 1].Count - 1)].Id;
                TaskCreationInformation task = NewTask(model, i, parentId);
                if (levelDepth == lstTask.Count)
                {
                    lstTask.Add(new List<TaskCreationInformation> { task });
                }
                else
                {
                    lstTask[levelDepth].Add(task);
                }
            }
            return lstTask;
        }

        private TaskCreationInformation NewTask(TaskModel model, int value, Guid parentId)
        {
            return new TaskCreationInformation()
            {
                Id = Guid.NewGuid(),
                Name = model.TaskNamePrefix + value.ToString(),
                Start = DateTime.Today.AddDays(RandomHelper.Random(model.Duration.Min, model.Duration.Max)),
                IsManual = false,
                Duration = RandomHelper.Random(model.Duration.Min, model.Duration.Max).ToString() + "d",
                ParentId = parentId
            };
        }

        private void AddTaskToProject(DraftProject project, IEnumerable<IEnumerable<TaskCreationInformation>> tasks, IEnumerable<EnterpriseResource> resources, int resourceCount)
        {
            foreach (TaskCreationInformation taskInfo in tasks.SelectMany(item => item))
            {
                LoggerHelper.Instance.Comment(string.Format("About to Add Task: {0} to Project: {1}", taskInfo.Name, project.Name));
                DraftTask draftTask = project.Tasks.Add(taskInfo);
                AddAssignmentToTask(draftTask, resources, resourceCount, taskInfo.Id);
            }

            ExcuteJobWithRetries(() =>
            {
                QueueJob job = project.Update();
                WaitForJobComplete(job);
                BaseProjectContext.ExecuteQuery();
                LoggerHelper.Instance.Comment(string.Format("Finish Adding Task to Project: {0}", project.Name));
            },
            "Add Task to Project");

            // Add tasks link
            int levelIndex = RandomHelper.Random(0, tasks.Count() - 1);
            AddLinkToProject(project, tasks.ElementAt(levelIndex));
        }

        private void AddAssignmentToTask(DraftTask task, IEnumerable<EnterpriseResource> resources, int resourceCount, Guid taskId)
        {
            int resourceCountPerTask = RandomHelper.Random(1, resourceCount);
            int[] resourcesIndex = RandomHelper.GetRandomIndex(resourceCountPerTask, resources.Count() - 1);

            for (int i = 0; i < resourcesIndex.Count(); i++)
            {
                AssignmentCreationInformation assignmentInfo = new AssignmentCreationInformation
                {
                    ResourceId = resources.ElementAt(resourcesIndex[i]).Id,
                    TaskId = taskId
                };
                task.Assignments.Add(assignmentInfo);
            }
        }

        private void AddLinkToProject(DraftProject project, IEnumerable<TaskCreationInformation> tasks)
        {
            // Load tasks
            IEnumerable<DraftTask> draftTasks = BaseProjectContext.LoadQuery(project.Tasks);
            BaseProjectContext.ExecuteQuery();

            // Disorder the tasks 
            List<TaskCreationInformation> levelTasks = tasks.ToList();
            List<TaskCreationInformation> linkTasks = new List<TaskCreationInformation>();
            while (levelTasks.Count > 0)
            {
                int taskIndex = RandomHelper.Random(0, levelTasks.Count - 1);
                linkTasks.Add(levelTasks[taskIndex]);
                levelTasks.RemoveAt(taskIndex);
            }

            // Add links
            for (int i = 0; i < linkTasks.Count - 1; i++)
            {
                TaskLinkCreationInformation linkInfo = NewFSTaskLink(linkTasks[i].Id, linkTasks[i + 1].Id);
                DraftTask task = draftTasks.Where(item => item.Id == linkInfo.StartId).FirstOrDefault();
                task.Successors.Add(linkInfo);
            }

            ExcuteJobWithRetries(() =>
            {
                QueueJob job = project.Update();
                WaitForJobComplete(job);
                BaseProjectContext.ExecuteQuery();
                LoggerHelper.Instance.Comment(string.Format("Finish Adding Links to Project: {0}", project.Name));
            }, "Add Links to project");
        }

        private TaskLinkCreationInformation NewFSTaskLink(Guid startId, Guid endId)
        {
            return NewTaskLink(startId, endId, DependencyType.FinishStart);
        }

        private TaskLinkCreationInformation NewTaskLink(Guid startId, Guid endId, DependencyType type)
        {
            return new TaskLinkCreationInformation()
            {
                DependencyType = type,
                StartId = startId,
                EndId = endId
            };
        }

        private void PublishProject(DraftProject project)
        {
            ExcuteJobWithRetries(() =>
            {
                LoggerHelper.Instance.Comment("About to Pubilsh Project with Name: " + project.Name);
                QueueJob job = project.Publish(true);
                BaseProjectContext.ExecuteQuery();
                LoggerHelper.Instance.Comment("Finish Pubilsh Project with Name: " + project.Name);
            }, "Publish Project");
        }
        #endregion
    }
}
