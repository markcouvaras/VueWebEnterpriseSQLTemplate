using System.Reflection;

namespace VueWebEnterpriseSQL.Infrastructure.Messaging
{
    public static class TemplateProcessor
    {
        private static readonly string _templateBasePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "Templates", "Email");

        public static string LoadTemplate(string templateName, Dictionary<string, string>? placeholders = null)
        {
            var layoutPath = Path.Combine(_templateBasePath, "_Layout.html");
            var templatePath = Path.Combine(_templateBasePath, templateName);

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Email template not found: {templateName}", templatePath);

            var body = File.ReadAllText(templatePath);

            if (File.Exists(layoutPath))
            {
                var layout = File.ReadAllText(layoutPath);
                body = layout.Replace("{{Content}}", body);
            }

            if (placeholders != null)
            {
                foreach (var (key, value) in placeholders)
                {
                    body = body.Replace($"{{{{{key}}}}}", value);
                }
            }

            return body;
        }
    }
}
