using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ProjectServer.Client;
using Project.Performance.Model;
using Project.Performance.Utility;

namespace Project.Performance.Repository
{
    public class CustomFieldCSOMClient : PerformanceBaseClient, ICSOMClient<CustomFieldModel>
    {
        #region Variables
        private static string strLock = string.Empty;
        #endregion

        #region Constructors
        public CustomFieldCSOMClient()
        { }
        #endregion

        #region ICSOMClient
        public bool Create(CustomFieldModel model)
        {
            if (model == null)
            {
                throw new ArgumentException("The CustomFieldModel is null.");
            }
            else
            {
                if (GetEntities().Any(item => item.Name == model.Name))
                {
                    // No need to do anything currently
                }
                else
                {
                    LookupTable table = null;
                    switch (model.InputType.ToLower())
                    {
                        case "lookuptable":
                            table = new LookupTableCSOMClient().GetEntities().FirstOrDefault(item => item.Name == model.LookupTable);
                            model.Formula = null;
                            break;
                        case "freeform":
                            model.Formula = null;
                            break;
                    }

                    CustomFieldCreationInformation customFieldInfo = new CustomFieldCreationInformation()
                    {
                        Id = Guid.NewGuid(),
                        EntityType = GetEntityType(model.EntityType),
                        FieldType = model.ValueType.ConvertToCustomFieldType(),
                        LookupTable = table,
                        Formula = model.Formula,
                        Name = model.Name
                    };

                    // CSOM API doesn't support to create CustomFiled using multi threads
                    lock (CustomFieldCSOMClient.strLock)
                    {
                        bool result = ExcuteJobWithRetries(() =>
                        {
                            LoggerHelper.Instance.Comment("About to Create CustomField with Name: " + customFieldInfo.Name);
                            CustomField field = BaseProjectContext.CustomFields.Add(customFieldInfo);
                            BaseProjectContext.CustomFields.Update();
                            BaseProjectContext.ExecuteQuery();
                            LoggerHelper.Instance.Comment("Finish Creating CustomField with Name: " + customFieldInfo.Name);
                        },
                        "Create CustomField");
                        return result;
                    }
                }
            }
            return true;
        }

        public bool Delete(CustomFieldModel model)
        {
            return ExcuteJobWithRetries(() =>
            {
                IEnumerable<CustomField> customFields = BaseProjectContext.LoadQuery(BaseProjectContext.CustomFields.Where(item => item.Name == model.Name));
                BaseProjectContext.ExecuteQuery();
                CustomField customField = customFields.FirstOrDefault();
                BaseProjectContext.Load(customField);
                customField.DeleteObject();
                BaseProjectContext.ExecuteQuery();
            },
            "Delete CustomField");
        }

        public int GetCount(XMLModel model)
        {
            if (model != null && model.CustomFieldModelList != null)
            {
                return GetEntities().Count(item => model.CustomFieldModelList.Any(obj => obj.Name == item.Name));
            }
            else
            {
                return 0;
            }
        }

        public IEnumerable<CustomField> GetEntities()
        {
            IEnumerable<CustomField> customFields = null;
            ExcuteJobWithRetries(() =>
            {
                customFields = BaseProjectContext.LoadQuery(BaseProjectContext.CustomFields);
                BaseProjectContext.ExecuteQuery();
            },
            "Query CustomField");
            return customFields ?? new List<CustomField>();
        }
        #endregion

        #region Private Methods
        private EntityType GetEntityType(string entityType)
        {
            EntityType result = null;
            switch (entityType.ToLower())
            {
                case "assignmententity":
                    result = BaseProjectContext.EntityTypes.AssignmentEntity;
                    break;
                case "project":
                    result = BaseProjectContext.EntityTypes.ProjectEntity;
                    break;
                case "resource":
                    result = BaseProjectContext.EntityTypes.ResourceEntity;
                    break;
                case "tasks":
                    result = BaseProjectContext.EntityTypes.TaskEntity;
                    break;
                default:
                    throw new ArgumentException("Sorry, this type doesn't exist!");
            }
            return result;
        }
        #endregion
    }
}
