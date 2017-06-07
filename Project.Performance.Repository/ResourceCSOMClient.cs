using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ProjectServer.Client;
using Project.Performance.Model;
using Project.Performance.Utility;

namespace Project.Performance.Repository
{
    public class ResourceCSOMClient : PerformanceBaseClient, ICSOMClient<ResourcePoolModel>
    {
        #region ICSOMClient
        public bool Create(ResourcePoolModel model)
        {
            if (model == null)
            {
                throw new ArgumentException("The ResourcePoolModel is null.");
            }
            else
            {
                IEnumerable<EnterpriseResource> resources = GetEntities();
                for (int i = 0; i < model.Count; i++)
                {
                    string resourceName = model.NamePrefix + i.ToString();
                    if (resources.Any(item => item.Name == resourceName && item.ResourceType.ToString().ToLower() == model.Type.ToLower()))
                    {
                        // No need to do anything currently
                    }
                    else
                    {
                        EnterpriseResourceCreationInformation resourceInfo = new EnterpriseResourceCreationInformation()
                        {
                            Name = resourceName,
                            ResourceType = model.Type.ConvertToResourceType()
                        };

                        EnterpriseResource resource = BaseProjectContext.EnterpriseResources.Add(resourceInfo);
                        int executePoint = 1000;
                        if (i % executePoint == 0 || i == model.Count - 1)
                        {
                            ExcuteJobWithRetries(() =>
                            {
                                LoggerHelper.Instance.Comment("About to Create Resource with Name: " + resourceInfo.Name);
                                BaseProjectContext.EnterpriseResources.Update();
                                BaseProjectContext.ExecuteQuery();
                                LoggerHelper.Instance.Comment("Finish Creating Resource with Name: " + resourceInfo.Name);
                            },
                           "Create Resource");
                        }
                    }
                }
                BaseProjectContext.ExecuteQuery();
            }
            return true;
        }

        public bool Delete(ResourcePoolModel model)
        {
            IEnumerable<EnterpriseResource> resources = BaseProjectContext.LoadQuery(BaseProjectContext.EnterpriseResources.Where(item => item.Name.StartsWith(model.NamePrefix)));
            BaseProjectContext.ExecuteQuery();

            foreach (EnterpriseResource resource in resources)
            {
                BaseProjectContext.Load(resource);
                resource.DeleteObject();
                BaseProjectContext.ExecuteQuery();
            }
            return true;
        }

        public int GetCount(XMLModel model)
        {
            if (model != null && model.ResourcePoolModelList != null)
            {
                return GetEntities().Count(item => model.ResourcePoolModelList.Any(obj => item.Name.StartsWith(obj.NamePrefix)));
            }
            else
            {
                return 0;
            }
        }

        public IEnumerable<EnterpriseResource> GetEntities()
        {
            IEnumerable<EnterpriseResource> resources = null;
            ExcuteJobWithRetries(() =>
            {
                resources = BaseProjectContext.LoadQuery(BaseProjectContext.EnterpriseResources);
                BaseProjectContext.ExecuteQuery();
                resources = resources.Where(item => item.ResourceType != EnterpriseResourceType.NotSpecified);
            },
            "Query EnterpriseResource");
            return resources ?? new List<EnterpriseResource>();
        }
        #endregion
    }
}
