using System;
using Microsoft.ProjectServer.Client;

namespace Project.Performance.Repository
{
    public static class RepositoryExtention
    {
        #region LookupTable
        #endregion

        #region CustomField
        public static CustomFieldType ConvertToCustomFieldType(this string vauleType)
        {
            switch (vauleType.ToLower())
            {
                case "date":
                    return CustomFieldType.DATE;
                case "duration":
                    return CustomFieldType.DURATION;
                case "cost":
                    return CustomFieldType.COST;
                case "number":
                    return CustomFieldType.NUMBER;
                case "flag":
                    return CustomFieldType.FLAG;
                case "text":
                    return CustomFieldType.TEXT;
                case "finishdate":
                    return CustomFieldType.FINISHDATE;
                default:
                    throw new ArgumentException(string.Format("The ValueType:{0} is not supported.", vauleType));
            }
        }
        #endregion

        #region EnterpriseResource
        public static EnterpriseResourceType ConvertToResourceType(this string resourceType)
        {
            switch (resourceType.ToLower())
            {
                case "notspecified":
                    return EnterpriseResourceType.NotSpecified;
                case "work":
                    return EnterpriseResourceType.Work;
                case "material":
                    return EnterpriseResourceType.Material;
                default:
                    throw new ArgumentException(string.Format("The ResourceType:{0} is not supported.", resourceType));
            }
        }
        #endregion
    }
}
