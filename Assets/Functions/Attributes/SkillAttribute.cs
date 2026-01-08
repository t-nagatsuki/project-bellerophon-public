using System;

namespace Functions.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SkillAttribute : Attribute
    {
        public string Name { get; }
        public SkillAttribute(string name)
        {
            Name = name;
        }
    }
}