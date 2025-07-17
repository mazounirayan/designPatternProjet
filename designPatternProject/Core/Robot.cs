namespace RobotFactory.Core
{
    public class Robot
    {
        public string TemplateName { get; set; }

        public Robot(string templateName)
        {
            TemplateName = templateName;
        }

        public override string ToString() => TemplateName;
    }
}
