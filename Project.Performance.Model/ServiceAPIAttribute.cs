using System;

namespace Project.Performance.Model
{
    public class ServiceAPIAttribute : Attribute
    {
        public Type ModelType { get; private set; }

        public ServiceAPIAttribute(Type modelType)
        {
            this.ModelType = modelType;
        }
    }
}
