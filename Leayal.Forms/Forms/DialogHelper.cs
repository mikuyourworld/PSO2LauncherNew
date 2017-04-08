using System.Collections.Generic;

namespace Leayal.Forms
{
    public static class DialogHelper
    {
        public static string DialogFileFilterFromList(IEnumerable<FileFilterItem> prelist)
        {
            string result = string.Empty;
            using (DialogFileFilterBuilder dffb = new DialogFileFilterBuilder(prelist))
                result = dffb.ToFileFilterString();
            return result;
        }

        public static string DialogFileFilterFromList(IDictionary<string, string[]> prelist)
        {
            string result = string.Empty;
            using (DialogFileFilterBuilder dffb = new DialogFileFilterBuilder(prelist))
                result = dffb.ToFileFilterString();
            return result;
        }

        public static string DialogFileFilterFromList(IDictionary<string, IEnumerable<string>> prelist)
        {
            string result = string.Empty;
            using (DialogFileFilterBuilder dffb = new DialogFileFilterBuilder(prelist))
                result = dffb.ToFileFilterString();
            return result;
        }
    }
}
