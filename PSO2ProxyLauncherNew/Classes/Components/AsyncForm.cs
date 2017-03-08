using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PSO2ProxyLauncherNew.Classes.Components.AsyncForms;

namespace PSO2ProxyLauncherNew.Classes.Components
{
    /// <summary>
    /// THIS IS REALLY A BAD IDEA.
    /// </summary>
    static class AsyncForm
    {
        private static Dictionary<Form, FormThreadInfo> innerDict;
        public static Dictionary<Form, FormThreadInfo> FormDictionary
        {
            get
            {
                if (innerDict == null)
                    innerDict = new Dictionary<Form, FormThreadInfo>();
                return innerDict;
            }
        }

        public static FormThreadInfo Get(Forms.AnotherExtendedForm _form)
        {
            if (FormDictionary.ContainsKey(_form))
                return FormDictionary[_form];
            else
            {
                FormThreadInfo result = new FormThreadInfo(_form);
                FormDictionary.Add(_form, result);
                return result;
            }
        }

        public static Forms.AnotherExtendedForm Get(FormThreadInfo _formThread)
        {
            if (FormDictionary.ContainsValue(_formThread))
                return _formThread.Form;
            else
            {
                FormDictionary.Add(_formThread.Form, _formThread);
                return _formThread.Form;
            }
        }

        public static void CloseAllForms()
        {
            foreach (FormThreadInfo info in FormDictionary.Values)
                info.Dispose();
            FormDictionary.Clear();
        }
    }
}
